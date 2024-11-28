using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class MKH_RoomManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Room }

    [SerializeField] MKH_RoomPanel roomPanel;

    private void Start()
    {
        // 방장과 같은 씬으로 이동
        PhotonNetwork.AutomaticallySyncScene = true;

        SetActivePanel(Panel.Room);
    }

    // 서버 접속
    public override void OnConnectedToMaster()
    {
        Debug.Log("접속에 성공했다!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        SetActivePanel(Panel.Room);
    }

    // 접속 종료
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"접속이 끊켰다. cause : {cause}");
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }


   

    #region 방 (입장, 퇴장, 플레이어 업데이트)
    // 방 입장 성공
    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 성공");
        SetActivePanel(Panel.Room);
    }

    // 방에서 퇴장
    public override void OnLeftRoom()
    {
        Debug.Log("방 퇴장 성공");
        PhotonNetwork.LoadLevel("MKH_ServerScene");
    }

    // 플레이어 입장
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.EnterPlayer(newPlayer);
    }

    // 플레이어 업데이트
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        roomPanel.UpdatePlayerProperty(targetPlayer, changedProps);
    }

    // 플레이어 퇴장
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.ExitPlayer(otherPlayer);
    }
    #endregion

    // 패널 정보
    private void SetActivePanel(Panel panel)
    {
        roomPanel.gameObject.SetActive(panel == Panel.Room);
    }
}
