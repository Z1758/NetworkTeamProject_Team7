using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WHS_InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    [SerializeField] Button potionButton;
    private int potionCount;
    private WHS_Inventory inventory;

    [SerializeField] Image potionImage;
    [SerializeField] Sprite[] potionSprites;

    private void Start()
    {
        potionButton.onClick.AddListener(UsePotion);
        StartCoroutine(FindPlayer());

        WHS_ItemManager.Instance.OnPotionGradeChanged += UpdatePotionImage;

        UpdatePotionImage(WHS_ItemManager.Instance.GetPotionGrade());
    }

    private void Update()
    {
        if (inventory != null)
        {
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.H))
            {
                UsePotion();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                inventory.UpgradePotion();
            }
        }
    }

    // 인벤토리를 가진 플레이어 찾기
    IEnumerator FindPlayer()
    {
        while (inventory == null)
        {
            yield return new WaitForSeconds(1f);

            foreach (PhotonView photonView in PhotonNetwork.PhotonViewCollection)
            {
                if (photonView.IsMine)
                {
                    GameObject player = photonView.gameObject;
                    inventory = player.GetComponent<WHS_Inventory>();

                    if (inventory != null)
                    {
                        Debug.Log($"인벤토리 등록 완료 {photonView.ViewID}");
                        break;
                    }
                }
            }
        }
    }

    public void UpdateUI()
    {
        potionCount = inventory.GetItemCount(ItemType.HP);
        potionCountText.text = $"{potionCount}";
        potionButton.interactable = potionCount > 0;
    }

    private void UsePotion()
    {
        if (potionCount > 0)
        {
            inventory.UseItem(ItemType.HP);
        }
        else
        {
            Debug.Log("보유중인 포션이 없습니다.");
        }
    }

    private void UpdatePotionImage(int grade)
    {
        if (grade - 1 < potionSprites.Length)
        {
            potionImage.sprite = potionSprites[grade - 1];
        }
        else
        {
            return;
        }
    }
}
