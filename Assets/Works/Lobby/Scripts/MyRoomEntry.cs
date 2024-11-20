using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyRoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] Button joinRoomButton;


    public void SetRoomInfo(RoomInfo info)
    {
        roomName.text = info.Name;
        currentPlayer.text = $"{info.PlayerCount}  / {info.MaxPlayers}";
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
    }

    public void JoinRoom()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
