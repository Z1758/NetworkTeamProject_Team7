using Firebase.Auth;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MKH_ServerPanel : MonoBehaviour
{
    [SerializeField] GameObject serverPanel;
    [SerializeField] GameObject createServerPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()         // 처음부터 방만들기 패널 만들기 방지
    {
        createServerPanel.SetActive(false);
    }

    public void CreateServerMenu()        // 방 만들기 기초 설정
    {
        createServerPanel.SetActive(true);

        //roomNameInputField.text = " ";          // 방 이름 랜덤
        //maxPlayerInputField.text = " ";                                         // 플레이어 수
    }

    public void CreateServerConfirm()     // 방 만들기
    {
        string serverName = roomNameInputField.text;
        if (serverName == "")      // 방 이름이 없을 시
        {
            Debug.LogWarning("방 이름을 지정해야 방을 생성할 수 있습니다.");
            return;
        }

        int maxPlayer = int.Parse(maxPlayerInputField.text);        // 최대 인원수 제한
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 20);                   // 플레이어 수 범위

        RoomOptions options = new RoomOptions();                    // 방 만들기에 대한 옵션 설정
        options.MaxPlayers = maxPlayer;                             // 최대 플레이어 수

        PhotonNetwork.CreateRoom(serverName, options);                // 방만들기(방이름, 옵션)
    }

    public void CreateServerCancel()      // 방 만들기 취소
    {
        createServerPanel.SetActive(false);
    }

    public void RandomMatching()        // 랜덤 매칭
    {
        Debug.Log("랜덤 매칭 요청");

        // 비어 있는 방이 없으면 새로 방을 만들어서 들어가는 방식
        string name = $"{Random.Range(1, 100)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void JoinLobby()             // 로비 들어가기
    {
        Debug.Log("서버 로비 입장 요청");
        PhotonNetwork.JoinLobby();
    }

    public void Logout()                // 나가기
    {
        Debug.Log("로그아웃 요청");
        PhotonNetwork.Disconnect();
    }
}
