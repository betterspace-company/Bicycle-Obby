using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SpecialProperties : MonoBehaviour
{
    public enum SpecialType
    {
        None,
        JumpPad,
    }

    public string propertiesJson;
    public SpecialType type;

    public JumpPadProperties jumpPadProperties;

    public void SetProperties(string json)
    {
        propertiesJson = json;

        switch (type)
        {
            case SpecialType.JumpPad:
                jumpPadProperties = JsonConvert.DeserializeObject<JumpPadProperties>(json);
                break;

            default:
                break;
        }
    }


    [System.Serializable]
    public struct JumpPadProperties
    {
        public float jumpStren;
    }
}

