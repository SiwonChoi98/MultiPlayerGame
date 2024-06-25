using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GamePlayerController : NetworkBehaviour
{
    private MoveComponent _moveComponent;
    private RotateComponent _rotateComponent;
    private CombatComponent _combatComponent;
    private StatusComponent _statusComponent;
    
    [SerializeField] private Camera _playerCamera;
    
    //Move Audio
    private bool isMoving = false;
    private float lastMoveTime = 0f;
    private float moveAudioCooldown = 0.3f; // 오디오 재생 간격
    
    #region UnityMethod

    private void Awake()
    {
        InitComponent();
    }

    private void Start()
    {
        EquipWeapon();
        OnStartSpawnPlayerCamera();
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        
        if (_statusComponent.CurrentHealth <= 0)
            return;
        
        Fire();
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        if (_statusComponent.CurrentHealth <= 0)
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
        Weapon weapon = GetComponentInChildren<WeaponInventory>().Weapons[0];    
        _combatComponent.EquipWeapon(weapon);
    }
    
    //플레이어 이동
    private void Move()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        Vector2 dir = new Vector2(inputX, inputY);
        
        _moveComponent.Move(dir);

        //if (dir != Vector2.zero)
        //{
        //    if (!isMoving || Time.time - lastMoveTime > moveAudioCooldown)
        //    {
        //        _audioComponent.PlayAudioOneShot(_moveComponent.MoveAudioClip);
        //        lastMoveTime = Time.time;
        //    }
        //    isMoving = true;
        //}
        //else
        //{
        //    isMoving = false;
        //}
        
    }

    //총알 발사
    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // if can fire ?
            //_combetComponent.CmdFire();
            _combatComponent.Fire();

            // PlayFireAudio
            // SpawnFireVFX
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
        playerCamera.GetComponent<PlayerCamera>().SetTarget(gameObject.transform);
        
        //로컬 플레이어 일때만 활성화
        if (!isLocalPlayer) playerCamera.gameObject.SetActive(false);
        else playerCamera.gameObject.SetActive(true); 
        
    }
    
}
