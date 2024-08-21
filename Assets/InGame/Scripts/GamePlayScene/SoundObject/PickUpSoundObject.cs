using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PickUpSoundObject : NetworkBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    
    public void PlaySound(AudioClip audioClip)
    {
        if (_audioSource == null)
            return;
        
        _audioSource.PlayOneShot(audioClip);
    }
}
