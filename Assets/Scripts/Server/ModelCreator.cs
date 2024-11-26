using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.Threading.Tasks;
using ServerStructs;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ModelCreator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetStatic()
    {
        textureColors = new();
    }
    public static Dictionary<Color, float> textureColors = new();
    public static Dictionary<string, Color> htmlColors = new();
    public static void FillDicStart(Texture2D paletteTexture2D)
    {
        for (int i = 0; i < paletteTexture2D.width; i++)
        {
            Color newColor = paletteTexture2D.GetPixel(i, 0);
            htmlColors[ColorUtility.ToHtmlStringRGB(newColor)] = newColor;
            float newPosition = ((float)i / (float)paletteTexture2D.width) + ((1f / (float)paletteTexture2D.width) / 2);

            textureColors[paletteTexture2D.GetPixel(i, 0)] = newPosition;
        }
    }

    public static Mesh BuildProp(VoxUnitData[] Udata, GameObject go, Texture2D paletteTexture2D, Material paletteBase, GameObject voxelPrefab)
    {
        // NOTE(sqdrck): To support local matrix.
        go = go.transform.GetChild(0).gameObject;

        Voxel[,,] voxelMap;
        List<Voxel> voxelList;
        float xcenter = 0;
        float ycenter = 0;
        float zcenter = 0;

        voxelMap = new Voxel[17, 17, 17];
        voxelList = new List<Voxel>();
        foreach (var item in Udata)
        {
            Voxel newVox = new Voxel(new Vector3Int(item.IntX, item.IntZ, item.IntY), item.color.ToUpper());
            voxelList.Add(newVox);
            voxelMap[item.IntX, item.IntZ, item.IntY] = newVox;
            xcenter += item.x;
            ycenter += item.y;
            zcenter += item.z;
        }

        VoxelFullModel fullModel = new VoxelFullModel(voxelMap, voxelList);

        xcenter = xcenter / Udata.Length;
        ycenter = ycenter / Udata.Length;
        zcenter = zcenter / Udata.Length;

        int count = 0;

        foreach (OptimizedVoxelCube MegaVoxel in fullModel._optimizedVoxels)
        {
            GameObject voxel;

            voxel = GameObject.Instantiate(voxelPrefab, go.transform, true);
            MeshFilter cubeMesh = voxel.GetComponent<MeshFilter>();
            Renderer cubeRenderer = voxel.GetComponent<Renderer>();

            voxel.transform.localRotation = Quaternion.identity;
            voxel.transform.localScale = MegaVoxel.scale * 0.1f;

            cubeMesh.mesh = CreateCube(MegaVoxel._front, MegaVoxel._top, MegaVoxel._right, MegaVoxel._left, MegaVoxel._back, MegaVoxel._bottom);

            Vector3 pos = new(
                MegaVoxel.coordinates.x * 0.1f - (xcenter * 0.1f),
                    MegaVoxel.coordinates.y * 0.1f - (zcenter * 0.1f),
                    MegaVoxel.coordinates.z * 0.1f - (ycenter * 0.1f));

            voxel.transform.localPosition = pos;

            cubeRenderer.sharedMaterial = paletteBase;

            Color color = htmlColors[MegaVoxel.color];
            float uvX = textureColors[color];
            var uv = new Vector2[cubeMesh.sharedMesh.vertices.Length];

            for (int c = 0; c < uv.Length; c++)
                uv[c] = new Vector2(uvX, 0f);

            cubeMesh.sharedMesh.uv = uv;
            count += 1;
        }

        var mesh = go.GetComponent<MeshCombiner>().CombineMeshes(true);
        go.AddComponent<MeshCollider>();

        GameObject.Destroy(go.GetComponent<MeshCombiner>());
        return mesh;
    }

#if false
    public static void RefrashItemOnClient(string json, GameObject nameGo)
    {
        var go = nameGo;
        var data = JsonConvert.DeserializeObject<UserVoxModelDataShort>(json);

        go.name = data.modelId;

        var Udata = data.vox_model_data.vox_unit_datas;

        BuildProp(Udata, go);

#if UNITY_EDITOR
        MeshFilter mf = go.GetComponent<MeshFilter>();

        if (!Directory.Exists(Path.GetDirectoryName("Assets/Resources/NewPropsRef/")))
        {
            Directory.CreateDirectory(Path.GetDirectoryName("Assets/Resources/NewPropsRef/"));
        }
        var savePath = "Assets/Resources/NewPropsRef/" + go.name + ".asset";

        AssetDatabase.CreateAsset(mf.sharedMesh, savePath);

        if (!Directory.Exists(Path.GetDirectoryName(Application.dataPath + "/Resources/NewProps/")))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Application.dataPath + "/Resources/NewProps/"));
        }
        PrefabUtility.SaveAsPrefabAssetAndConnect(go, Application.dataPath + "/Resources/NewProps/" + go.name + ".prefab", InteractionMode.UserAction);
        //DestroyImmediate(go.GetComponent<PhotonView>());
        //DestroyImmediate(go.GetComponent<PhotonTransformView>());
#else
        //Destroy(go.GetComponent<PhotonView>());
        //Destroy(go.GetComponent<PhotonTransformView>());
#endif

        go.GetComponent<MeshRenderer>().enabled = false;
        go.GetComponent<MeshCollider>().isTrigger = true;
    }
#endif

    private static int GetNeighbouringCount(Vector3Int voxel, VoxUnitData[] voxels)
    {
        int count = 0;
        foreach (var item in voxels)
        {
            if (count == 6) break;
            Vector3Int otherVoxPos = new Vector3Int((int)item.IntX, (int)item.IntZ, (int)item.IntY);
            if (Vector3Int.Distance(otherVoxPos, voxel) == 1)
            {
                count++;
            }
        }
        return count;
    }

    private static bool[] GetNeighbouringSides(Vector3Int voxel, VoxUnitData[] voxels)
    {
        // int count = 0;
        bool[] sides =
        {
            true,true,true,true,true,true
        };//front,top,right,left,back,bottom
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX, (int)item.IntZ, (int)item.IntY - 1);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//FRONT maybe right
            {
                sides[0] = false;
                break;
            }
        }
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX, (int)item.IntZ, (int)item.IntY + 1);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//BACK
            {
                sides[4] = false;
                break;
            }
        }
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX, (int)item.IntZ - 1, (int)item.IntY);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//TOP
            {
                sides[1] = false;
                break;
            }
        }
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX, (int)item.IntZ + 1, (int)item.IntY);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//BOTTOM
            {
                sides[5] = false;
                break;
            }
        }
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX - 1, (int)item.IntZ, (int)item.IntY);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//RIGHT
            {
                sides[2] = false;
                break;
            }
        }
        foreach (var item in voxels)
        {
            Vector3Int checkPos = new Vector3Int((int)item.IntX + 1, (int)item.IntZ, (int)item.IntY);

            if (Vector3Int.Distance(checkPos, voxel) == 0)//LEFT
            {
                sides[3] = false;
                break;
            }
        }

        return sides;
    }

    private static Mesh CreateCube(bool _front, bool _top, bool _right, bool _left, bool _back, bool _bottom)
    {

        Mesh mesh = new Mesh();
        mesh.Clear();

        //2) Define the cube's dimensions
        float length = 1f;
        float width = 1f;
        float height = 1f;


        //3) Define the co-ordinates of each Corner of the cube 
        Vector3[] c = new Vector3[8];

        c[0] = new Vector3(-length * .5f, -width * .5f, height * .5f);
        c[1] = new Vector3(length * .5f, -width * .5f, height * .5f);
        c[2] = new Vector3(length * .5f, -width * .5f, -height * .5f);
        c[3] = new Vector3(-length * .5f, -width * .5f, -height * .5f);

        c[4] = new Vector3(-length * .5f, width * .5f, height * .5f);
        c[5] = new Vector3(length * .5f, width * .5f, height * .5f);
        c[6] = new Vector3(length * .5f, width * .5f, -height * .5f);
        c[7] = new Vector3(-length * .5f, width * .5f, -height * .5f);


        //4) Define the vertices that the cube is composed of:
        //I have used 16 vertices (4 vertices per side). 
        //This is because I want the vertices of each side to have separate normals.
        //(so the object renders light/shade correctly) 
        Vector3[] vertices = new Vector3[]
        {
            c[0], c[1], c[2], c[3], // Bottom
	        c[7], c[4], c[0], c[3], // Left
	        c[4], c[5], c[1], c[0], // Front
	        c[6], c[7], c[3], c[2], // Back
	        c[5], c[6], c[2], c[1], // Right
	        c[7], c[6], c[5], c[4]  // Top
        };

        //5) Define each vertex's Normal
        Vector3 up = Vector3.up;
        Vector3 down = Vector3.down;
        Vector3 forward = Vector3.forward;
        Vector3 back = Vector3.back;
        Vector3 left = Vector3.left;
        Vector3 right = Vector3.right;


        Vector3[] normals = new Vector3[]
        {
            down, down, down, down,             // Bottom
	        left, left, left, left,             // Left
	        forward, forward, forward, forward,	// Front
	        back, back, back, back,             // Back
	        right, right, right, right,         // Right
	        up, up, up, up                      // Top
        };


        //7) Define the Polygons (triangles) that make up the our Mesh (cube)
        //IMPORTANT: Unity uses a 'Clockwise Winding Order' for determining front-facing polygons.
        //This means that a polygon's vertices must be defined in 
        //a clockwise order (relative to the camera) in order to be rendered/visible.
        List<int> realTriangles = new List<int>();
        int[] triangles = new int[]
        {
            3, 1, 0,        3, 2, 1,        // Bottom	
	        7, 5, 4,        7, 6, 5,        // Left
	        11, 9, 8,       11, 10, 9,      // Front
	        15, 13, 12,     15, 14, 13,     // Back
	        19, 17, 16,     19, 18, 17,	    // Right
	        23, 21, 20,     23, 22, 21,     // Top
        };
        if (_bottom) { realTriangles.Add(3); realTriangles.Add(1); realTriangles.Add(0); realTriangles.Add(3); realTriangles.Add(2); realTriangles.Add(1); }
        if (_left) { realTriangles.Add(7); realTriangles.Add(5); realTriangles.Add(4); realTriangles.Add(7); realTriangles.Add(6); realTriangles.Add(5); }//left
        if (_front) { realTriangles.Add(11); realTriangles.Add(9); realTriangles.Add(8); realTriangles.Add(11); realTriangles.Add(10); realTriangles.Add(9); }//front
        if (_back) { realTriangles.Add(15); realTriangles.Add(13); realTriangles.Add(12); realTriangles.Add(15); realTriangles.Add(14); realTriangles.Add(13); }
        if (_right) { realTriangles.Add(19); realTriangles.Add(17); realTriangles.Add(16); realTriangles.Add(19); realTriangles.Add(18); realTriangles.Add(17); }
        if (_top) { realTriangles.Add(23); realTriangles.Add(21); realTriangles.Add(20); realTriangles.Add(23); realTriangles.Add(22); realTriangles.Add(21); }



        //8) Build the Mesh
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = realTriangles.ToArray();
        mesh.normals = normals;
        mesh.Optimize();


        return mesh;
    }
    class VoxelFullModel
    {
        public VoxelFullModel(Voxel[,,] voxels, List<Voxel> listOfAllVoxels)
        {
            voxelMap = voxels;

            allVoxels = new List<Voxel>();
            foreach (var item in voxelMap)
            {
                if (item != null)
                {
                    allVoxels.Add(item);
                }

            }
            Init();
        }
        Voxel[,,] voxelMap = new Voxel[17, 17, 17];

        internal List<Voxel> allVoxels;
        private List<VoxelColorGroup> _allColorGroups;

        internal List<OptimizedVoxelCube> _optimizedVoxels;

        private void Init()
        {
            CutInvisbleVoxels();
            _allColorGroups = FillColorGroups();
            _optimizedVoxels = FindOptimizedVoxelCubes();
        }

        private List<VoxelColorGroup> FillColorGroups()
        {
            List<VoxelColorGroup> colorGroups = new List<VoxelColorGroup>();
            foreach (var voxel in allVoxels)
            {
                bool colorGroupFinded = false;
                foreach (var colorGroup in colorGroups)
                {
                    if (colorGroup.color == voxel.color)
                    {
                        colorGroupFinded = true;

                        voxel.colorGroup = colorGroup;
                        colorGroup.voxels[voxel.position.x, voxel.position.y, voxel.position.z] = voxel;

                        break;
                    }
                }
                if (!colorGroupFinded)
                {
                    VoxelColorGroup colorGroup = new VoxelColorGroup();
                    colorGroup.color = voxel.color;
                    colorGroup.voxels[voxel.position.x, voxel.position.y, voxel.position.z] = voxel;
                    colorGroups.Add(colorGroup);
                }
            }
            return colorGroups;
        }
        private List<OptimizedVoxelCube> FindOptimizedVoxelCubes()
        {
            List<OptimizedVoxelCube> optimizedVoxelCubes = new List<OptimizedVoxelCube>();

            foreach (var colorGroup in _allColorGroups)
            {
                foreach (var voxel in colorGroup.voxels)
                {
                    if (voxel != null && !voxel.isInGroup)
                    {
                        OptimizedVoxelCube optimizedVoxelCube = new OptimizedVoxelCube();
                        optimizedVoxelCube.voxelsInGroup = FindOptimized(colorGroup, voxel);
                        optimizedVoxelCube.color = colorGroup.color;
                        optimizedVoxelCube.CalculateCube();

                        optimizedVoxelCubes.Add(optimizedVoxelCube);

                        foreach (var item in optimizedVoxelCube.voxelsInGroup)
                        {
                            item.isInGroup = true;
                        }

                    }
                }
                //for (int i = 0; i < colorGroup.voxels.Length; i++)
                //{

                //}
            }
            List<Voxel> FindOptimized(VoxelColorGroup colorGroup, Voxel startVoxel)
            {
                List<Voxel> voxelsToCombine = new List<Voxel>();

                // ��� ��� ����� ������� �������������

                List<Voxel> x_voxels_rectangle = new List<Voxel>();
                List<Voxel> y_voxels_rectangle = new List<Voxel>();
                List<Voxel> z_voxels_rectangle = new List<Voxel>();

                List<Voxel> biggest_rectangle = new List<Voxel>();

                Vector3Int startPos = startVoxel.position;
                bool x_stop = false;
                bool y_stop = false;
                bool z_stop = false;

                bool x_best_rectangle = false;
                bool y_best_rectangle = false;
                bool z_best_rectangle = false;


                for (int i = 0; i < 17; i++)
                {
                    if (startPos.x + i <= 16 && !x_stop && colorGroup.voxels[startPos.x + i, startPos.y, startPos.z] != null && !colorGroup.voxels[startPos.x + i, startPos.y, startPos.z].isInGroup)
                    {
                        x_voxels_rectangle.Add(colorGroup.voxels[startPos.x + i, startPos.y, startPos.z]);
                    }
                    else x_stop = true;

                    if (startPos.y + i <= 16 && !y_stop && colorGroup.voxels[startPos.x, startPos.y + i, startPos.z] != null && !colorGroup.voxels[startPos.x, startPos.y + i, startPos.z].isInGroup)
                    {
                        y_voxels_rectangle.Add(colorGroup.voxels[startPos.x, startPos.y + i, startPos.z]);
                    }
                    else y_stop = true;

                    if (startPos.z + i <= 16 && !z_stop && colorGroup.voxels[startPos.x, startPos.y, startPos.z + i] != null && !colorGroup.voxels[startPos.x, startPos.y, startPos.z + i].isInGroup)
                    {
                        z_voxels_rectangle.Add(colorGroup.voxels[startPos.x, startPos.y, startPos.z + i]);
                    }
                    else z_stop = true;
                }
                if (x_voxels_rectangle.Count >= y_voxels_rectangle.Count && x_voxels_rectangle.Count >= z_voxels_rectangle.Count)
                {
                    biggest_rectangle = x_voxels_rectangle;
                    x_best_rectangle = true;
                }
                else if (y_voxels_rectangle.Count > x_voxels_rectangle.Count && y_voxels_rectangle.Count > z_voxels_rectangle.Count)
                {
                    biggest_rectangle = y_voxels_rectangle;
                    y_best_rectangle = true;
                }
                else if (z_voxels_rectangle.Count > x_voxels_rectangle.Count && z_voxels_rectangle.Count > y_voxels_rectangle.Count)
                {
                    biggest_rectangle = z_voxels_rectangle;
                    z_best_rectangle = true;
                }
                else if (x_voxels_rectangle.Count > 0)
                {
                    biggest_rectangle = x_voxels_rectangle;
                    x_best_rectangle = true;
                }
                else if (y_voxels_rectangle.Count > 0)
                {
                    biggest_rectangle = y_voxels_rectangle;
                    y_best_rectangle = true;
                }
                else if (z_voxels_rectangle.Count > 0)
                {
                    biggest_rectangle = z_voxels_rectangle;
                    z_best_rectangle = true;
                }


                List<Voxel> biggest_square = new List<Voxel>();
                List<Voxel> x_square = new List<Voxel>();//X DIRECTION
                List<Voxel> y_square = new List<Voxel>();//Y DIRECTION
                List<Voxel> z_square = new List<Voxel>();//Z DIRECTION

                bool x_square_stop = false;
                bool y_square_stop = false;
                bool z_square_stop = false;
                for (int step = 0; step < 17; step++)
                {
                    List<Voxel> x_square_iteration = new List<Voxel>();//X DIRECTION
                    List<Voxel> y_square_iteration = new List<Voxel>();//Y DIRECTION
                    List<Voxel> z_square_iteration = new List<Voxel>();//Z DIRECTION
                    foreach (var voxel in biggest_rectangle)
                    {
                        if (!x_square_stop /*&& !x_best_rectangle*/)
                        {
                            Vector3Int checkPos = voxel.position + new Vector3Int(step, 0, 0);//X DIRECTION
                            if (checkPos.x > 16 || checkPos.y > 16 || checkPos.z > 16)
                            {
                                x_square_stop = true;
                            }
                            else if (colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z] != null && !colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z].isInGroup)
                            {
                                x_square_iteration.Add(colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z]);
                            }
                            else x_square_stop = true;
                        }
                        if (!y_square_stop /*&& !y_best_rectangle*/)
                        {
                            Vector3Int checkPos = voxel.position + new Vector3Int(0, step, 0);//Y DIRECTION
                            if (checkPos.x > 16 || checkPos.y > 16 || checkPos.z > 16)
                            {
                                y_square_stop = true;
                            }
                            else if (colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z] != null && !colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z].isInGroup)
                            {
                                y_square_iteration.Add(colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z]);
                            }
                            else y_square_stop = true;
                        }
                        if (!z_square_stop /*&& !z_best_rectangle*/)
                        {

                            Vector3Int checkPos = voxel.position + new Vector3Int(0, 0, step);//Z DIRECTION
                            if (checkPos.x > 16 || checkPos.y > 16 || checkPos.z > 16)
                            {
                                z_square_stop = true;
                            }
                            else if (colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z] != null && !colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z].isInGroup)
                            {
                                z_square_iteration.Add(colorGroup.voxels[checkPos.x, checkPos.y, checkPos.z]);
                            }
                            else z_square_stop = true;
                        }
                    }
                    if (!x_square_stop) x_square.AddRange(x_square_iteration);
                    if (!y_square_stop) y_square.AddRange(y_square_iteration);
                    if (!z_square_stop) z_square.AddRange(z_square_iteration);


                }
                if (!x_best_rectangle && (x_square.Count >= y_square.Count && x_square.Count >= z_square.Count))
                {
                    biggest_square = x_square;
                }
                if (!y_best_rectangle && (y_square.Count > x_square.Count && y_square.Count > z_square.Count))
                {
                    biggest_square = y_square;
                }
                if (!z_best_rectangle && (z_square.Count > x_square.Count && z_square.Count > y_square.Count))
                {
                    biggest_square = z_square;
                }

                if (biggest_square.Count > biggest_rectangle.Count)
                {
                    voxelsToCombine = biggest_square;
                    // Debug.Log("MUST BE BIGGEST SQUARE");
                }
                else if (biggest_square.Count < biggest_rectangle.Count)
                {
                    voxelsToCombine = biggest_rectangle;
                }
                else if (biggest_square.Count == biggest_rectangle.Count)
                {
                    voxelsToCombine = biggest_rectangle;
                }

                return voxelsToCombine;
            }
            return optimizedVoxelCubes;
        }
        private void CutInvisbleVoxels()
        {

            for (int x = 0; x < voxelMap.GetLength(0); x++)
            {
                for (int y = 0; y < voxelMap.GetLength(1); y++)
                {
                    for (int z = 0; z < voxelMap.GetLength(2); z++)
                    {
                        if (voxelMap[x, y, z] != null)
                        {
                            Voxel voxel = voxelMap[x, y, z];
                            //Debug.Log("Check is hidden voxel or visible"); 
                            if (GetNeighbouringCountOptimized(voxel) == 6)
                            {
                                //Debug.Log("Hidden voxel");
                                voxel.hidden = true;
                                allVoxels.Remove(voxel);

                            }
                        }
                        else
                        {
                            //Debug.Log("Voxel is null"); 
                        }
                    }
                }
            }

            // Debug.Log("Cut Invisible voxels after : " + allVoxels.Count);
        }

        private int GetNeighbouringCountOptimized(Voxel voxel)
        {
            int count = 0;

            if (voxel.position.x != 0 && voxelMap[voxel.position.x - 1, voxel.position.y, voxel.position.z] != null)
            {
                count += 1;
                voxel._left = false;
            }
            if (voxel.position.x != 16 && voxelMap[voxel.position.x + 1, voxel.position.y, voxel.position.z] != null)
            {
                count += 1;
                voxel._right = false;
            }
            if (voxel.position.y != 0 && voxelMap[voxel.position.x, voxel.position.y - 1, voxel.position.z] != null)
            {
                count += 1;
                voxel._bottom = false;
            }
            if (voxel.position.y != 16 && voxelMap[voxel.position.x, voxel.position.y + 1, voxel.position.z] != null)
            {
                count += 1;
                voxel._top = false;
            }
            if (voxel.position.z != 0 && voxelMap[voxel.position.x, voxel.position.y, voxel.position.z - 1] != null)
            {
                count += 1;
                voxel._back = false;
            }
            if (voxel.position.z != 16 && voxelMap[voxel.position.x, voxel.position.y, voxel.position.z + 1] != null)
            {
                count += 1;
                voxel._front = false;
            }

            return count;
        }
    }
    class OptimizedVoxelCube
    {
        internal Vector3 coordinates;
        internal Vector3 scale;
        internal string color;
        internal List<Voxel> voxelsInGroup;


        internal bool _front = false;
        internal bool _top = false;
        internal bool _right = false;
        internal bool _left = false;
        internal bool _back = false;
        internal bool _bottom = false;

        internal void CalculateCube()// 1 1 1     1 1 2
        {
            float xmin = 16;
            float ymin = 16;
            float zmin = 16;

            float xmax = 0;
            float ymax = 0;
            float zmax = 0;

            foreach (var voxel in voxelsInGroup)
            {
                if (voxel._front) _front = true;
                if (voxel._top) _top = true;
                if (voxel._right) _right = true;
                if (voxel._left) _left = true;
                if (voxel._back) _back = true;
                if (voxel._bottom) _bottom = true;

                if (voxel.position.x < xmin) xmin = voxel.position.x;
                if (voxel.position.y < ymin) ymin = voxel.position.y;
                if (voxel.position.z < zmin) zmin = voxel.position.z;

                if (voxel.position.x > xmax) xmax = voxel.position.x;
                if (voxel.position.y > ymax) ymax = voxel.position.y;
                if (voxel.position.z > zmax) zmax = voxel.position.z;
            }


            //coordinates = new Vector3(xmax - (xmax - xmin) /2, ymax - (ymin + ymax) / 2,zmax - (zmin + zmax) / 2);

            scale = new Vector3(xmax - xmin + 1, ymax - ymin + 1, zmax - zmin + 1);

            coordinates = new Vector3((xmin + xmax) / 2, (ymax + ymin) / 2, (zmax + zmin) / 2);

        }
    }

    public class VoxelColorGroup
    {

        internal string color;
        internal Voxel[,,] voxels = new Voxel[17, 17, 17];

    }

    [System.Serializable]
    public class Voxel
    {
        internal bool hidden = false;
        internal bool isInGroup = false;
        internal VoxelColorGroup colorGroup;

        internal Vector3Int position;
        internal string color;
        public Voxel(Vector3Int pos, string c)
        {
            position = pos;
            color = c;
        }

        internal bool _front = true;
        internal bool _top = true;
        internal bool _right = true;
        internal bool _left = true;
        internal bool _back = true;
        internal bool _bottom = true;
    }
}
