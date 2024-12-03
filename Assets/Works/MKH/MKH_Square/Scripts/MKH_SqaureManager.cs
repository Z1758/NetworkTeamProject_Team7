using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MKH_SqaureManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(PlayerSpawns());
    }

    public void Warrior()
    {
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player1(UI)", gameObject.transform.position, Quaternion.identity);
    }

    public void Wizard()
    {
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player2(UI)", gameObject.transform.position, Quaternion.identity);
    }

    public void Assassin()
    {
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player(UI)", gameObject.transform.position, Quaternion.identity);
    }

    public void Viking()
    {
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player4(UI)", gameObject.transform.position, Quaternion.identity);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void PlayerSpawn()
    {
        Vector3 randPos = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));
        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player", randPos, Quaternion.identity);
    }

    IEnumerator PlayerSpawns()
    {
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.LocalPlayer.IsLocal)
        {
            PlayerSpawn();
        }
    }
}
