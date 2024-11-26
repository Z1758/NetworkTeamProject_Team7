using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_TEST : MonoBehaviourPun, IPunObservable
{
    private ChatClient _chatClient;  // Photon Chat 클라이언트 객체

    private string _userName;        // 사용자 이름
    private string _currentChannelName;  // 현재 채팅 채널 이름
    private string _privateReceiver = "";    // 개인 메시지 수신자
    private bool isWhispering = false;  // 귓말 모드인지 여부
    [SerializeField] TMP_InputField _privateInputField; // 개인 메시지 대상자 이름 입력

    private PhotonView myView;

    [SerializeField] GameObject _speechBubble;  // 말풍선 게임 오브젝트
    private Coroutine _speechBubbleCoroutine;   // 말풍선 코르틴;

    [SerializeField] GameObject _networkChat;       // 네트워크 채팅 오브젝트
    [SerializeField] TMP_InputField _inputField;  // 유저가 메시지를 입력하는 InputField UI 요소
    [SerializeField] Text _outputText;        // 메시지가 출력되는 Text UI 요소
    [SerializeField] Text _speechBubbleText;        // 말풍선에 메시지 출력


    private void Start()
    {
        myView = GetComponent<PhotonView>();    // 
        _speechBubble.SetActive(false);         // 오브젝트 비활성화
        _networkChat.SetActive(false);
    }

    private void Update()
    {
        // 서버에 연결을 유지하고 지속적으로 호출하여 수신 메세지를 받습니다.
        _chatClient.Service();

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Return))
        {
            if (_networkChat.activeSelf)
            {
                // 채팅창을 닫기
                _networkChat.SetActive(false);
                if (_inputField.text.Length > 0)
                {
                    if (_chatClient.State == ChatState.ConnectedToFrontEnd)
                    {
                        SubmitPublicChatOnClick();
                        SubmitPrivateChatOnClick();

                        // 채팅 메시지를 다른 사람에게 보냄
                        myView.RPC("OpenChatBox", RpcTarget.AllBuffered);
                        _inputField.text = "";
                        if (_speechBubbleCoroutine != null)
                        {
                            StopCoroutine(_speechBubbleCoroutine);
                        }
                        // 새로 Coroutine 시작
                        _speechBubbleCoroutine = StartCoroutine(DisplaySpeechBubble());
                    }
                }
            }
            else
            {
                // 채팅창 활성화
                _networkChat.SetActive(true);
                _inputField.ActivateInputField(); // 입력 필드 활성화
                _inputField.Select(); // 입력 필드에 포커스 설정
            }
        }

        // 탭 키를 눌러 입력란 전환
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInputField();
        }
    }


    // 프로그램 종료 시, 서버와의 연결을 끊기
    public void OnApplicationQuit()
    {
        if (_chatClient != null)
        {
            _chatClient.Disconnect();  // 애플리케이션 종료 시, 서버와의 연결을 끊습니다.
        }
    }


    public void AddLine(string lineString)
    {
        _outputText.text += lineString + "\r\n";  // outputText에 메시지를 추가
    }

    // 개인 메시지를 받을 수신자를 설정
    public void ReceiverOnValueChange(string valueIn)
    {
        _privateReceiver = valueIn; // 입력된 값을 privateReceiver에 저장
    }


    // 메시지를 입력한 후 엔터 키가 눌렸을 때 호출되는 함수
    public void Input_OnEndEdit(string text)
    {
        // 연결 상태가 'ConnectedToFrontEnd'일 때만 메시지 전송 가능
        if (_chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            SubmitPublicChatOnClick();
            SubmitPrivateChatOnClick();
        }
    }


    // 전체 채팅을 제출하는 함수
    public void SubmitPublicChatOnClick()
    {
        // 개인 메시지가 아니라면 공개 채팅으로 처리
        if (_privateReceiver == "")
        {
            // 채널에 메시지를 전송
            _chatClient.PublishMessage(_currentChannelName, _inputField.text);
        }

    }


    // 개인 채팅을 제출하는 함수
    public void SubmitPrivateChatOnClick()
    {

        if (_privateReceiver != "") // 수신자가 지정되어 있으면
        {
            _chatClient.SendPrivateMessage(_privateReceiver, _inputField.text); // 해당 수신자에게 개인 메시지 전송
        }
    }

    [PunRPC]
    public void OpenChatBox()
    {
        // 말풍선 표시
        _speechBubble.SetActive(true);

        // 말풍선 메시지 내용 초기화
        _speechBubbleText.text = "";

        // 말풍선 텍스트 설정
        _speechBubbleText.text = _inputField.text;
    }

    [PunRPC]
    public void CloseChatBox()
    {
        _speechBubble.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)  // 내가 데이터를 보낼 때
        {
            stream.SendNext(_speechBubbleText.text);  // 채팅 메시지 전송
        }
        else if (stream.IsReading) // 내가 데이터를 받을 때
        {
            _speechBubbleText.text = (string)stream.ReceiveNext();  // 채팅 메시지 수신
        }
    }

    private IEnumerator DisplaySpeechBubble()
    {
        // 3초 동안 대기
        yield return new WaitForSeconds(3);

        // 3초 후에 말풍선 숨기기
        myView.RPC("CloseChatBox", RpcTarget.AllBuffered);
    }

    // 채팅 입력란과 귓말 입력란을 전환
    private void ToggleInputField()
    {
        if (isWhispering)
        {
            // 귓말 모드에서 채팅 모드로 전환
            _inputField.Select(); // 채팅 입력란 포커스
        }
        else
        {
            // 채팅 모드에서 귓말 모드로 전환
            _privateInputField.Select(); // 귓말 대상자 입력란 포커스
        }

        // 귓말 모드 여부 토글
        isWhispering = !isWhispering;
    }
}
