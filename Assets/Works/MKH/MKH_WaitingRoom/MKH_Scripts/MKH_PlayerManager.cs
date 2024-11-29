using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;

public class MKH_PlayerManager : MonoBehaviourPun
{
    // 나의 카메라
    [SerializeField] Camera camera;

    // 프리팹의 닉네임
    [SerializeField] TMP_Text nickName;

    private void Start()
    {
        // 내 캐릭터가 아니면 카메라 끄기
        if (!photonView.IsMine)
        {
            camera.gameObject.SetActive(false);
        }

        camera = Camera.main;

        // 닉네임 설정
        nickName.text = photonView.Owner.NickName;
    }


}
