using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
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
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioComponent = GetComponentInChildren<AudioComponent>();
    }

    private void Start()
    {
        //Server_InitSpeed(GameSettings.InitPlayerMoveSpeed);
    }

    [Server]
    private void Server_InitSpeed(float speed)
    {
        if (isServer)
        {
            _speed = speed;
        }
    }

    
    //AI
    [Server]
    public void Server_Move(Vector2 dir)
    {
        if (isServer)
        {
            Move(dir);    
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
                _audioComponent.PlayAudioOneShot(_moveAudioClipList[_moveClipIndex]);
                _moveClipIndex++;

                if (_moveAudioClipList.Count-1 < _moveClipIndex)
                    _moveClipIndex = 0;
            }
        }
    }
    
}
