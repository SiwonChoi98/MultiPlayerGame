using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class RankUI : MonoBehaviour
{
    [SerializeField] private List<UserInfoItem> _userInfoItemList;

    public void SetRankUI(int userCount)
    {
        for (int i = 0; i < userCount; i++)
        {
            GameObject userObject = FindNetworkObject(BattleManager.Instance.ManagedPlayers[i]);
            if(userObject == null)
                continue;
            
            InGameUserInfo info = userObject.GetComponent<InGameUserInfo>();
            
            _userInfoItemList[i].gameObject.SetActive(true);
            _userInfoItemList[i].SetUserInfo(info.UserName, info.UserScore);
        }

        
    }
    
    // 특정 네트워크 ID로 네트워크 오브젝트를 찾는 함수
    public GameObject FindNetworkObject(uint netId)
    {
        // 네트워크 오브젝트를 담을 변수
        NetworkIdentity foundObject;

        // 네트워크 ID로 네트워크 오브젝트 찾기
        if (NetworkClient.spawned.TryGetValue(netId, out foundObject))
        {
            return foundObject.gameObject;
        }

        // 네트워크 오브젝트를 찾지 못했을 경우 null 반환
        return null;
    }
}
