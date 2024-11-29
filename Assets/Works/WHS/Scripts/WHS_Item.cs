using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public enum ItemType { 
    HP ,
    MaxHP, 
    Attack,
    AtkSpeed,
    MoveSpeed ,
    MaxStamina,
    ConsumeStamina,
    RecoveryStaminaMag,
    CriticalRate ,
    CriticalDamageRate,
    SkillCoolTime,
}

public class WHS_Item : MonoBehaviourPun, IPunObservable
{
    [Serializable]
    public struct AdditionalItemInfo
    {
        public ItemType type;
        public float value;

    }

    public AdditionalItemInfo[] additionalItemValue;

    public ItemType type;
    public float value;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // 아이템 움직임
        float newY = startPos.y + Mathf.Sin(Time.time * 5f) * 0.3f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, 90f * Time.deltaTime, 0);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StatusModel statusModel = collision.gameObject.GetComponent<StatusModel>();
            WHS_Inventory inventory = collision.gameObject.GetComponent<WHS_Inventory>();

            if (statusModel.photonView.IsMine && statusModel.photonView != null && inventory != null)
            {
                if (type == ItemType.HP)
                {
                    inventory.AddItem(type, 1);
                    Debug.Log($"{statusModel.photonView.Owner.NickName}이 {type} 아이템 획득");
                    Debug.Log($"현재 보유 수량 : {inventory.GetItemCount(ItemType.HP)}");
                }
                else
                {
                    WHS_ItemManager.Instance.ApplyItem(statusModel, this);
                }

                photonView.RPC(nameof(DestroyItemObj), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    protected virtual void DestroyItemObj()
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
