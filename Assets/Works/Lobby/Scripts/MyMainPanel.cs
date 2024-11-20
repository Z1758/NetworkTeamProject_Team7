using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyMainPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
   
    [SerializeField] GameObject createRoomPanel;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    private void OnEnable()
    {
        createRoomPanel.SetActive(false);
    }

    public void CreateRoomMenu()
    {
        createRoomPanel.SetActive(true);

        roomNameInputField.text = $"Room {Random.Range(1000, 10000)}";
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text;
        if (roomName == "")
        {
            Debug.LogWarning("방 이름을 지정해주세요");
            return;
        }

        int maxPlayer = int.Parse(maxPlayerInputField.text);

        maxPlayer = Mathf.Clamp(maxPlayer, 1, 4);

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CreateRoomCancel()
    {
        createRoomPanel.SetActive(false);
    }

    public void RandomMatching()
    {
        Debug.Log("랜덤 매칭 요청");
        string roomName = $"Room {Random.Range(1000, 10000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 4 };



        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);

    }

    public void JoinLobby()
    {

        Debug.Log("로비 입장 신청");
        PhotonNetwork.JoinLobby();
    }

    public void Logout()
    {
        Debug.Log("로그아웃 요청");
        PhotonNetwork.Disconnect();
    }

    public void DeleteUser()
    {
        FirebaseUser user= BackendManager.Auth.CurrentUser;
        user.DeleteAsync().ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("DeleteAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("User deleted successfully.");
            PhotonNetwork.Disconnect();
        });
    }
}
