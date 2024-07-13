using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance = null; 
    
    public SpawnItems SpwanItem;

    public GameObject AI;
    #region UnityMethod

    private void Awake()
    {
        Init_BattleManager();
    }
    
    private void Update()
    {
        if (!isServer)
            return;

        Server_SpawnHealthItem();
        Server_SpawnGunItem();
        
        Server_SpawnAI();
    }

    #endregion

    private void Init_BattleManager()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject); 
        }
    }

    [Server]
    private void Server_SpawnHealthItem()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SpwanItem.Server_SpawnHealthItem();
        }
    }

    [Server]
    private void Server_SpawnGunItem()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SpwanItem.Server_SpawnGunItem(RandomIndex(SpwanItem.GunItemList.Count));
        }
    }
    
    [Server]
    private void Server_SpawnAI()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameObject gameObject = Instantiate(AI);
            NetworkServer.Spawn(gameObject);
        }
    }
    public int RandomIndex(int maxCount)
    {
        int index =  Random.Range(0, maxCount);
        return index;
    }
    
}
