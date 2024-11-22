using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Chest : MonoBehaviourPun
{
    private WHS_ItemManager itemManager;
    private bool isDestroyed = false;

    public void SetItemManager(WHS_ItemManager manager)
    {
        itemManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!isDestroyed && other.CompareTag("Player"))
        {
            isDestroyed = true;
            itemManager.DestroyAllChests(this);
            Vector3 spawnPos = transform.position;
            itemManager.SpawnItem(spawnPos);

            PhotonNetwork.Destroy(gameObject);
        }
    }
}