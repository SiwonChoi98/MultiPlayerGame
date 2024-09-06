using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Org.BouncyCastle.Asn1.X509;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Time = UnityEngine.Time;

public class BattleManager : NetworkBehaviour
{
    public static BattleManager Instance = null;
    
    [Header("설정----------------")]
    [SerializeField] private bool _isRandomPosition = false; //GetSpawnRandomPosition 여부
    [SerializeField] private bool _isItemSpawn = false; //아이템 스폰 여부
    [SerializeField] private bool _isUsedBullet = false; //총알 소모 여부 
    public bool IsUsedbullet { get => _isUsedBullet; } 

    [Header("시간----------------")]
    [SyncVar]
    [SerializeField] private double _playTime;
    [SerializeField] public double InitialTime = 60.0;
    public Action<bool> UpdateEnd;
    
    [SyncVar(hook = nameof(OnEndChanged))]
    public bool IsEnd = false;
    public double PlayTime => _playTime;
    [Header("맵 아이템 최대 갯수-----------▼▼다합쳐서 맵에 스폰지역 갯수 보다 아래여야함!!▼▼")] 
    [SerializeField] private int _addScoreItemMaxCount;
    [SerializeField] private int _healthItemMaxCount;
    [SerializeField] private int _aiMaxCount;
    [SerializeField] private int _weaponMaxCount;
    
    [Header("소환----------------")]
    public SpawnItems SpwanItem;
    public BasePoolObject AI;

    private bool _isSpawned = false;
    private float _nextSpawnTime = 0f;
    [SerializeField] private float _spawnInterval; 
    
    //itemOnly
    private int _itemSpawnPosIndex;
    
    [Header("관리----------------")] 
    public readonly SyncList<uint> ManagedPlayers = new SyncList<uint>();

    public readonly SyncDictionary<uint, int> ManagedPlayerScoreDictionary = new SyncDictionary<uint, int>();
    
    [SerializeField] private List<BasePoolObject> _managedItems = new List<BasePoolObject>();

    public readonly SyncList<PosValueTuple> SpawnPlayerPosList = new SyncList<PosValueTuple>();
    [SerializeField] private GameObject _spawnPlayerPosGroup;
    
    public readonly SyncList<PosValueTuple> SpawnItemPosList = new SyncList<PosValueTuple>();
    [SerializeField] private GameObject _spawnItemPosGroup;
    
    #region UnityMethod

    private void Awake()
    {
        Init_BattleManager();
    }

    public override void OnStartServer()
    {
        Server_SetRule();
        Server_SetSpawnPos();

        //ClearPossiblePosition(_spawnPlayerPosList);
    }
    
    private void FixedUpdate()
    {
        if (IsEnd)
            return;
        
        if (!isServer)
            return;
        
        Server_InputSpawn(); //나중에 지울 코드
        
        
        Server_SpawnTimeCalculate();
        Server_UpdatePlayTime();
        
        if (_isItemSpawn)
        {
            Server_SpawnRandomItem();
        }
    }
    

    #endregion

    private void Init_BattleManager()
    {
        if (Instance == null) 
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (Instance != this)
                Destroy(this.gameObject); 
        }
    }

    
    private void OnEndChanged(bool oldValue, bool newValue)
    {
        UpdateEnd.Invoke(newValue);
    }
    [Server]
    private void Server_SetRule()
    {
        _nextSpawnTime = Time.time + _spawnInterval;
        _playTime = InitialTime;
        IsEnd = false;
        Time.timeScale = 1f;
    }

    [Server]
    private void Server_UpdatePlayTime()
    {
        if (_playTime > 0)
        {
            _playTime -= Time.fixedDeltaTime;
            if (_playTime < 0)
            {
                IsEnd = true;
                Time.timeScale = 0f;
                _playTime = 0;
            }
        }
    }

    [Server]
    private void Server_SetSpawnPos()
    {
        Transform[] _spawnPlayerPosTransforms = _spawnPlayerPosGroup.GetComponentsInChildren<Transform>();
        Transform[] _spawnItemPosTransforms = _spawnItemPosGroup.GetComponentsInChildren<Transform>();

        //부모 transform 제외
        for (int i = 1; i < _spawnPlayerPosTransforms.Length; i++)
        {
            SpawnPlayerPosList.Add(new PosValueTuple(_spawnPlayerPosTransforms[i].position, true));
        }

        for (int i = 1; i < _spawnItemPosTransforms.Length; i++)
        {
            SpawnItemPosList.Add(new PosValueTuple(_spawnItemPosTransforms[i].position, true));
        }
    }

    [Server]
    private void Server_InputSpawn()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Server_SpawnAI(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Server_SpawnGunItem(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Server_SpawnHealthItem(transform.position);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            Server_SpawnAddScoreItem(transform.position);
        }
    }
    
    //test
    [Server]
    private void Server_SpawnRandomItem()
    {
        if (!_isSpawned)
            return;

        if (_managedItems.Count >= _addScoreItemMaxCount + _healthItemMaxCount + _aiMaxCount + _weaponMaxCount)
            return;
            
        //maxCount = caseCount + 1;
        int index = GetRandomIndex(4);
        Vector3 pos = GetSpawnRandomPosition(SpawnPostionType.ITEM, 0);

        Debug.Log($"[Server_SpawnRandomItem] 아이템 스폰 위치: {pos}, 인덱스: {_itemSpawnPosIndex}");

        switch (index)
        {
            case 0:
                if (!Server_ItemSpawnCountCheck(_healthItemMaxCount, PoolObjectType.HEALTH_ITEM))
                    return;
                Server_SpawnHealthItem(pos);
                
                break;
            case 1:
                if (!Server_ItemSpawnCountCheck(_addScoreItemMaxCount, PoolObjectType.ADDSCORE_ITEM))
                    return;
                Server_SpawnAddScoreItem(pos);
                
                break;
            case 2:
                if (!Server_ItemSpawnCountCheck(_weaponMaxCount, PoolObjectType.WINCHESTER_ITEM) ||
                    !Server_ItemSpawnCountCheck(_weaponMaxCount, PoolObjectType.SEMIAUTO_ITEM) ||
                    !Server_ItemSpawnCountCheck(_weaponMaxCount, PoolObjectType.SHOTGUN_ITEM))
                    return;

                Server_SpawnGunItem(pos);
                
                break;
            case 3:
                if (!Server_ItemSpawnCountCheck(_aiMaxCount, PoolObjectType.AI))
                    return;
                Server_SpawnAI(pos);
                
                break;
            default:
                break;
        }
        
        _isSpawned = false;
        
    }

    [Server]
    private bool Server_ItemSpawnCountCheck(int count, PoolObjectType poolObjectType)
    {
        int itemCount = 0;
        foreach (var managedItem in _managedItems)
        {
            if (poolObjectType == managedItem.Server_GetObjectType())
            {
                itemCount++;
                
                if (count <= itemCount)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    [Server]
    private void Server_SpawnTimeCalculate()
    {
        if (_isSpawned)
            return;

        // 현재 시간이 다음 스폰 시간보다 크면 스폰
        if (Time.time >= _nextSpawnTime)
        {
            _isSpawned = true;
            _nextSpawnTime = Time.time + _spawnInterval; // 다음 스폰 시간 설정
            Debug.Log("[Server_SpawnTimeCalculate] 스폰 타이머 리셋, _isSpawned = true");
        }

    }
    [Server]
    private void Server_SpawnHealthItem(Vector3 pos)
    {
        BasePoolObject healthItem = SpwanItem.Server_SpawnHealthItem(pos);
        healthItem.Server_SetSpawnPosIndex(_itemSpawnPosIndex);
        Server_AddManagedItem(healthItem);
    }

    [Server]
    private void Server_SpawnGunItem(Vector3 pos)
    {
        BasePoolObject weaponItem = SpwanItem.Server_SpawnGunItem(GetRandomIndex(SpwanItem.WeaponItemList.Count), pos);
        weaponItem.Server_SetSpawnPosIndex(_itemSpawnPosIndex);
        Server_AddManagedItem(weaponItem);
    }
    
    [Server]
    private void Server_SpawnAI(Vector3 pos)
    {
        BasePoolObject AIObject = PoolManager.Instance.SpawnFromPool(PoolObjectType.AI, AI, pos, Quaternion.identity);
        if (AIObject != null)
        {
            NetworkServer.Spawn(AIObject.gameObject);
            AIObject.InitObjectType(PoolObjectType.AI);
            AIObject.GetComponent<StatusComponent>().Server_SetHealth(100,100);
            
            AIObject.Server_SetSpawnPosIndex(_itemSpawnPosIndex);
            Server_AddManagedItem(AIObject);
        }
    }

    [Server]
    private void Server_SpawnAddScoreItem(Vector3 pos)
    {
        BasePoolObject addScoreItem = SpwanItem.Server_SpawnAddScoreItem(pos);
        addScoreItem.Server_SetSpawnPosIndex(_itemSpawnPosIndex);
        Server_AddManagedItem(addScoreItem);
    }
    
    private Vector3 GetPossiblePosition(SyncList<PosValueTuple> positionList, SpawnPostionType spawnPostionType, uint netId)
    {
        
        //spawn이 true인 지역만 이동 가능하게
        int newIndex = 0;
        while (true)
        {
            int randomIndex = Random.Range(0, positionList.Count);
            if (positionList[randomIndex].Item2)
            {
                newIndex = randomIndex;
                
                if (isServer)
                {
                    //유저 또는 아이템에게 생성 지역을 index를 update (true) 해준다. //playerOnly //item은 remove될 때 Set
                    if (spawnPostionType == SpawnPostionType.PLAYER)
                    {
                        int prevIndex = Server_GetUserSpawnPosIndex(netId);
                        //첫 시작은 -1
                        if (prevIndex != -1)
                        {
                            Server_SetTuple(positionList, prevIndex, true);
                        }
                        
                    }
                    
                    //생성 위치 update (false) 해준다. (해당 지역에서 다시 스폰 안되게) //player, item
                    Server_SetTuple(positionList, newIndex, false);

                    if (spawnPostionType == SpawnPostionType.PLAYER)
                    {
                        //생성 지역 index를 유저 또는 아이템 에게 전달한다. //player
                        Server_UpdateUserSpawnPosIndex(newIndex, netId);
                    }
                    else if(spawnPostionType == SpawnPostionType.ITEM)
                    {
                        _itemSpawnPosIndex = newIndex;
                        Debug.Log($"[GetPossiblePosition] ITEM 스폰 인덱스 설정: {_itemSpawnPosIndex}");
                    }
                }
                break;
            }
        }
        return positionList[newIndex].Item1;
        
    }

    
    [Server]
    public int Server_GetUserSpawnPosIndex(uint netId)
    {
        //playerOnly
        int ret = 0;
        NetworkIdentity targetObject;
        
        if (NetworkServer.spawned.TryGetValue(netId, out targetObject))
        {
            ret = targetObject.GetComponent<InGameUserInfo>().Server_GetUserSpawnPosIndex();
        }
        
        return ret;
    }

    [Server]
    private void Server_UpdateUserSpawnPosIndex(int updateIndex, uint netId)
    {
        NetworkIdentity targetObject;
        if (NetworkServer.spawned.TryGetValue(netId, out targetObject))
        {
            targetObject.GetComponent<InGameUserInfo>().Server_SetUserSpawnPosIndex(updateIndex);
        }
    }
    
    [Server]
    public PosValueTuple Server_UpdateTuple(Vector3 vector3, bool isBool)
    {
        PosValueTuple updatedTuple = new PosValueTuple();
        updatedTuple.Item1 = vector3;
        updatedTuple.Item2 = isBool;
        
        return updatedTuple;
    }

    [Server]
    public void Server_SetTuple(SyncList<PosValueTuple> positionList, int setIndex, bool isPossible)
    {
        PosValueTuple updateTuple = Server_UpdateTuple(positionList[setIndex].Item1, isPossible);

        positionList[setIndex] = updateTuple;
        
        Debug.Log($"[Server_SetTuple] 인덱스 설정: {setIndex}, 가능 여부: {isPossible}");
        
        if (positionList.Count != 11) //11개가 플레이어 스폰지역 갯수
        {
            Debug.Log("-----------------------------");
            for (int i = 0; i < positionList.Count; i++)
            {
                Debug.Log("itemPosCheck : " + i + " "+ positionList[i].Item1 + " , " + positionList[i].Item2);
            }
        }
    }
    
    private void ClearPossiblePosition(SyncList<PosValueTuple> positionList)
    {
        for(int i=0; i<positionList.Count; i++){
            positionList[i] = Server_UpdateTuple(positionList[i].Item1, true); 
        } 
        Debug.Log("Clear Tuple");
    }
    
    [Server]
    public void Server_AddManagedPlayer(uint netId)
    {
        ManagedPlayers.Add(netId);
    }

    [Server]
    public void Server_AddManagedItem(BasePoolObject basePoolObject)
    {
        _managedItems.Add(basePoolObject);
    }

    [Server]
    public void Server_RemoveManagedItem(BasePoolObject basePoolObject)
    {
        _managedItems.Remove(basePoolObject);
    }

    [Server]
    public void Server_AddManagedPlayerScore(uint netId)
    {
        ManagedPlayerScoreDictionary.Add(netId, 0);
    }

    [Server]
    public void Server_UpdateManagedPlayerScore(uint netId, int newValue)
    {
        ManagedPlayerScoreDictionary[netId] += newValue;
    }
    
    public int GetManagedPlayerScore(uint netId)
    {
        return ManagedPlayerScoreDictionary[netId];
    }
    
    [Server]
    public NetworkIdentity Server_GetTopScorePlayer()
    {
        uint topNetid = UInt32.MinValue;
        int maxScore = Int32.MinValue;
        
        foreach (var playerNetId in ManagedPlayers)
        {
            //0점일 때 고려해서 >= 허용 단! 늦게 찾은 플레이어 점수가 덮어씌워짐
            if (GetManagedPlayerScore(playerNetId) >= maxScore)
            {
                topNetid = playerNetId;
                maxScore = GetManagedPlayerScore(playerNetId);
            }
        }
        
        return Server_FindPlayer(topNetid);
    }
    
    public int GetRandomIndex(int maxCount)
    {
        int index =  Random.Range(0, maxCount);
        return index;
    }
    
    public Vector3 GetRandomNavMeshPosition(float radius)
    {
        // 현재 위치에서 반경 내의 랜덤 위치를 계산합니다.
        Vector3 randomDirection = transform.position + Random.insideUnitSphere * radius;

        // NavMesh 상의 유효한 위치인지 확인합니다.
        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, radius, NavMesh.AllAreas))
        {
            return navHit.position;
        }
        else
        {
            // 유효하지 않으면 재귀적으로 다시 시도합니다.
            return GetRandomNavMeshPosition(radius);
        }
    }

    
    public Vector3 GetSpawnRandomPosition(SpawnPostionType spawnPostionType, uint netId)
    {
        //netId = PlayerOnly
        
        //you want world position 0,0 -> Battle Manager - inspector is non check
        if (!_isRandomPosition)
            return transform.position;
        
        Vector3 pos = new Vector3();
        switch (spawnPostionType)
        {
            case SpawnPostionType.PLAYER:
                pos = GetPossiblePosition(SpawnPlayerPosList, SpawnPostionType.PLAYER, netId);
                break;
            case SpawnPostionType.ITEM:
                //pos = GetPossiblePosition(SpawnItemPosList, SpawnPostionType.ITEM, netId);
                pos = GetRandomNavMeshPosition(20f);
                break;
        }

        return pos;
    }
    
    [Server]
    public void Server_AIDead(GameObject gameObject)
    {
        BasePoolObject aiPoolObject = gameObject.GetComponent<AIPoolObject>();
        
        PoolManager.Instance.ReturnToPool(aiPoolObject.Server_GetObjectType(), aiPoolObject);
        NetworkServer.UnSpawn(aiPoolObject.gameObject);
        
        Server_RemoveManagedItem(aiPoolObject); 
    }

    [Server]
    public void Server_PlayerDead(uint netId)
    {
        NetworkIdentity networkIdentity = Server_FindPlayer(netId);
        if (networkIdentity)
        {
            StartCoroutine(Server_Resurrection(networkIdentity.gameObject));
        }
    }

    [Server]
    public NetworkIdentity Server_FindPlayer(uint netId)
    {
        NetworkIdentity targetObject;
        if (ManagedPlayers.Contains(netId))
        {
            if (NetworkServer.spawned.TryGetValue(netId, out targetObject))
            {
                return targetObject;
            }
        }
        return null;
    }
    
    [Server]
    private IEnumerator Server_Resurrection(GameObject targetObject)
    {
        yield return GameSettings.WInGameDeadTime;
        targetObject.GetComponent<StatusComponent>().Server_Resurrection();
    }
    
}
