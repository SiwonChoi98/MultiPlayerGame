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
        Debug.Log("unitask ����");
        await UniTask.Delay(3000);
        Debug.Log("3���� ���� 1");
        await UniTask.Delay(3000);
        Debug.Log("3���� ���� 2");
        await UniTask.Delay(3000);
        Debug.Log("3���� ���� 3");
        await UniTask.Delay(3000);
        Debug.Log("������");

    }
    
}
