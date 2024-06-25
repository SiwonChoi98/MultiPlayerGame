using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Test : MonoBehaviour
{

    void Start()
    {
        test();
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public async void test()
    {
        await task();
    }

    async UniTask task()
    {
        Debug.Log("unitask 시작");
        await UniTask.Delay(3000);
        Debug.Log("3초후 실행 1");
        await UniTask.Delay(3000);
        Debug.Log("3초후 실행 2");
        await UniTask.Delay(3000);
        Debug.Log("3초후 실행 3");
        await UniTask.Delay(3000);
        Debug.Log("마무리");

    }
    
}
