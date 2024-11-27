using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LobbyScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Menu, Lobby, Room, Waiting }

    [SerializeField] MKH_MainPanel mainPanel;
    [SerializeField] MKH_RoomPanel roomPanel;
    [SerializeField] MKH_LobbyPanel lobbyPanel;
    [SerializeField] MKH_WaitingPanel waitingPanel;

    private void Start()
    {
        // 방장과 같은 씬으로 이동
        PhotonNetwork.AutomaticallySyncScene = true;


        SetActivePanel(Panel.Waiting);
        //PlayerSpawn();

    }

    // 서버 접속
    public override void OnConnectedToMaster()
    {
        Debug.Log("접속에 성공했다!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Menu);
    }

    // 접속 종료
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"접속이 끊켰다. cause : {cause}");
        PhotonNetwork.LoadLevel("MKH_LobbyScene");
    }


    #region 방 생성
    // 방 생성 성공
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공");
    }

    // 방 생성 실패
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"방 생성 실패, 사유 : {message}");
        SetActivePanel(Panel.Menu);
    }
    #endregion

    #region 방 (입장, 퇴장, 플레이어 업데이트)
    // 방 입장 성공
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        //SetActivePanel(Panel.Room);
        SetActivePanel(Panel.Menu);
    }

    // 방 입장 실패
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"방 입장 실패, 사유 : {message}");
    }

    // 랜덤 방 입장 실패
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"랜덤 매칭 실패, 사유 : {message}");
    }

    // 방에서 퇴장
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장 성공");
        SetActivePanel(Panel.Menu);
    }

    // 플레이어 입장
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        waitingPanel.EnterPlayer(newPlayer);
        //roomPanel.EnterPlayer(newPlayer);
    }

    // 플레이어 업데이트
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        roomPanel.UpdatePlayerProperty(targetPlayer, changedProps);
    }

    // 플레이어 퇴장
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        waitingPanel.EnterPlayer(otherPlayer);
        //roomPanel.ExitPlayer(otherPlayer);
    }
    #endregion

    #region 로비
    // 로비 입장
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장 성공");
        SetActivePanel(Panel.Lobby);
    }

    // 로비 퇴장
    public override void OnLeftLobby()
    {
        Debug.Log("로비 퇴장 성공");
        // 딕셔너리 방 목록 삭제
        lobbyPanel.ClearRoomEntries();
        SetActivePanel(Panel.Menu);
    }

    // 방 리스트 업데이트
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }
    #endregion

    // 패널 정보
    private void SetActivePanel(Panel panel)
    {
        mainPanel.gameObject.SetActive(panel == Panel.Menu);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
        waitingPanel.gameObject.SetActive(panel == Panel.Waiting);
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player(UI)", randomPos, Quaternion.identity);
    }
}
