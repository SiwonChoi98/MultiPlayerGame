using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class MoveComponent : NetworkBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private AudioComponent _audioComponent;
    
    public List<AudioClip> _moveAudioClipList = new ();
    private int _moveClipIndex = 0;
        
    [SerializeField] private bool _isMoveable = true;
    private float stepSoundSensitivity = 1.0f;
    private float moveAccumForStepSound = 0.0f;
    
    
    [SyncVar]
    [SerializeField] private float _speed;
    
    //private float _resurrectionRadius = 20.0f;
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioComponent = GetComponent<AudioComponent>();
    }
    

    [Server]
    private void Server_InitSpeed(float speed)
    {
        if (isServer)
        {
            _speed = speed;
        }
    }
    
    //User
    public void Move(Vector2 dir)
    {
        if (_isMoveable)
        {
            Vector2 moveDelta = dir * (_speed * Time.fixedDeltaTime);
            Vector2 newPosition = _rigidbody2D.position + moveDelta;
            _rigidbody2D.MovePosition(newPosition);

            PlayMoveSound(moveDelta);
        }
    }

    private void PlayMoveSound(Vector2 moveDelta)
    {
        moveAccumForStepSound += moveDelta.magnitude;
        
        if (moveAccumForStepSound >= stepSoundSensitivity)
        {
            int soundNum = (int)(moveAccumForStepSound / stepSoundSensitivity);
            moveAccumForStepSound -= soundNum * stepSoundSensitivity;

            // 발자국 소리를 soundNum 번 재생
            for (int i = 0; i < soundNum; i++)
            {
                if (isLocalPlayer)
                {
                    PlayMoveSound(_moveClipIndex);
                }
                
                if (!isServer)
                {
                    CmdPlayMoveSound(_moveClipIndex);
                }
                else if (isServer)
                {
                    RpcPlayMoveSound(_moveClipIndex);
                }
                
                _moveClipIndex++;

                if (_moveAudioClipList.Count-1 < _moveClipIndex)
                    _moveClipIndex = 0;
            }
        }
    }

    [Command]
    private void CmdPlayMoveSound(int clipIndex)
    {
        RpcPlayMoveSound(clipIndex);
    }
    
    [ClientRpc]
    private void RpcPlayMoveSound(int clipIndex)
    {
        if (isOwned)
            return;
        PlayMoveSound(clipIndex);
    }

    private void PlayMoveSound(int clipIndex)
    {
        _audioComponent.PlayAudioOneShot(_moveAudioClipList[clipIndex]);
    }
    [Server]
    public void Server_GetResurrectionPosition()
    {
        if (!isServer)
            return;
        
        Vector3 newPosition = BattleManager.Instance.GetSpawnRandomPosition(SpawnPostionType.PLAYER, netId);
        transform.position = newPosition;

        // 클라이언트에 위치 변경을 알림
        RpcUpdatePosition(newPosition);
    }

    [ClientRpc]
    private void RpcUpdatePosition(Vector3 newPosition)
    {
        if (isServer) return;

        if (!isLocalPlayer) return;
        
        transform.position = newPosition;
    }
    
}
