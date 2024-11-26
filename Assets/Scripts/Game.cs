using System;
using System.Collections.Generic;
using ServerStructs;
using UnityEngine;
using UnityEngine.Serialization;

public class Game : MonoBehaviour
{
    public CameraController mainCamera;
    public PhysicsController playerPrefab;
    public PhysicsController actualPlayer;
    public Server.SpawnSaveInfo saveInfo;
    public SaveSettings saveSettings;
    public List<Checkpoint> checkpoints = new();
    public GameObject finish;
    public Portal portal0;
    public Portal portal1;
    public Vector3 spawnPosition;
    
    private void Start()
    {
        OnGotNextGame("GAME-BC71EE3BD00437E2");
    }
    
    public void OnGotNextGame(string gameId)
    {
        string saveId = gameId.Replace("GAME-", "SAVE-");
        Server.SpawnSave(saveId, OnSpawnSave);
    }
    
    public void OnSpawnSave(Server.SpawnSaveInfo info)
    {
        // levelAuthorText.gameObject.SetActive(!string.IsNullOrEmpty(info.ownerName));
        // levelAuthorText.text = "Game by: " + info.ownerName;

        saveInfo = info;
        saveSettings = info.settings;

        checkpoints.Clear();
        foreach (var special in info.specials)
        {
            if (String.Equals(special.Key, SettingsSO.checkpointTag, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var c in special.Value)
                {
                    checkpoints.Add(c.GetComponent<Checkpoint>());
                }
            }
            if (String.Equals(special.Key, SettingsSO.finishTag, StringComparison.OrdinalIgnoreCase))
            {
                finish = special.Value[0];
            }
            if (String.Equals(special.Key, SettingsSO.primaryPortalId, StringComparison.OrdinalIgnoreCase))
            {
                if (special.Value.Count == 1)
                {
                    portal0 = special.Value[0].GetComponent<Portal>();
                    portal0.trigger.tag = SettingsSO.primaryPortalId;
                }
                else
                {
                    Debug.LogWarning($"There are {special.Value.Count} primary portals in save. Look it up!");
                }
            }
            if (String.Equals(special.Key, SettingsSO.secondPortalId, StringComparison.OrdinalIgnoreCase))
            {
                if (special.Value.Count == 1)
                {
                    portal1 = special.Value[0].GetComponent<Portal>();
                    portal1.trigger.tag = SettingsSO.secondPortalId;
                }
                else
                {
                    Debug.LogWarning($"There are {special.Value.Count} secondary portals in save. Look it up!");
                }
            }
        }

        actualPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.Euler(0, 180, 0));
        mainCamera.player = actualPlayer.transform;
    }
}
