using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_ServerListPanel : MonoBehaviour
{
    [SerializeField] RectTransform serverContent;
    [SerializeField] MKH_ServerEntry serverEntryPrefab;

    private Dictionary<string, MKH_ServerEntry> serverDictionary = new Dictionary<string, MKH_ServerEntry>();

    public void LeaveLobby()
    {
        Debug.Log("서버 로비 퇴장 요청");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateServerList(List<RoomInfo> serverList)
    {
        foreach (RoomInfo info in serverList)
        {

            // 방이 사라진 경우 + 방이 비공개인 경우 + 입장이 불가능한 방인 경우
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                // 예외 상황 : 로비 들어가자마자 사라지는 방인 경우
                if (serverDictionary.ContainsKey(info.Name) == false)     // room목록에 추가한 적이 없는 방
                    continue;                                           // 다른 방 처리를 위한 continue

                Destroy(serverDictionary[info.Name].gameObject);          // 사라진 방
                serverDictionary.Remove(info.Name);                       // 딕셔너에서 제거
            }

            // 새로운 방이 생성된 경우
            else if (serverDictionary.ContainsKey(info.Name) == false)
            {
                MKH_ServerEntry serverEntry = Instantiate(serverEntryPrefab, serverContent);    // 방 생성
                serverDictionary.Add(info.Name, serverEntry);                           // 생성된 방 딕셔너리로 추가
                serverEntry.SetServerInfo(info);                                        // TODO : 방 정보 설정
            }

            // 방의 정보가 변경된 경우
            else if (serverDictionary.ContainsKey((string)info.Name) == true)
            {
                MKH_ServerEntry roomEntry = serverDictionary[info.Name];                    // 딕셔너리에 있는 기존 방 변경
                roomEntry.SetServerInfo(info);                                        // TODO : 방 정보 설정
            }
        }
    }

    public void ClearServerEntries()
    {
        foreach (string name in serverDictionary.Keys)
        {
            Destroy(serverDictionary[name].gameObject);
        }
        serverDictionary.Clear();
    }
}
