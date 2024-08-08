using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class GamePlayerController : NetworkBehaviour
{
    private MoveComponent _moveComponent;
    private RotateComponent _rotateComponent;
    private CombatComponent _combatComponent;
    private StatusComponent _statusComponent;
    
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _minimapCamera;
    
    #region UnityMethod

    private void Awake()
    {
        InitComponent();
    }

    public override void OnStartServer() 
    {
        EquipWeapon();
        BattleManager.Instance.Server_AddManagedPlayer(gameObject);
    }

    public override void OnStartClient()
    {
        OnStartSpawnPlayerCamera();
        GetStartPosition();
    }

    private void Update()
    {
        if (!CheckInput())
            return;
        
        Fire();
        InputShowRankUI();
    }

    private void FixedUpdate()
    {
        if (!CheckInput())
            return;
        
        Move();
        Rotate();
        
    }
    

    #endregion
    
    
    
    //컴포넌트 초기화
    private void InitComponent()
    {
        _moveComponent = GetComponent<MoveComponent>();
        _rotateComponent = GetComponent<RotateComponent>();
        _combatComponent = GetComponent<CombatComponent>();
        _statusComponent = GetComponent<StatusComponent>();
    }
    
    
    //무기 장착
    private void EquipWeapon()
    {
        if (_combatComponent == null)
            return;
        
        _combatComponent.EquipWeaponData(GunDataType.WINCHESTER_DATA);
    }
    
    //플레이어 이동
    private void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        Vector2 dir = new Vector2(inputX, inputY);
        
        _moveComponent.Move(dir);
    }

    //총알 발사
    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _combatComponent.Fire();
        }
    }

    //회전
    private void Rotate()
    {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirVec = mouse - (Vector2)transform.position;
        float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
        
        _rotateComponent.RotateTowardsMouse(angle);
    }
    
    //카메라 생성
    private void OnStartSpawnPlayerCamera()
    {
        Camera playerCamera = Instantiate(_playerCamera);
        playerCamera.GetComponent<TargetCamera>().SetTarget(gameObject.transform);

        Camera minimapCamera = Instantiate(_minimapCamera);
        minimapCamera.GetComponent<TargetCamera>().SetTarget(gameObject.transform);
        
        //로컬 플레이어 일때만 활성화
        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
            minimapCamera.gameObject.SetActive(false);
        }
        else
        {
            playerCamera.gameObject.SetActive(true);
            minimapCamera.gameObject.SetActive(true);
        } 
        
    }

    private void InputShowRankUI()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            GamePlayUI.Instance.ShowRankUI();
        }
    }

    private void GetStartPosition()
    {
        transform.position = BattleManager.Instance.GetSpawnRandomPosition(SpawnPostionType.PLAYER, netId);
    }

    private bool CheckInput()
    {
        if (BattleManager.Instance != null)
        {
            if (BattleManager.Instance.IsEnd)
                return false;
        }
            
        if(!isLocalPlayer)
            return false;
        
        if(_statusComponent.IsDead)
            return false;

        return true;
    }
}
