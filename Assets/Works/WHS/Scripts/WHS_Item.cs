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

    private bool isCollected = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {

        float newY = startPos.y + Mathf.Sin(Time.time * 5f) * 0.3f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, 90f * Time.deltaTime, 0);
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StatusModel statusModel = collision.gameObject.GetComponent<StatusModel>();
            if (statusModel.photonView != null && statusModel.photonView.IsMine)
            {
                Debug.Log("아이템 적용");
                isCollected = true;
                WHS_ItemManager.Instance.ApplyItem(statusModel, this);
                photonView.RPC(nameof(DestroyItemObj), RpcTarget.All);
            }
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if(!isCollected && other.CompareTag("Player"))
        {
            StatusModel statusModel = other.GetComponent<StatusModel>();

            if (statusModel.photonView != null && statusModel.photonView.IsMine)
            {
                Debug.Log("아이템 적용");
                isCollected = true;
                WHS_ItemManager.Instance.ApplyItem(statusModel, this);
                photonView.RPC(nameof(DestroyItemObj), RpcTarget.All);
            }
        }
    }

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
