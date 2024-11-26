using System;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using AOT;
using Unity.VectorGraphics;
using ServerStructs;
using Newtonsoft.Json;
using BestHTTP;

public static class Server
{
    public const string MEGAOBBY_ID = "1288038048000380948";
    public const string MEGAACTIVITY_ID = "1288038048000380948";
    public static void AddMegamodHeaders(HTTPRequest req)
    {
        req.AddHeader("Content-Type", "application/json");
        req.AddHeader("Authorization", $"Bearer 409145e8-c804-40e0-b12c-9942e72d98ae");
    }

    public const string DISCORD_PROXY_API_PATH_MEGAOBBY = "https://1288038048000380948.discordsays.com/.proxy/api/better-space/";
    public const string DISCORD_PROXY_API_PATH_MEGAACTIVITY = "https://1259815757441667165.discordsays.com/.proxy/api/better-space/";
    public const string BETTERSPACE_API_PATH = "https://better-space-api.herokuapp.com/api/";

    public const string DISCORD_PROXY_API_PATH_REPLAYS_MEGAOBBY =
        "https://1288038048000380948.discordsays.com/.proxy/api/replays/";
    public const string DISCORD_PROXY_API_PATH_REPLAYS_MEGAACTIVITY =
        "https://1259815757441667165.discordsays.com/.proxy/api/replays/";


    public const string DISCORD_PROXY_API_PATH_IMAGES_MEGAOBBY =
        "https://1288038048000380948.discordsays.com/.proxy/api/images/";
    public const string DISCORD_PROXY_API_PATH_IMAGES_MEGAACTIVITY =
        "https://1259815757441667165.discordsays.com/.proxy/api/images/";
    
    public static string GetBasePath()
    {
        return BETTERSPACE_API_PATH;
    }

    // SAVE-5FCA12F81FEF3006?game=true
    private static string GET_MODELS_URL = GetBasePath() + "props-models/";
    private static string GET_SAVE_URL = GetBasePath() + "saves/";
    private static string GET_NEXT_GAME_URL = GetBasePath() + "games/next-game/";
    private static string GET_SKIN_URL = GetBasePath() + "model/";
    private static string GET_SKINS_PAGE_URL = GetBasePath() + "model/list/SKIN";
    private static string REPLAYS_URL = GetBasePath() + "replays/";
    
    public static void DownloadFile(string url, Action<byte[]> onFinished)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            if (response.IsSuccess)
            {
                onFinished?.Invoke(response.Data);
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }
    }
    
    public static void DownloadTexture(string url, Action<Texture2D> onFinished)
    {
        Uri uri = new Uri(url);
        Debug.Log(uri);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        // AddMegamodHeaders(req);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            if (response.IsSuccess)
            {
                onFinished?.Invoke(response.DataAsTexture2D);
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }
    }

    public static void GetSkinsPage(int page, Action<SkinsPageData> onFinished)
    {
        Uri uri = new Uri(GET_SKINS_PAGE_URL + "?page=" + page.ToString());
        Debug.Log(uri);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        AddMegamodHeaders(req);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("GetSkin response:");
            Debug.Log(response.DataAsText);
            if (response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<SkinsPageData>(response.DataAsText);
                onFinished?.Invoke(model);
            }
            else
            {
                onFinished?.Invoke(default);
            }
        }
    }

    public static void GetSkin(string skinId, Action<FullModelData> onFinished)
    {
        Uri uri = new Uri(GET_SKIN_URL + skinId);
        Debug.Log(uri);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        AddMegamodHeaders(req);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("GetSkin response:");
            Debug.Log(response.DataAsText);
            if (response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<FullModelData>(response.DataAsText);
                onFinished?.Invoke(model);
            }
            else
            {
                onFinished?.Invoke(default);
            }
        }
    }

    // public static void GetSkinTexture(FullModelData data, Action<Texture2D> onFinished)
    // {
    //     int needToLoad = data.parts.Length;
    //     int loaded = 0;
    //     var parts = new UserVoxModelDataShort[data.parts.Length];
    //     var skin = new Texture2D(64, 32, TextureFormat.ARGB32, false);
    //     foreach (var part in data.parts)
    //     {
    //         Server.GetSkinPart(part, (UserVoxModelDataShort partData) =>
    //         {
    //             parts[loaded] = partData;
    //             loaded++;
    //             Debug.Log("Loaded skin part: " + partData.modelId);
    //             Debug.Log("Unit datas" + partData.vox_model_data.vox_unit_datas.Length);
    //             SkinToTexture.SetPart(skin, partData.modelId[0], partData.vox_model_data.vox_unit_datas);
    //             if (loaded == needToLoad)
    //             {
    //                 onFinished?.Invoke(skin);
    //             }
    //         });
    //     }
    // }

    // public static void GetSkinTexture(string skinId, Action<Texture2D> onFinished)
    // {
    //     Server.GetSkin(skinId, (FullModelData data) => { GetSkinTexture(data, onFinished); });
    // }

    public static void GetSkinPart(string partId, Action<UserVoxModelDataShort> onFinished)
    {
        Uri uri = new Uri(GET_SKIN_URL + partId);
        Debug.Log(uri);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        AddMegamodHeaders(req);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("GetPart response:");
            Debug.Log(response.DataAsText);
            if (response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<UserVoxModelDataShort>(response.DataAsText);
                onFinished?.Invoke(model);
            }
            else
            {
                onFinished?.Invoke(default);
            }
        }
    }

    public class SpawnSaveInfo
    {
        public GameObject root;
        public GameObject[] props;
        public SaveSettings settings;
        public Dictionary<string, List<GameObject>> specials;
        public string ownerName;
    }

    public static void GetNextGame(string currentGameId, Action<string> onFinished)
    {
        Debug.Log(GET_NEXT_GAME_URL + currentGameId);
        Uri uri = new Uri(GET_NEXT_GAME_URL + currentGameId);
        var req = new HTTPRequest(uri, HTTPMethods.Get, cb);

        AddMegamodHeaders(req);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            Debug.Log("GetNextGame response:");
            Debug.Log(response.DataAsText);
            if (response.IsSuccess)
            {
                onFinished?.Invoke(response.DataAsText);
            }
            else
            {
                onFinished?.Invoke(null);
            }
        }
    }

    public static void SpawnSave(string id, Action<SpawnSaveInfo> onFinished)
    {
        GetSave(id, onGotSave);
        void onGotSave(Save save)
        {
            SpawnProps(save.models, onSpawned);
            void onSpawned(SpawnSaveInfo result)
            {
                result.ownerName = save.userData.owner_name;
                result.settings = save.settings;
                onFinished?.Invoke(result);
            }
        }
    }

    public static void SpawnSave(TextAsset jsonAsset, TextAsset modelsAsset, Action<SpawnSaveInfo> onFinished)
    {
        var save = JsonConvert.DeserializeObject<Save>(jsonAsset.text);
        SpawnProps(save.models, modelsAsset, onFinished);
    }

    public static void SpawnProps(Prop[] propsInfos, TextAsset modelsCreateInfoJson, Action<SpawnSaveInfo> onFinished)
    {
        var result = SpawnPropsWithModelData(modelsCreateInfoJson.text, propsInfos);
        onFinished?.Invoke(result);
    }


    public static void GetSave(string id, Action<Save> onFinished)
    {
        string cachedFilePath = Path.Combine(Application.persistentDataPath, id + ".json");

#if UNITY_EDITOR
        if (File.Exists(cachedFilePath))
        {
            Save result;
            try
            {
                string cachedData = File.ReadAllText(cachedFilePath);
                result = JsonConvert.DeserializeObject<Save>(cachedData);
                Debug.Log("Loaded from cache: " + cachedFilePath);
            }
            catch (Exception e)
            {
                result = null;
                Debug.LogError("Error reading cache: " + e.Message);
            }
            onFinished?.Invoke(result);
        }
        else
#endif
        {
            var req = new HTTPRequest(new Uri(GET_SAVE_URL + id), HTTPMethods.Get, cb);
            Debug.Log("GetSave: " + new Uri(GET_SAVE_URL + id));

            AddMegamodHeaders(req);
            req.Send();

            void cb(HTTPRequest request, HTTPResponse response)
            {
                Debug.Log("GetSave response:");
                Save result = null;
                if (response.IsSuccess)
                {
                    Debug.Log(response.DataAsText);
                    try
                    {
                        result = JsonConvert.DeserializeObject<Save>(response.DataAsText);
#if UNITY_EDITOR
                        try
                        {
                            File.WriteAllText(cachedFilePath, response.DataAsText);
                            Debug.Log("Saved to cache: " + cachedFilePath);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError("Error saving cache: " + e.Message);
                        }
#endif
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Deserialization error: " + e.Message);
                    }
                }
                else
                {
                    Debug.LogError("Request error: " + response.DataAsText);
                }

                onFinished?.Invoke(result);
            }
        }
    }

    public static string[] GetIds(Prop[] props)
    {
        HashSet<string> ids = new();
        for (int i = 0; i < props.Length; i++)
        {
            ids.Add(props[i].id);
        }
        string[] result = ids.ToArray();
        return result;
    }

    public static void SpawnProps(Prop[] propsInfos, Action<SpawnSaveInfo> onFinished)
    {
        var data = new PostPropsModelsData(GetIds(propsInfos));
        string requestDataJson = JsonConvert.SerializeObject(data);

#if UNITY_EDITOR
        // Generate a cache key based on the request data
        string cacheKey = GenerateCacheKey(requestDataJson);
        string cachedFilePath = Path.Combine(Application.persistentDataPath, cacheKey + "_props.json");

        if (File.Exists(cachedFilePath))
        {
            SpawnSaveInfo result;
            try
            {
                string cachedData = File.ReadAllText(cachedFilePath);
                Debug.Log("Loaded props data from cache.");
                result = SpawnPropsWithModelData(cachedData, propsInfos);
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading cache: " + e.Message);
                result = null;
            }
            onFinished?.Invoke(result);
        }
        else
#endif
        {
            // Make HTTP request
            var req = new HTTPRequest(new Uri(GET_MODELS_URL), HTTPMethods.Post, cb);
            req.RawData = Encoding.UTF8.GetBytes(requestDataJson);
            AddMegamodHeaders(req);
            req.Send();

            void cb(HTTPRequest request, HTTPResponse response)
            {
                if (response.IsSuccess)
                {
                    Debug.Log(response.DataAsText);
                    var result = SpawnPropsWithModelData(response.DataAsText, propsInfos);
                    onFinished?.Invoke(result);

#if UNITY_EDITOR
                    // Save response data to cache
                    try
                    {
                        File.WriteAllText(cachedFilePath, response.DataAsText);
                        Debug.Log("Saved props data to cache.");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Error writing cache: " + e.Message);
                    }
#endif
                }
                else
                {
                    Debug.LogWarning("Failed to get prop model:");
                    Debug.Log(response.DataAsText);
                    onFinished?.Invoke(null);
                }
            }
        }
    }

    // Helper method to generate a cache key based on request data
    private static string GenerateCacheKey(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public static SpawnSaveInfo SpawnPropsWithModelData(string dataJson, Prop[] propsInfos)
    {
        System.Diagnostics.Stopwatch sw = new();
        sw.Start();

        GameObject root = new GameObject();
        root.name = "SAVE";
        var models = new LoadPropsModelsDataResponse() { vox_model_data = JsonConvert.DeserializeObject<UserVoxModelDataShort[]>(dataJson) };

        Debug.Log("Models downloaded: " + models.vox_model_data.Length);
        if (models.vox_model_data.Length == 0)
        {
            Debug.LogWarning("Got zero models.");
            return null;
        }

        Dictionary<string, VoxModelData> modelsCreateInfo = new();

        for (int i = 0; i < models.vox_model_data.Length; i++)
        {
            modelsCreateInfo[models.vox_model_data[i].modelId] = models.vox_model_data[i].vox_model_data;
        }

        SpawnSaveInfo result = new();
        result.specials = new Dictionary<string, List<GameObject>>();
        result.props = new GameObject[propsInfos.Length];
        result.root = root;
        Dictionary<string, GameObject> cache = new();

        ModelCreator.FillDicStart(SettingsSO.paletteTexture2D);
        for (int i = 0; i < propsInfos.Length; i++)
        {
            Prop prop = propsInfos[i];
            GameObject cached;
            GameObject go;

            // if (prop.id.ToLower().StartsWith("x42") || prop.id.ToLower().StartsWith("16x"))
            try
            {
                if (cache.TryGetValue(prop.id, out cached))
                {
                    go = GameObject.Instantiate(cached, root.transform);
                }
                else
                {
                    GameObject special;
                    if (SettingsSO.specialProps.TryGetValue(prop.id, out special))
                    {
                        go = GameObject.Instantiate(special, root.transform);
                        if (!result.specials.ContainsKey(prop.id))
                        {
                            result.specials[prop.id] = new();
                        }
                        result.specials[prop.id].Add(go);

                        go.GetComponent<SpecialProperties>()?.SetProperties(prop.properties);
                    }
                    else
                    {
                        go = GameObject.Instantiate(SettingsSO.propTemplate, root.transform);
                        // Debug.Log($"Set [{i}]: {prop.id}");
                        ModelCreator.BuildProp(
                                modelsCreateInfo[prop.id].vox_unit_datas,
                                go,
                                SettingsSO.paletteTexture2D,
                                SettingsSO.paletteBase,
                                SettingsSO.voxelPrefab);

                        cache[prop.id] = go;
                    }
                }
                go.name = prop.id;
                go.SetActive(true);
                result.props[i] = go;
                go.transform.position = new Vector3(prop.X, prop.Y, prop.Z);
                go.transform.localScale = Vector3.one * (float)prop.scale;
                go.transform.rotation = Quaternion.Euler(prop.RotX, prop.RotY, prop.RotZ);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }

        if (SettingsSO.enableStaticBatching)
        {
            StaticBatchingUtility.Combine(root);
        }

        sw.Stop();
        Debug.Log("SpawnProps ms elapsed: " + sw.Elapsed.TotalMilliseconds);
        return result;
    }

    public static void SpawnProp(string id, Action<GameObject> onFinished)
    {
        Debug.Log("SpawnProp... + id");
        id = id.TrimEnd('\r', '\n');
        var ids = new string[] { id };
        var data = new PostPropsModelsData(ids);

        var req = new HTTPRequest(new Uri(GET_MODELS_URL), HTTPMethods.Post, cb);
        string requestDataJson = JsonConvert.SerializeObject(data);

        req.RawData = Encoding.UTF8.GetBytes(requestDataJson);
        AddMegamodHeaders(req);
        Debug.Log(requestDataJson);
        Debug.Log("Sending request to get model " + id);
        req.Send();

        void cb(HTTPRequest request, HTTPResponse response)
        {
            if (response.IsSuccess)
            {
                Debug.Log(response.DataAsText);
                var go = SpawnPropWithModelData(response.DataAsText);
                onFinished?.Invoke(go);
            }
            else
            {
                Debug.LogWarning("Failed to get prop model:");
                Debug.Log(response.DataAsText);
                onFinished?.Invoke(null);
            }
        }
    }

    public static GameObject SpawnPropWithModelData(string dataJson)
    {
        var models = new LoadPropsModelsDataResponse() { vox_model_data = JsonConvert.DeserializeObject<UserVoxModelDataShort[]>(dataJson) };
        Debug.Log("Models downloaded: " + models.vox_model_data.Length);
        if (models.vox_model_data.Length > 0)
        {
            var go = GameObject.Instantiate(SettingsSO.propTemplate);
            go.name = models.vox_model_data[0].modelId;
            ModelCreator.BuildProp(models.vox_model_data[0].vox_model_data.vox_unit_datas, go, SettingsSO.paletteTexture2D, SettingsSO.paletteBase, SettingsSO.voxelPrefab);
            go.SetActive(true);
            return go;
        }
        else
        {
            Debug.LogWarning("Got zero models.");
            return null;
        }
    }
}
