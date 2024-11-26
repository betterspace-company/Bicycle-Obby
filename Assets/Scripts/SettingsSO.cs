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

    [Tag]
    public string _speedBoosterTag;
    public static string speedBoosterTag => GetInstance()._speedBoosterTag;

    [Tag]
    public string _jumpPadTag;
    public static string jumpPadTag => GetInstance()._jumpPadTag;

    [Tag]
    public string _keyTag;
    public static string keyTag => GetInstance()._keyTag;

    [Tag]
    public string _doorTag;
    public static string doorTag => GetInstance()._doorTag;

    [Tag]
    public string _primaryPortalId;
    public static string primaryPortalId => GetInstance()._primaryPortalId;

    [Tag]
    public string _secondPortalId;
    public static string secondPortalId => GetInstance()._secondPortalId;

    [Tag]
    public string _coinTag;
    public static string coinTag => GetInstance()._coinTag;

    [Tag]
    public string _finishTag;
    public static string finishTag => GetInstance()._finishTag;

    [Tag]
    public string _lethalTag;
    public static string lethalTag => GetInstance()._lethalTag;

    [Tag]
    public string _checkpointTag;
    public static string checkpointTag => GetInstance()._checkpointTag;
    
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
