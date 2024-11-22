using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_WaitingScene : MonoBehaviourPunCallbacks
{
    public const string RoomName = "WaitingRoom";

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"Player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(RoomName, options, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        PlayerSpawn();
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player(UI)", randomPos, Quaternion.identity);
    }
   
}
