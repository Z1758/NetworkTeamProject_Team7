using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MKH_MenuPanel : MonoBehaviour
{
    [SerializeField] GameObject MenuPanel;
    private void Update()
    {
        Menu();
    }

    private void Menu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (MenuPanel.activeSelf == false)
            {
                Debug.Log("1");
                MenuPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (MenuPanel.activeSelf == true)
            {
                Debug.Log("2");
                MenuPanel.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }
}
