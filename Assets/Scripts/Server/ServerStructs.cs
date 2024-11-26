using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ServerStructs
{
    [System.Serializable]
    public class ReplaysResponse
    {
        public string[] replays;
    }

    [System.Serializable]
    public struct MegaCubeInfo
    {
        public float p_x;
        public float p_y;
        public float p_z;
        public string cubeId;
        public Prop[] models;
        public string preview_base64;
    }

    [System.Serializable]
    public class Save
    {
        public string serverId;
        public string saveId;
        public Prop[] models;

        public string preview_base64;
        public SaveSettings settings;
        public UserData userData;
    }

    [System.Serializable]
    public struct UserData
    {
        public string owner_name;
    }

    [System.Serializable]
    public struct SaveSettings
    {
        public int propsOnServer;
        public int billboardPreview;
        public float running_speed;
        public float jump_intertion;
        public float jump_force;
        public bool shift_mode_enabled;
        public bool fly_mode_enabled;
        public bool floor_mat_enabled;
        public bool bunnyHop_mode_enabled;
        public bool autoRun_mode_enabled;
        public bool doubleJump_mode_enabled;
        public bool fog_enabled;
        public bool chroma_enabled;
        public bool floor_enabled;
        public bool background_enabled;
        public bool publicServer_enabled;
        public bool adminControl_enabled;
        public bool playersCollision_enabled;
        // public SkyboxSettings skybox;
        public Vector3 spawnPointPosition;
        public float spawnPointYRotation;
    }

    [System.Serializable]
    public class Prop
    {
        public double scale;
        public double x;
        public double y;
        public double z;
        public double rotation_x;
        public double rotation_y;
        public double rotation_z;
        public string properties;
        public string id;

        public float X => (float)x;
        public float Y => (float)y;
        public float Z => (float)z;

        public float RotX => (float)rotation_x;
        public float RotY => (float)rotation_y;
        public float RotZ => (float)rotation_z;

        public Prop()
        {

        }

        public Vector3 Pos => new Vector3(X, Y, Z);
        public Quaternion Rot => Quaternion.Euler(RotX, RotY, RotZ);
        public Vector3 Scale => Vector3.one * (float)scale;


        public Prop(Transform transform)
        {
            scale = Math.Round(transform.lossyScale.x, 3);
            Vector3 position = transform.position;

            x = Math.Round(position.x, 3);
            y = Math.Round(position.y, 3);
            z = Math.Round(position.z, 3);

            Quaternion rotation = transform.rotation;

            rotation_x = Math.Round(rotation.eulerAngles.x, 3);
            rotation_y = Math.Round(rotation.eulerAngles.y, 3);
            rotation_z = Math.Round(rotation.eulerAngles.z, 3);

            id = transform.name.Replace("(Clone)", "");
        }
    }


    [System.Serializable]
    public struct LoadPropsModelsDataResponse
    {
        public UserVoxModelDataShort[] vox_model_data;
    }

    [System.Serializable]
    public struct UserVoxModelDataShort
    {
        public VoxModelData vox_model_data;
        public string preview_base64;
        public string id;
        public string owner_name;

        public string modelId;
    }

    [System.Serializable]
    public struct VoxModelData
    {
        public string body_part;
        public VoxUnitData[] vox_unit_datas;
    }

    [System.Serializable]
    public struct FullModelData
    {
        public string[] parts;
        public string modelId;
        public string preview_base64;
    }

    [System.Serializable]
    public struct SkinsPageData
    {
        public FullModelData[] items;
        public int page;
        public int totalCount;
        public int totalPages;
        public int pageSize;
    }

    [System.Serializable]
    public struct VoxUnitData
    {
        public string color;
        public float x;
        public float y;
        public float z;

        [JsonIgnore]
        public int IntX => (int)x;

        [JsonIgnore]
        public int IntY => (int)y;

        [JsonIgnore]
        public int IntZ => (int)z;
    }


    [System.Serializable]
    public struct PostPropsModelsData
    {
        public PostPropsModelsData(string[] ids)
        {
            propsIds = ids;
        }

        public string[] propsIds;
    }
    public class Message
    {
        public string role;
        public string content;
    }

    public class Request
    {
        public string model;
        public List<Message> messages;
    }

    public class Content
    {
        public string beforeCode;
        public string code;
        public string afterCode;
    }

    public class ResponseData
    {
        public string id;
        public string @object;
        public ulong created;
        public string model;
        public List<Choice> choices;
        public Usage usage;
        public Content content;
    }

    public class Choice
    {
        public int index;
        public Message message;
        public string finish_reason;
    }

    public class Usage
    {
        public int prompt_tokens;
        public int completion_tokens;
        public int total_tokens;
    }

    [System.Serializable]
    public class LikeScriptResponse
    {
        public string updatedAt;
        public List<string> likedScripts;
        public List<string> dislikedScripts;
        public List<string> favoriteScripts;
        public List<string> clickedScripts;
    }

}
