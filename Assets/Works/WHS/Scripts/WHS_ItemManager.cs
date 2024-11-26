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

    public Dictionary<ItemType, WHS_Item> itemData = new Dictionary<ItemType, WHS_Item>();

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

    private void Start()
    {
        if (photonView.IsMine)
        {
            InitItemData();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        // TODO : 몬스터 사망 후 아이템 생성
        if (Input.GetKeyDown(KeyCode.I))
        {
            Vector3 spawnPos = new Vector3(Random.Range(0, 5), 1, Random.Range(0, 5));
            SpawnItem(spawnPos);
        }

        // TODO : 몬스터 사망 후 상자 생성
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnChest(chestPos);
        }
    }

    // 마스터 클라이언트에서만 아이템 생성 호출
    public void SpawnItem(Vector3 position)
    {
        photonView.RPC(nameof(SpawnItemRPC), RpcTarget.MasterClient, position);
    }

    // 지정된 타입 중 랜덤한 아이템 생성
    [PunRPC]
    private void SpawnItemRPC(Vector3 position)
    {
        int randomIndex = Random.Range(0, itemPrefabs.Length);
        ItemPrefab selectedItem = itemPrefabs[randomIndex];

        string itemPath = "GameObject/Items/" + selectedItem.prefab.name;
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);

        GameObject itemObj = PhotonNetwork.Instantiate(itemPath, position, rotation);
        WHS_Item item = itemObj.GetComponent<WHS_Item>();
    }

    // 획득한 아이템 스탯 적용 호출
    public void ApplyItem(StatusModel statusModel, WHS_Item item)
    {
        photonView.RPC(nameof(ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, (int)item.type, item.value);
    }

    // 각 플레이어 획득한 아이템 스탯 적용
    [PunRPC]
    public void ApplyItemRPC(int playerViewID, int itemTypeIndex, float itemValue)
    {
        ItemType itemType = (ItemType)itemTypeIndex;
        PhotonView playerPV = PhotonView.Find(playerViewID);

        if (playerPV != null)
        {
            StatusModel statusModel = playerPV.GetComponent<StatusModel>();

            if (statusModel != null)
            {
                switch (itemType)
                {
                    case ItemType.HP:
                        statusModel.HP += itemValue;
                        Debug.Log($"체력 {itemValue} 회복");
                        break;

                    // TODO : 체력 외 다른 스탯 증가?
                    case ItemType.MaxHP:
                        statusModel.MaxHP += itemValue;
                        Debug.Log($"최대 체력 {itemValue} 증가");
                        break;
                    case ItemType.Attack:
                        Debug.Log($"공격력 {itemValue} 증가");
                        break;
                }
            }
        }
    }


    // 마스터 클라이언트에서만 상자 생성
    private void SpawnChest(Vector3 position)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPos = position + Vector3.right * (i - 1) * chestDistance;
            photonView.RPC(nameof(SpawnChestRPC), RpcTarget.MasterClient, spawnPos);
        }
    }

    // 상자 생성, 배열에 추가
    [PunRPC]
    private void SpawnChestRPC(Vector3 position)
    {
        Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f);
        string chestPath = "GameObject/Items/" + chestPrefab.name;
        GameObject chestObj = PhotonNetwork.Instantiate(chestPath, position, rotation);
        WHS_Chest chest = chestObj.GetComponent<WHS_Chest>();
        chests.Add(chest);
    }

    // 직접 부순 상자 외 다른 상자 제거
    public void DestroyAllChests(WHS_Chest destroyedChest)
    {
        foreach (WHS_Chest chest in chests)
        {
            if (chest != destroyedChest)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.Destroy(chest.gameObject);
                }
            }
        }
        chests.Clear();
    }

    private void InitItemData()
    {
        foreach(ItemPrefab itemPrefab in itemPrefabs)
        {
            string itemPath = "GameObject/Items/" + itemPrefab.prefab.name;
            GameObject prefab = Resources.Load<GameObject>(itemPath);

            if(prefab != null)
            {
                WHS_Item item = prefab.GetComponent<WHS_Item>();
                if(item != null)
                {
                    itemData[item.type] = item;
                }
                else
                {
                    Debug.Log("WHS_Item없음");
                }
            }
            Debug.Log("프리팹을 찾을 수 없음" + itemPath);
        }
    }
}
