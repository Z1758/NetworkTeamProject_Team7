using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Chest : MonoBehaviourPun
{
    // 상자 충돌 후 아이템 생성
    // TODO : 플레이어가 상자 공격해서 열기
    
    Animator animator;
    BoxCollider boxCollider;
    private void Awake()
    {

        animator = GetComponent<Animator>();
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hitbox"))
        {
            if (photonView.IsMine)
            {
                gameObject.layer = (int)LayerEnum.DISABLE_BOX;
                StartCoroutine(DestroyChest());
                
                //WHS_ItemManager.Instance.DestroyAllChests(this);

            
            }
        }
    }

    IEnumerator DestroyChest()
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        animator.Play("Open");

        yield return wait;
        Vector3 spawnPos = transform.position;
        spawnPos.y += 1f;

        WHS_ItemManager.Instance.SpawnItem(spawnPos);
        
        yield return wait;
        PhotonNetwork.Destroy(gameObject);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                // WHS_ItemManager.Instance.DestroyAllChests(this);

                Vector3 spawnPos = transform.position;
                spawnPos.y += 1f;

                WHS_ItemManager.Instance.SpawnItem(spawnPos);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
    */
}