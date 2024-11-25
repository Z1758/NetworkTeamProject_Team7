using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MKH_PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text readyText;
    [SerializeField] TMP_Text nameText;
    [SerializeField] Button readyButton;
    //[SerializeField] GameObject play;

    public void SetPlayer(Player player)
    {
        // 플레이어 닉네임
        // 방장인 플레이어
        if (player.IsMasterClient)
        {
            // 이름 양 옆에 별 있음(다른걸로 변경 가능)
            nameText.text = $"Master \n {player.NickName}";
        }
        // 방장이 아닌 플레이어
        else
        {
            nameText.text = player.NickName;
        }

        //play.SetActive(true);
        // 레디버튼 활성화
        readyButton.gameObject.SetActive(true);
        // 나 자신이면 레디버튼 클릭 가능
        readyButton.interactable = player == PhotonNetwork.LocalPlayer;
        if(player.GetReady())
        {
            readyText.text = "Ready";
        }
        else
        {
            readyText.text = "";
        }
    }

    public void SetEmpty()
    {
        readyText.text = "";
        nameText.text = "None";
        readyButton.gameObject.SetActive(false);
        //play.SetActive(false);
    }

    public void Ready()
    {
        // 레디가 풀려있으면 다시 클릭해주고
        // 클릭 되어있으면 다시 풀어주는 작업을 위한 코드
        bool ready = PhotonNetwork.LocalPlayer.GetReady();
        if (ready)
        {
            PhotonNetwork.LocalPlayer.SetReady(false);
        }
        else
        {
            PhotonNetwork.LocalPlayer.SetReady(true);
        }

    }
}
