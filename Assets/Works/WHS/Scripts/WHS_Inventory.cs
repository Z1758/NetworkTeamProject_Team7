using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WHS_ItemManager;

public class WHS_Inventory : MonoBehaviourPun
{
    private Dictionary<ItemType, int> items = new Dictionary<ItemType, int>();
    private StatusModel statusModel;
    private int hpPotionGrade = 1;

    private GameObject healEffectPrefab;

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

    // ������ �߰�
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

    // ������ ��� ȣ��
    public void UseItem(ItemType type)
    {
        if (statusModel.HP <= 0)
        {
            Debug.Log("�׾����ϴ�");
            return;
        }

        if (items.ContainsKey(type) && items[type] > 0)
        {
            if (statusModel.HP == statusModel.MaxHP)
            {
                Debug.Log("ü���� �ִ��Դϴ�.");
                return;
            }

            items[type]--;
            photonView.RPC(nameof(UseItemRPC), RpcTarget.MasterClient, type, statusModel.photonView.ViewID);

            photonView.RPC(nameof(HealEffectRPC), RpcTarget.All, transform.position);

            if (photonView.IsMine)
            {
                PlayItemUseSound(type);
            }
        }
    }

    // ������ ���
    // TODO : ���� �ִϸ��̼��̳� ����Ʈ
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

                // PlayItemUseSound(type);

                Debug.Log($"{statusModel.photonView.ViewID}�� {item.value} ȸ��");
            }
        }
    }

    // ������ ���� ���
    public int GetItemCount(ItemType type)
    {
        return items.ContainsKey(type) ? items[type] : 0;
    }

    // ���� �� ���� 3�� ����
    private void InitInventory()
    {
        AddItem(ItemType.HP, 3);

        string itemPath = "GameObject/Items/" + "FX_Heal_01";
        healEffectPrefab = Resources.Load<GameObject>(itemPath);
    }


    public void UpgradePotion()
    {
        if (photonView.IsMine)
        {
            WHS_ItemManager.Instance.UpgradePotion();
        }
    }

    // �����۸Ŵ������� ���� ��� ����Ǹ� ����
    private void UpdatePotionGrade(int grade)
    {
        hpPotionGrade = grade;
    }

    // ȸ�� ����Ʈ ȣ��
    [PunRPC]
    private void HealEffectRPC(Vector3 position)
    {
        if (healEffectPrefab == null)
        {
            string itemPath = "GameObject/Items/FX_Heal_01";
            healEffectPrefab = Resources.Load<GameObject>(itemPath);
        }

        else if (healEffectPrefab != null)
        {
            GameObject effect = Instantiate(healEffectPrefab, position, Quaternion.identity);
            effect.transform.SetParent(transform);
            Destroy(effect, 2.0f);
        }
    }

    // ������ ȿ���� ���
    private void PlayItemUseSound(ItemType type)
    {
        if (type == ItemType.HP)
        {
            AudioClip clip = AudioManager.GetInstance().GetCommonSoundDic("HPPotion");
            AudioManager.GetInstance().PlaySound(clip);
        }
    }
}
