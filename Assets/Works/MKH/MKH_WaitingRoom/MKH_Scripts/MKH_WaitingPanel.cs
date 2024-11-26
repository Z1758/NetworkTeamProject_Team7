using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class MKH_WaitingPanel : MonoBehaviour
{
    [SerializeField] MKH_WaitingPlayerEntry[] playerEntries;

    // 방에 들어왔을 때
    private void OnEnable()
    {
        UpdatePlayers();

        // 플레이어 넘버링 업데이트
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

    }

    // 방에 나갔을 때
    private void OnDisable()
    {
        // 플레이어 넘버링 업데이트 안하기
        PlayerNumbering.OnPlayerNumberingChanged -= UpdatePlayers;
    }

    public void UpdatePlayers()
    {
        // 플레이어 들어오기전 공간 셋팅
        foreach (MKH_WaitingPlayerEntry entry in playerEntries)
        {
            entry.SetEmpty();
        }
        // 모든 플레이어 확인
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // 아직 번호 할당 안받았을 때 업데이트 하지 말기
            if (player.GetPlayerNumber() == -1)
                continue;
            // 플레이어 넘버 가지고 오기
            int number = player.GetPlayerNumber();
            // 플레이어 엔트리에서 같은 넘버 플레이어 정보 셋팅
            playerEntries[number].SetPlayer(player);
        }
    }

    // 플레이어 입장
    public void EnterPlayer(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} 입장!");
        UpdatePlayers();
    }

    // 플레이어 퇴장
    public void ExitPlayer(Player otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} 퇴장!");
        UpdatePlayers();
    }

    // 방 퇴장
    public void LeaveRoom()
    {
        PhotonNetwork.LoadLevel("MKH_LobbyScene");
    }
}

