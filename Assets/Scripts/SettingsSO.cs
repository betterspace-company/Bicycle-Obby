using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SettingsSO", order = 1)]
public class SettingsSO : SerializedScriptableObject
{
    public bool _enableStaticBatching;
    public static bool enableStaticBatching => GetInstance()._enableStaticBatching;
    
    public GameObject _voxelPrefab;
    public static GameObject voxelPrefab => GetInstance()._voxelPrefab;

    public Texture2D _paletteTexture2D;
    public static Texture2D paletteTexture2D => GetInstance()._paletteTexture2D;

    public Material _paletteBase;
    public static Material paletteBase => GetInstance()._paletteBase;

    public GameObject _propTemplate;
    public static GameObject propTemplate => GetInstance()._propTemplate;

    [SerializeField]
    private bool _postBuildCompression;
    public static bool postBuildCompression => GetInstance()._postBuildCompression;

    public Dictionary<string, GameObject> _specialProps = new();
    public static Dictionary<string, GameObject> specialProps => GetInstance()._specialProps;

    private static SettingsSO _instance;
    public static SettingsSO GetInstance()
    {
        if (_instance == null)
        {
            Debug.Log("Access to Helper for the first time. Loading...");
            _instance = Resources.Load(typeof(SettingsSO).Name) as SettingsSO;
        }
        return _instance;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void ResetStatic()
    {
        _instance = null;
    }
}
