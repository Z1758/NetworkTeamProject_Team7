using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;

using Photon.Realtime;
using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviourPunCallbacks
{
    [SerializeField] Button gameOverButton;
    [SerializeField] TMP_Text countDownText;
    private void Start()
    {
        PhotonNetwork.LocalPlayer.SetLoad(true);

       
        SetGameOverButton();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            Debug.Log($"{targetPlayer.NickName} 이 로딩이 완료되었다.");
            bool allLoaded = CheckAllLoad();
            Debug.Log($"모든 플레이어가 로딩 완료되었는가 : {allLoaded}");
            if (allLoaded)
            {
                GameStart();
            }
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        SetGameOverButton();
    }

    public void SetGameOverButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            gameOverButton.interactable = true;
        }
        else
        {
            gameOverButton.interactable = false;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void GameOver()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void GameStart()
    {
        StartCoroutine(CountDownRoutine());
    }

    IEnumerator CountDownRoutine()
    {
       for(int i = 3; i >0; i--)
        {
            countDownText.text = i.ToString();
            yield return new WaitForSeconds(1f);

        }

        Debug.Log("게임 시작!");
        countDownText.text = "Game Start!";
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
    }

    private bool CheckAllLoad()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad() == false)
            {
                return false;   
            }

          
        }
        return true;
    }
}
