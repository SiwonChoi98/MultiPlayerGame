using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
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
        await WaitReturnObject(_runtime);
    }

    private async UniTask WaitReturnObject(int runtime)
    {
        await UniTask.Delay(runtime); //1초 == 1000

        ReturnToPool();
    }
}
