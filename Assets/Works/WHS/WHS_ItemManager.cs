using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

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
        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnItem(new Vector3(Random.Range(0, 5), 1, Random.Range(0, 5)));
        }
    }

    public void SpawnItem(Vector3 position)
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
        item.photonView.RPC(nameof(WHS_Item.ApplyItemRPC), RpcTarget.All, statusModel.photonView.ViewID, item.type, item.value);
        PhotonNetwork.Destroy(item.gameObject);
    }
}
