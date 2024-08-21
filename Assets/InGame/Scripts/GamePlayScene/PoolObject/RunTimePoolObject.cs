using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine.SceneManagement;

public class RunTimePoolObject : BasePoolObject
{
    [SerializeField] private int _runtime;
    private void OnEnable()
    {
        ReturnObject();
    }
    
    private async void ReturnObject()
    {
        var cancellationToken = this.GetCancellationTokenOnDestroy(); // 오브젝트가 파괴될 때 취소됨
        await WaitReturnObject(_runtime, cancellationToken);
    }

    private async UniTask WaitReturnObject(int runtime, CancellationToken cancellationToken)
    {
        try
        {
            await UniTask.Delay(runtime, cancellationToken: cancellationToken); // 오브젝트가 파괴되면 취소
        }
        catch (OperationCanceledException)
        {
            return; // 오브젝트가 파괴된 경우 처리
        }

        if (this == null || gameObject == null)
            return;
        
        ReturnToPool();
    }

}
