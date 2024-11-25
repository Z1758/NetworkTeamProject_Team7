using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class TimelineTest : MonoBehaviourPun
{
    [SerializeField] PlayableDirector timeline;
  
    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            photonView.RPC(nameof(PlayTimeLineRPC), RpcTarget.AllViaServer);
           
        }
    }
    [PunRPC]
    public void PlayTimeLineRPC()
    {

        timeline.Play();
  
        Vector3 randomPos = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        PhotonNetwork.InstantiateRoomObject("GameObject/Boss2", randomPos, Quaternion.identity);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
    
}
