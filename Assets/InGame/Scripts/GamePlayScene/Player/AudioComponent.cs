using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class AudioComponent : NetworkBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioClip _currentAudioClip; 
    
    [SerializeField] private bool isPerspective; //원근감 결정 bool

    private void Start()
    {
        InitSetting(isPerspective);
    }

    private void InitSetting(bool isPerspective)
    {
        if (isPerspective)
        {
            // Spatial Blend를 3D로 설정
            _audioSource.spatialBlend = 1.0f;

            // 볼륨 롤오프를 로그로 설정
            _audioSource.rolloffMode = AudioRolloffMode.Logarithmic;

            // Min 및 Max Distance 설정
            _audioSource.minDistance = 1.0f;
            _audioSource.maxDistance = 50.0f;

            // Spatialize 활성화
            _audioSource.spatialize = true;

            return;
        }
        
        // Spatial Blend를 2D로 설정
        _audioSource.spatialBlend = 0.0f;

        // 볼륨 롤오프를 기본 값으로 설정 (비활성화)
        _audioSource.rolloffMode = AudioRolloffMode.Logarithmic; // 롤오프 모드는 2D 오디오에 영향을 주지 않습니다.

        // Min 및 Max Distance 기본값 설정 (2D 오디오에 영향 없음)
        _audioSource.minDistance = 1.0f; // 기본값
        _audioSource.maxDistance = 500.0f; // 기본값

        // Spatialize 비활성화
        _audioSource.spatialize = false;
        
    }

    public void PlayAudioOneShot(AudioClip audioClip = null)
    {
        if (audioClip == null)
            return;
        
        _currentAudioClip = audioClip;
        _audioSource.PlayOneShot(_currentAudioClip);
    }
    
}
