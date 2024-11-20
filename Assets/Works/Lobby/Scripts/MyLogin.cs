using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MyLogin : MonoBehaviour
{
    [SerializeField] TMP_InputField idText;
    [SerializeField] TMP_InputField nickText;
    private void Start()
    {
        idText.text = $"ID {Random.Range(1000, 10000)}";
        nickText.text = $"NickName";
    }

    public void SetLogin()
    {
        if (idText.text == "")
        {
            Debug.LogWarning("아이디를 입력해야 접속이 가능합니다.");
            return;
        }
        if (nickText.text == "")
        {
            Debug.LogWarning("닉네임을 입력해야 접속이 가능합니다.");
            return;
        }
        PhotonNetwork.LocalPlayer.NickName = idText.text;
        PhotonNetwork.LocalPlayer.SetNickName(nickText.text);
      
        PhotonNetwork.ConnectUsingSettings();
        
    }
}
