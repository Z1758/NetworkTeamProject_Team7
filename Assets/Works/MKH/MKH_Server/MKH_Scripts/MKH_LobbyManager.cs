using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_MainPanel menuPanel;
    [SerializeField] MKH_LobbyPanel lobbyPanel;
    [SerializeField] GameObject cover;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        MKH_LoadingSceneController.Create();

        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Menu);
        }
        else
        {
            SetActivePanel(Panel.Login);
            cover.SetActive(true);
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            cover.SetActive(false);
        }
    }

    public override void OnConnectedToMaster()      // 마스터 서버에 접속을 완료 했을 때
    {
        Debug.Log("접속에 성공했다!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Menu);
    }

    public override void OnDisconnected(DisconnectCause cause)      // 접속을 끊었을 시
    {
        Debug.Log($"접속이 끊켰다. cause : {cause}");
        SetActivePanel(Panel.Login);
    }

    public override void OnCreatedRoom()        // 방 생성 성공 시
    {
        Debug.Log("방 생성 성공");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        MKH_LoadingSceneController.Instance.LoadScene("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // 방 입장 실패 시
    {
        Debug.Log($"방 입장 실패, 사유 : {message}");
        SetActivePanel(Panel.Menu);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // 랜덤 방 입장 실패 시
    {
        Debug.Log($"랜덤 매칭 실패, 사유 : {message}");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장 성공");
        lobbyPanel.ClearRoomEntries();          // 딕셔너리 방 목록 삭제
        SetActivePanel(Panel.Menu);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 방의 목록이 변경이 있는 경우 서버에서 보내는 정보들
        // 주의 사항
        // 1. 처음 로비 입장 시 : 모든 방 목록을 전달
        // 2. 입장 중 방 목록이 변경되는 경우 : 변경된 방 목록만 전달
        lobbyPanel.UpdateRoomList(roomList);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
