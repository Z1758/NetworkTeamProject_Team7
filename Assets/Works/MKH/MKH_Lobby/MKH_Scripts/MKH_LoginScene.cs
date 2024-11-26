using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_LoginScene : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Server }

    [SerializeField] MKH_LoginPanel loginPanel;
    [SerializeField] MKH_ServerPanel serverPanel;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            SetActivePanel(Panel.Server);
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

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)         // 방 입장 실패 시
    {
        Debug.Log($"방 입장 실패, 사유 : {message}");
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        serverPanel.gameObject.SetActive(panel == Panel.Server);
    }
}
