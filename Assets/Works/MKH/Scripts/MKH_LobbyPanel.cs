using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class MKH_LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] MKH_RoomEntry roomEntryPrefab;

    private Dictionary<string, MKH_RoomEntry> roomDictionary = new Dictionary<string, MKH_RoomEntry>();

    /*
    private void OnEnable()     // 들어 갔을 때
    {
        ClearRoomEntries();
    }

    private void OnDisable()    // 나갔을 때
    {
        ClearRoomEntries();
    }
    */

    public void LeaveLobby()
    {
        Debug.Log("로비 퇴장 요청");
        PhotonNetwork.LeaveLobby();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {

            // 방이 사라진 경우 + 방이 비공개인 경우 + 입장이 불가능한 방인 경우
            if (info.RemovedFromList == true || info.IsVisible == false || info.IsOpen == false)
            {
                // 예외 상황 : 로비 들어가자마자 사라지는 방인 경우
                if (roomDictionary.ContainsKey(info.Name) == false)     // room목록에 추가한 적이 없는 방
                    continue;                                           // 다른 방 처리를 위한 continue

                Destroy(roomDictionary[info.Name].gameObject);          // 사라진 방
                roomDictionary.Remove(info.Name);                       // 딕셔너에서 제거
            }

            // 새로운 방이 생성된 경우
            else if (roomDictionary.ContainsKey(info.Name) == false)
            {
                MKH_RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);    // 방 생성
                roomDictionary.Add(info.Name, roomEntry);                           // 생성된 방 딕셔너리로 추가
                roomEntry.SetRoomInfo(info);                                        // TODO : 방 정보 설정
            }

            // 방의 정보가 변경된 경우
            else if (roomDictionary.ContainsKey((string)info.Name) == true)
            {
                MKH_RoomEntry roomEntry = roomDictionary[info.Name];                    // 딕셔너리에 있는 기존 방 변경
                roomEntry.SetRoomInfo(info);                                        // TODO : 방 정보 설정
            }
        }
    }

    public void ClearRoomEntries()
    {
        foreach(string name in roomDictionary.Keys)
        {
            Destroy(roomDictionary[name].gameObject);
        }
        roomDictionary.Clear();
    }
}
