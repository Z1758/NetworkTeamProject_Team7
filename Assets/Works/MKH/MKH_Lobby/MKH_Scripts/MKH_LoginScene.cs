using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LoginScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Server, List }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_ServerPanel serverPanel;
    [SerializeField] MKH_ServerListPanel listPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LoadLevel("MKH_WaitingScene");
        }
        else if (PhotonNetwork.InLobby)
        {
            SetActivePanel(Panel.List);
        }
        else if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Server);
        }
        else
        {
            SetActivePanel(Panel.Login);
        }
    }

    public override void OnConnectedToMaster()      // 마스터 서버에 접속을 완료 했을 때
    {
        Debug.Log("접속에 성공했다!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Server);
    }

    public override void OnDisconnected(DisconnectCause cause)      // 접속을 끊었을 시
    {
        Debug.Log($"접속이 끊켰다. cause : {cause}");
        SetActivePanel(Panel.Login);
    }

    public override void OnCreatedRoom()        // 방 생성 성공 시
    {
        Debug.Log("서버 생성 성공");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("서버 입장 성공");
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // 방 입장 실패 시
    {
        Debug.Log($"서버 입장 실패, 사유 : {message}");
        SetActivePanel(Panel.Server);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)       // 랜덤 방 입장 실패 시
    {
        Debug.Log($"랜덤 매칭 실패, 사유 : {message}");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
        SetActivePanel(Panel.List);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장 성공");
        listPanel.ClearServerEntries();          // 딕셔너리 방 목록 삭제
        SetActivePanel(Panel.Server);
    }

    public override void OnRoomListUpdate(List<RoomInfo> serverList)
    {
        // 방의 목록이 변경이 있는 경우 서버에서 보내는 정보들
        // 주의 사항
        // 1. 처음 로비 입장 시 : 모든 방 목록을 전달
        // 2. 입장 중 방 목록이 변경되는 경우 : 변경된 방 목록만 전달
        listPanel.UpdateServerList(serverList);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        serverPanel.gameObject.SetActive(panel == Panel.Server);
        listPanel.gameObject.SetActive(panel == Panel.List);
    }
}
