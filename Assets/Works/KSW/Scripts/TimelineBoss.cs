using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class TimelineBoss : MonoBehaviourPun
{
    [SerializeField] PlayableDirector timeline;
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] FractureObjectManager fractureObjectManager;

    private void Awake()
    {
        timeline = GetComponent<PlayableDirector>();
    }

    private void Update()
    {/*
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;
            photonView.RPC(nameof(PlayTimeLineRPC), RpcTarget.All, 1);
           
        }*/

    }

    public void StartTimeline(int num)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;
        photonView.RPC(nameof(PlayTimeLineRPC), RpcTarget.All, num);
    }

    [PunRPC]
    public void PlayTimeLineRPC(int num)
    {
       

        if (PhotonNetwork.IsMasterClient)
        {
            fractureObjectManager.SpawnObject();
            
            
        }
        StartCoroutine(DelayBossSpawn(num));
    }

    IEnumerator DelayBossSpawn(int num)
    {
        yield return new WaitForSeconds(3.0f);
        timeline.playableAsset = Resources.Load<PlayableAsset>($"GameObject/Timeline/Boss{num}");
        timeline.Play();
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.InstantiateRoomObject($"GameObject/Boss{num}", bossSpawnPoint.position, Quaternion.identity);
        }
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }
    
}
