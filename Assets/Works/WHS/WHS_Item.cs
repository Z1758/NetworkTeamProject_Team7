using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum ItemType { HP, MaxHP, Attack, }

public class WHS_Item : MonoBehaviourPun
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
        if (!photonView.IsMine) return;

        float newY = startPos.y + Mathf.Sin(Time.time * 5f) * 0.3f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        transform.Rotate(Vector3.up, 90f * Time.deltaTime, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision 충돌");
        if (collision.gameObject.CompareTag("Player"))
        {
            StatusModel statusModel = collision.gameObject.GetComponent<StatusModel>();
            if (statusModel.photonView.IsMine)
            {
                Debug.Log("아이템 적용");
                WHS_ItemManager.Instance.ApplyItem(statusModel, this);
            }
        }
    }

    [PunRPC]
    public void ApplyItemRPC(int viewID, ItemType itemType, float itemValue)
    {
        PhotonView pv = PhotonView.Find(viewID);

        StatusModel statusModel = pv.GetComponent<StatusModel>();
        if (statusModel != null)
        {
            switch (itemType)
            {
                case ItemType.HP:
                    statusModel.HP += itemValue;
                    Debug.Log($"체력 {itemValue} 회복");
                    break;
                case ItemType.MaxHP:
                    // statusModel.IncreaseMaxHP(itemValue);
                    Debug.Log($"최대 체력 {itemValue} 증가");
                    break;
                case ItemType.Attack:
                    // statusModel.IncreaseAttack(itemValue);
                    Debug.Log($"공격력 {itemValue} 증가");
                    break;
            }
        }
    }
}
