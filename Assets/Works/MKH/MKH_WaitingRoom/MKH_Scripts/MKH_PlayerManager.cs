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

    // 실제 Player를 담고있는 부모 gameobject
    public GameObject playerList;

    private void Start()
    {
        // 내 캐릭터가 아니면 카메라 끄기
        if (!photonView.IsMine)
        {
            camera.enabled = false;
        }

        // 닉네임 설정
        nickName.text = photonView.Owner.NickName; // connection manager의 join room에서 설정해줌

    }

    public void SelectModel(string characterName)
    {
        // rpc 함수로 캐릭터를 생성
        photonView.RPC(nameof(RpcSelectModel), RpcTarget.AllBuffered, characterName);
    }

    [PunRPC]
    void RpcSelectModel(string characterName)
    {
        // Player 프리팹 안에 들어있는 캐릭터 중
        foreach (Transform t in playerList.transform)
        {
            if (t.name == characterName)
            {
                t.gameObject.SetActive(true);
            }
        }
    }
}
