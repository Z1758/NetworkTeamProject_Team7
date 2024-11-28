using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MKH_PlayerManager : MonoBehaviourPun
{
    // 나의 카메라
    public Camera camera;

    // 프리팹의 닉네임
    public TMP_Text nickName;

    private void Start()
    {
        camera = Camera.main;

        // 내 캐릭터가 아니면 카메라 끄기
        if (!photonView.IsMine)
        {
            camera.enabled = false;
        }

        // 닉네임 설정
        nickName.text = photonView.Owner.NickName;

    }

   
}
