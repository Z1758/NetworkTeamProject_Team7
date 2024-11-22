using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MKH_MenuPanel : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;

    private void Awake()
    {
    }

    void Start()
    {
        menuPanel.SetActive(false);
       
    }

    private void Update()
    {
        Menu();
    }

    private void Menu()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            menuPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            menuPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
