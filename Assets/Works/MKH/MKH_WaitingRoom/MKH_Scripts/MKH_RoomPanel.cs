using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


public class MKH_RoomPanel : MonoBehaviourPun
{
    [SerializeField] MKH_PlayerEntry[] playerEntries;
    [SerializeField] Button startButton;

    // 방에 들어왔을 때
    private void OnEnable()
    {
        UpdatePlayers();
        // 플레이어 넘버링 업데이트
        PlayerNumbering.OnPlayerNumberingChanged += UpdatePlayers;

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);
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
        foreach (MKH_PlayerEntry entry in playerEntries)
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

        // 내가 방장이었을 경우
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            // 모든 플레이어가 레디가 되어있으면 스타트버튼 활성화
            startButton.interactable = CheckAllReady();
        }
        // 내가 방장이 아닌 경우
        else
        {
            startButton.interactable = false;
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

    // 플레이어 업데이트
    public void UpdatePlayerProperty(Player targetPlayer, Hashtable properties)
    {
        // 레디 커스텀 프로퍼티를 변경한 경우면 READY 키가 있음
        if (properties.ContainsKey(CustomProperty.READY))
        {
            UpdatePlayers();
        }
    }

    // 레디 체크
    private bool CheckAllReady()
    {
        // 플레이어 리스트에 있는 플레이어들이 레디를 안했을 경우 비활성화
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetReady() == false)
                return false;
        }

        return true;
    }

    // 게임 실행
    public void StartGame()
    {
        // TODO : 게임 시작 구현
        PhotonNetwork.LoadLevel("GameScene");
        // 게임 진행 중 들어오기 금지
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    // 방 퇴장
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}
