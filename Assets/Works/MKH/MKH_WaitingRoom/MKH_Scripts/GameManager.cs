using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;

    private void Awake()
    {
        
    }

    void Start()
    {
        PlayerSpawn();
    }

    private void PlayerSpawn()
    {
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        PhotonNetwork.Instantiate("GameObject/MatchMaking/Player(UI)", randomPos, Quaternion.identity);
    }
   
}
