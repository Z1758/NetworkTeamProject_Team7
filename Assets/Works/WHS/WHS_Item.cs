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
        1
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
}
