using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MKH_ServerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text serverName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button joinRoomButton;

    public void SetServerInfo(RoomInfo info)
    {
        serverName.text = info.Name;
        currentPlayer.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(serverName.text);
    }
}
