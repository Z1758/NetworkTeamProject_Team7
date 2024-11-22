using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Chest : MonoBehaviourPun
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PhotonNetwork.IsMasterClient)
        {
            if (photonView.IsMine || PhotonNetwork.IsMasterClient)
            {
                WHS_ItemManager.Instance.DestroyAllChests(this);

                Vector3 spawnPos = transform.position;
                spawnPos.y += 1f;

                if (PhotonNetwork.IsMasterClient)
                {
                    WHS_ItemManager.Instance.SpawnItem(spawnPos);
                    PhotonNetwork.Destroy(gameObject);
                }
            }

        }
    }
}