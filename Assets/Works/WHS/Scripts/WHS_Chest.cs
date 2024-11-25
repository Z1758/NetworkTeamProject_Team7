using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Chest : MonoBehaviourPun
{
    // 상자 충돌 후 아이템 생성
    // TODO : 플레이어가 상자 공격해서 열기
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                WHS_ItemManager.Instance.DestroyAllChests(this);

                Vector3 spawnPos = transform.position;
                spawnPos.y += 1f;

                WHS_ItemManager.Instance.SpawnItem(spawnPos);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}