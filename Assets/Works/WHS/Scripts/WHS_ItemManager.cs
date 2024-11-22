using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Pun.Demo.Cockpit;
using static UnityEditor.Progress;

public class WHS_ItemManager : MonoBehaviourPun
{
    [System.Serializable]
    public class ItemPrefab
    {
        public ItemType type;
        public GameObject prefab;
    }

    [SerializeField] ItemPrefab[] itemPrefabs;
    [SerializeField] GameObject chestPrefab;
    [SerializeField] float chestDistance;

    [SerializeField] Vector3 chestPos;
    private List<WHS_Chest> chests = new List<WHS_Chest>();


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

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnChest(chestPos);
        }

    }

    public void SpawnItem(Vector3 position)
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

        photonView.RPC(nameof(SetItemValue), RpcTarget.MasterClient, item.photonView.ViewID, (int)selectedItem.type);
    }

    [PunRPC]
    private void SetItemValue(int itemViewID, int itemType)
    {
        PhotonView itemPV = PhotonView.Find(itemViewID);
        if (itemPV != null)
        {
            WHS_Item item = itemPV.GetComponent<WHS_Item>();

            item.type = (ItemType)itemType;
        }
    }

    public void ApplyItem(StatusModel statusModel, WHS_Item item)
    {
        if (photonView != null && photonView.ViewID != 0)
        {
            photonView.RPC(nameof(ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, (int)item.type, item.value);
        }
    }

    [PunRPC]
    public void ApplyItemRPC(int playerViewID, int itemTypeIndex, float itemValue)
    {
        ItemType itemType = (ItemType)itemTypeIndex;
        PhotonView playerPV = PhotonView.Find(playerViewID);

        StatusModel statusModel = playerPV.GetComponent<StatusModel>();

        if (statusModel != null && playerPV.IsMine)
        {
            switch (itemType)
            {
                case ItemType.HP:
                    statusModel.HP += itemValue;
                    Debug.Log($"체력 {itemValue} 회복");
                    break;
                case ItemType.MaxHP:
                    Debug.Log($"최대 체력 {itemValue} 증가");
                    break;
                case ItemType.Attack:
                    Debug.Log($"공격력 {itemValue} 증가");
                    break;
            }
        }
    }

    private void SpawnChest(Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient == false) return;

        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = position + Vector3.right * (i - 1) * chestDistance;
            photonView.RPC(nameof(SpawnChestRPC), RpcTarget.All, spawnPos);
        }
    }

    [PunRPC]
    private void SpawnChestRPC(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        GameObject chestObj = Instantiate(chestPrefab, position, rotation);
        WHS_Chest chest = chestObj.GetComponent<WHS_Chest>();
        chest.SetItemManager(this);
        chests.Add(chest);
    }

    public void DestroyAllChests(WHS_Chest destroyedChest)
    {
        foreach(WHS_Chest chest in chests)
        {
            if(chest != destroyedChest)
            {
                Destroy(chest.gameObject);
            }
        }
        chests.Clear();
    }
}
