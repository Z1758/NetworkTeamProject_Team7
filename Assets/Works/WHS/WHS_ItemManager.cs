using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Cockpit;

public class WHS_ItemManager : MonoBehaviourPun
{
    [System.Serializable]
    public class ItemPrefab
    {
        public ItemType type;
        public GameObject prefab;
    }

    [SerializeField] ItemPrefab[] itemPrefabs;

    public static WHS_ItemManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 spawnPos = new Vector3(Random.Range(0, 5), 1, Random.Range(0, 5));
            SpawnItem(spawnPos);
        }
    }
        
    private void SpawnItem(Vector3 position)
    {
        photonView.RPC(nameof(SpawnItemRPC), RpcTarget.MasterClient, position);
    }

    [PunRPC]
    private void SpawnItemRPC(Vector3 position)
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        ItemPrefab selectedItem = itemPrefabs[randomIndex];

        string itemPath = "GameObject/Items/" + selectedItem.prefab.name;
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        GameObject itemObj = PhotonNetwork.Instantiate(itemPath, position, rotation);
        WHS_Item item = itemObj.GetComponent<WHS_Item>();

        item.type = selectedItem.type;
        item.value = 200;
    }

    public void ApplyItem(StatusModel statusModel, WHS_Item item)
    {
        photonView.RPC(nameof(ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, item.type, item.value);
        PhotonNetwork.Destroy(item.gameObject);
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
