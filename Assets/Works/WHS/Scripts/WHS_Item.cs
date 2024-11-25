using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ItemType { HP, MaxHP, Attack, }

public class WHS_Item : MonoBehaviourPun, IPunObservable
{
    public ItemType type;
    public float value;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // ¾ÆÀÌÅÛ ¿òÁ÷ÀÓ
        float newY = startPos.y + Mathf.Sin(Time.time * 5f) * 0.3f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, 90f * Time.deltaTime, 0);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            StatusModel statusModel = collision.gameObject.GetComponent<StatusModel>();
            WHS_Inventory inventory = collision.gameObject.GetComponent<WHS_Inventory>();

            if (statusModel.photonView != null && statusModel.photonView.IsMine && inventory != null)
            {
                if (type == ItemType.HP)
                {
                    inventory.AddItem(type, 1);
                    Debug.Log("HPÆ÷¼Ç È¹µæ");
                }
                else
                {
                    WHS_ItemManager.Instance.ApplyItem(statusModel, this);
                }
                photonView.RPC(nameof(DestroyItemObj), RpcTarget.All);
            }
        }
    }
    

    // ¾ÆÀÌÅÛ È¹µæ ¹× Àû¿ë
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StatusModel statusModel = other.GetComponent<StatusModel>();
            WHS_Inventory inventory = other.GetComponent<WHS_Inventory>();

            if (statusModel.photonView != null && statusModel.photonView.IsMine && inventory != null)
            {
                if(type == ItemType.HP)
                {
                    inventory.AddItem(type, 1);
                    Debug.Log("HPÆ÷¼Ç È¹µæ");
                }
                WHS_ItemManager.Instance.ApplyItem(statusModel, this);
                photonView.RPC(nameof(DestroyItemObj), RpcTarget.All);
            }
        }
    }
    */
    [PunRPC]
    private void DestroyItemObj()
    {
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((int)type);
            stream.SendNext(value);
        }
        else
        {
            type = (ItemType)stream.ReceiveNext();
            value = (float)stream.ReceiveNext();
        }
    }
}
