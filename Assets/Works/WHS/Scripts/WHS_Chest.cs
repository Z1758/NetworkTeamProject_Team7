using Photon.Pun;
using Photon.Voice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WHS_Chest : MonoBehaviourPun
{
    // ���� �浹 �� ������ ����
    // TODO : �÷��̾ ���� �����ؼ� ����
    
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
            /*
            if (photonView.IsMine)
            {
                gameObject.layer = (int)LayerEnum.DISABLE_BOX;
                StartCoroutine(DestroyChest());
            }
            */
            gameObject.layer = (int)LayerEnum.DISABLE_BOX;
            photonView.RPC(nameof(DestroyChestRPC), RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void DestroyChestRPC()
    {
        StartCoroutine(DestroyChest());
    }

    [PunRPC]
    public void ChestAnimation()
    {
        animator.Play("Open");
    }

    IEnumerator DestroyChest()
    {
        WaitForSeconds wait = new WaitForSeconds(1.0f);
        photonView.RPC(nameof(ChestAnimation), RpcTarget.All);

        yield return wait;
        for (int i = 0; i < 10; i++)
        {
            Vector3 spawnPos = transform.position;

            spawnPos.x = Random.Range(-5f, 5f);
            spawnPos.z = Random.Range(-5f, 5f);
            spawnPos.y += 1.5f;

            WHS_ItemManager.Instance.SpawnItem(spawnPos);



        }


    
        
        yield return wait;
        PhotonNetwork.Destroy(gameObject);
    }
}