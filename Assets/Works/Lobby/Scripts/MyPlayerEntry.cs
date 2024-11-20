
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class MyPlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button readyButton;
    [SerializeField] Button banButton;

    Player playerCheck;
    [SerializeField] string n;

    public void SetPlayer(Player player)
    {
        PhotonNetwork.EnableCloseConnection = true;
        n = player.NickName;
        playerCheck = player;
        if (player.IsMasterClient)
        {
            nameText.text = $"Host\n{player.GetNick()}";
        }
        else
        {
            nameText.text = player.GetNick();
        }


        readyButton.gameObject.SetActive(true);
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;


        if (player.GetReady())
        {
            readyText.text = "Ready";

        }
        else
        {
            readyText.text = "";
        }


        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            banButton.gameObject.SetActive(true);
            if(player == PhotonNetwork.LocalPlayer)
            banButton.gameObject.SetActive(false);
        }
        else
        {
            banButton.gameObject.SetActive(false);
        }

       
    }

    public void SetEmpty()
    {
        readyText.text = "";
        nameText.text = "None";
        readyButton.gameObject.SetActive(false);
        banButton.gameObject.SetActive(false);
    }


    public void Ready()
    {

        bool ready = PhotonNetwork.LocalPlayer.GetReady();


        if (ready)
        {

            PhotonNetwork.LocalPlayer.SetReady(false);

        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }
    }

    public void Ban()
    {
       
        Debug.Log(playerCheck.NickName);
        PhotonNetwork.CloseConnection(playerCheck);
    }
}
