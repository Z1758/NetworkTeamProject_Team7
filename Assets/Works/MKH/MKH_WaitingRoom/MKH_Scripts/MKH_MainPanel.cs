using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MKH_MainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject createRoomPanel;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        // 처음부터 방만들기 패널 만들기 방지
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        PhotonNetwork.LeaveRoom();

        // 방 만들기 기초 설정
        createRoomPanel.SetActive(true);

        // 방 이름 랜덤
        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
        // 플레이어 수
        maxPlayerInputField.text = "1 ~ 4";
    }

    // 방 만들기
    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        // 방 이름이 없을 시
        if (roomName == "")
        {
            Debug.LogWarning("방 이름을 지정해야 방을 생성할 수 있습니다.");
            return;
        }

        int maxPlayer = int.Parse(maxPlayerInputField.text);        // 최대 인원수 제한
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 4);                   // 플레이어 수 범위

        RoomOptions options = new RoomOptions();                    // 방 만들기에 대한 옵션 설정
        options.MaxPlayers = maxPlayer;                             // 최대 플레이어 수

        PhotonNetwork.CreateRoom(roomName, options);                // 방만들기(방이름, 옵션)
    }

    // 방 만들기 취소
    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    // 랜덤 매칭
    public void RandomMatching()
    {
        Debug.Log("랜덤 매칭 요청");
        PhotonNetwork.LeaveRoom();

        // 비어 있는 방이 없으면 새로 방을 만들어서 들어가는 방식
        string name = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 4 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    // 로비 들어가기
    public void JoinLobby()
    {
        Debug.Log("로비 입장 요청");
        PhotonNetwork.JoinLobby();
    }

    public void Server()
    {
        Debug.Log("서버선택지로 이동");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    // 로그아웃
    public void Logout()
    {
        Debug.Log("로그아웃 요청");
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    // 나가기
    public void Exit()
    {
        Application.Quit();
    }

    // 유저 데이터 삭제
    public void DeleteUser()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        user.DeleteAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User deleted successfully.");
                PhotonNetwork.Disconnect();
            });
    }
}
