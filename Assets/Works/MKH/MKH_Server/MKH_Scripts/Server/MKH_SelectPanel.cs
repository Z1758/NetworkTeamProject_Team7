using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_SelectPanel : MonoBehaviourPunCallbacks
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                string CharacterName = hit.transform.name;
                PlayerPrefs.SetString("CharacterName", CharacterName);
                Debug.Log(CharacterName);
            }
        }
    }

    public void LoadRoom()
    {
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }
}
