using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    public static PopupPanel Instance { get; private set; }
    [SerializeField] TMP_Text popupText;
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
        gameObject.SetActive(false);
    }

  

    public void SetPopupText(string text)
    {
        popupText.text = text;
        gameObject.SetActive(true);
    }
}
