using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class MKH_RoomManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Room }

    [SerializeField] MKH_RoomPanel roomPanel;


    private void Start()
    {
        // 방장과 같은 씬으로 이동
        PhotonNetwork.AutomaticallySyncScene = true;

        StartCoroutine(PlayerSpawns());
    }

    // 접속 종료
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"접속이 끊켰다. cause : {cause}");
    }


    #region 방 (플레이어 업데이트)

    // 플레이어 입장
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.EnterPlayer(newPlayer);
    }

    // 플레이어 업데이트
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
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
        Vector3 randPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player", randPos, Quaternion.identity);
    }

    IEnumerator PlayerSpawns()
    {
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            PlayerSpawn();
            SetActivePanel(Panel.Room);
        }
    }
}
