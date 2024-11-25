using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_TEST : MonoBehaviour
{
    private ChatClient _chatClient;  // Photon Chat 클라이언트 객체

    private string _userName;        // 사용자 이름
    private string _currentChannelName;  // 현재 채팅 채널 이름
    private string _privateReceiver = "";    // 개인 메시지 수신자

    [SerializeField] GameObject _speechBubble;  // 말풍선 게임 오브젝트
    private Coroutine _speechBubbleCoroutine;   // 말풍선 코르틴;

    [SerializeField] TMP_InputField _inputField;  // 유저가 메시지를 입력하는 InputField UI 요소
    [SerializeField] Text _outputText;        // 메시지가 출력되는 Text UI 요소
    [SerializeField] Text _speechBubbleText;        // 말풍선에 메시지 출력


    [PunRPC]
    // 인 게임 말풍선 효과
    public void SpeechBubble(string lineString)
    {
        if (_speechBubbleCoroutine != null)
        {
            StopCoroutine(_speechBubbleCoroutine);
        }

        // 새로 Coroutine 시작
        _speechBubbleCoroutine = StartCoroutine(DisplaySpeechBubble(lineString));
    }

    private IEnumerator DisplaySpeechBubble(string lineString)
    {
        // 말풍선 표시
        _speechBubble.SetActive(true);

        // 말풍선 텍스트 설정
        _speechBubbleText.text += lineString + "\r\n";

        // 3초 동안 대기
        yield return new WaitForSeconds(3f);

        // 3초 후에 말풍선 숨기기
        _speechBubble.SetActive(false);
    }
}
