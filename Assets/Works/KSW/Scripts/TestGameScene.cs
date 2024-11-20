using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class TestGameScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "TestRoom";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            BossSpawn();
        }
    }

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        options.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(StartDelayRoutine());
    }

    IEnumerator StartDelayRoutine()
    {
        yield return new WaitForSeconds(1f); // 네트워크 준비에 필요한 시간 살짝 주기
        TestGameStart();
    }

    public void TestGameStart()
    {
        Debug.Log("게임 시작");
        PlayerSpawn();

        // 방장만 진행하는 코드

        if (PhotonNetwork.IsMasterClient == false)
            return;

    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
           
        }
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
  
        PhotonNetwork.Instantiate("GameObject/Player", randomPos, Quaternion.identity);
        
    }

    private void BossSpawn()
    {

        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));


        PhotonNetwork.InstantiateRoomObject("GameObject/Boss", randomPos, Quaternion.identity);

    }

}
