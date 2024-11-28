using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WHS_Inventory : MonoBehaviourPun
{
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
    private StatusModel statusModel;
    private int hpPotionGrade = 1;

    private void Awake()
    {
        statusModel = GetComponent<StatusModel>();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            InitInventory();

            WHS_ItemManager.Instance.OnPotionGradeChanged += UpdatePotionGrade;
        }
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            WHS_ItemManager.Instance.OnPotionGradeChanged -= UpdatePotionGrade;
        }
    }

    // 아이템 추가
    public void AddItem(ItemType type, int amount)
    {
        if (items.ContainsKey(type))
        {
            items[type] += amount;
        }
        else
        {
            items[type] = amount;
        }
    }

    // 아이템 사용 호출
    public void UseItem(ItemType type)
    {
        if (items.ContainsKey(type) && items[type] > 0)
        {
            if (statusModel.HP == statusModel.MaxHP)
            {
                Debug.Log("체력이 최대입니다.");
                return;
            }

            items[type]--;
            photonView.RPC(nameof(UseItemRPC), RpcTarget.MasterClient, type, statusModel.photonView.ViewID);
        }
    }

    // 아이템 사용
    // TODO : 물약 애니메이션이나 이펙트
    [PunRPC]
    private void UseItemRPC(ItemType type, int playerViewID)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        if (playerPV != null)
        {
            StatusModel statusModel = playerPV.GetComponent<StatusModel>();

            if (WHS_ItemManager.Instance.itemData.TryGetValue(type, out WHS_Item item))
            {
                if (item is WHS_HPPotion hpPotion)
                {
                    hpPotion.UpdateGrade(WHS_ItemManager.Instance.GetPotionGrade());
                    item.value = hpPotion.value;
                }
                WHS_ItemManager.Instance.ApplyItem(statusModel, item);
                Debug.Log($"{statusModel.photonView.ViewID}가 {item.value} 회복");
            }
        }
    }

    // 아이템 개수 출력
    public int GetItemCount(ItemType type)
    {
        return items.ContainsKey(type) ? items[type] : 0;
    }

    // 시작 시 포션 3개 보유
    private void InitInventory()
    {
        AddItem(ItemType.HP, 3);
    }


    public void UpgradePotion()
    {
        if (photonView.IsMine)
        {
            WHS_ItemManager.Instance.UpgradePotion();       
        }
    }

    // 아이템매니저에서 포션 등급 변경되면 갱신
    private void UpdatePotionGrade(int grade)
    {
        hpPotionGrade = grade;
    }

}
