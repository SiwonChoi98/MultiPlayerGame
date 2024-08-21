using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

// 전체 사운드만 사용 (개인은 캐릭터)
public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;
    protected override void Awake()
    {
        base.Awake();
        
        IsDonDestroy = true;
    }
    //LOBBY, GAMEROOM, INGAME BGM
    public void PlayBGM(AudioType audioType, float volume, bool isLoop)
    {
        if (_bgmAudioSource == null)
            return;
        
        _bgmAudioSource.volume = volume;
        _bgmAudioSource.loop = isLoop;

        AudioClip audioClip = DataManager.Instance.GameAudioDic[audioType];
        _bgmAudioSource.clip = audioClip;
        
        _bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    public void PauseBGM()
    {
        _bgmAudioSource.Pause();
    }
    //BUTTON?
    public void PlaySFX(AudioType audioType, float volume, bool isLoop = false)
    {
        if (_sfxAudioSource == null)
            return;
        
        _sfxAudioSource.volume = volume;
        _sfxAudioSource.loop = isLoop;
        
        AudioClip audioClip = DataManager.Instance.GameAudioDic[audioType];
        
        _sfxAudioSource.PlayOneShot(audioClip);
    }
}
