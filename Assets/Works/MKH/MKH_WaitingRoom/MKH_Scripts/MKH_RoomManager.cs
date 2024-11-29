using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
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

        if (PhotonNetwork.LocalPlayer == PhotonNetwork.MasterClient)
        {
            PlayerSpawn();
        }
        else if(PhotonNetwork.LocalPlayer != PhotonNetwork.MasterClient)
        {
            return;
        }

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
        if (PhotonNetwork.LocalPlayer != null)
        {
            PlayerSpawn();
            Debug.Log("1");
        }
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
        if (PhotonNetwork.LocalPlayer == newPlayer)
        {
            PlayerSpawn();
            Debug.Log("1");
        }
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

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player", randomPos, Quaternion.identity);
    }
}
