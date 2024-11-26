using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MKH_WaitingPlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;

    public void SetPlayer(Player player)
    {
        //nameText.text = player.NickName;

        PlayerSpawn();
    }

    public void SetEmpty()
    {
        //nameText.text = "None";
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player(UI)", randomPos, Quaternion.identity);
    }
}
