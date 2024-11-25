using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;

public class KSH_NetworkRoomChat : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat 클라이언트 객체

    private string _userName;        // 사용자 이름
    private string _currentChannelName;  // 현재 채팅 채널 이름
    private string _privateReceiver = "";    // 개인 메시지 수신자
    [SerializeField] TMP_InputField _inputField;  // 유저가 메시지를 입력하는 InputField UI 요소
    [SerializeField] Text _outputText;        // 메시지가 출력되는 Text UI 요소

    private string _chatID;
    public string ChatID { get { return _chatID; } }
    public TMP_InputField InputField { get { return _inputField; } }
    public Text OutputText { get { return _outputText; } }

    private void OnEnable()
    {
        _outputText.text = "";

        // 백그라운드에서 실행 허용
        Application.runInBackground = true;

        // 사용자 이름을 로컬플레이어 이름으로 설정
        _userName = PhotonNetwork.LocalPlayer.NickName;

        // 설정 값 적용
        _chatID = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;

        // 기본 채널 설정
        _currentChannelName = "Channel 002";

        // ChatClient 생성, IChatClientListener를 `this`로 전달
        _chatClient = new ChatClient(this);

        //// 채팅 서버의 지역 설정 EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // 서버 연결을 시도
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(_chatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat 서버에 연결 시도

        // 연결 시도 메시지 출력 {0}에 부분에 nserName 표기
        AddLine(string.Format(" 연결시도 : {0}", _userName));
    }

    void OnDisable()
    {
        // UI 오브젝트가 파괴될 때 채널에서 나감
        if (_chatClient != null && _chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            _chatClient.Unsubscribe(new string[] { _currentChannelName });
            Debug.Log("채널에서 나갔습니다: " + _currentChannelName);
        }
    }

    private void Update()
    {
        // 서버에 연결을 유지하고 지속적으로 호출하여 수신 메세지를 받습니다.
        _chatClient.Service();
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

            // 메시지 입력 필드 초기화
            _inputField.text = "";
        }

    }


    // 개인 채팅을 제출하는 함수
    public void SubmitPrivateChatOnClick()
    {

        if (_privateReceiver != "") // 수신자가 지정되어 있으면
        {
            _chatClient.SendPrivateMessage(_privateReceiver, _inputField.text); // 해당 수신자에게 개인 메시지 전송
            _inputField.text = ""; // 입력 필드 초기화
        }
    }

    // 디버그 메시지를 출력하는 함수
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
        {
            Debug.Log(message); // 오류 메시지일 경우 에러 로그로 출력
        }
        else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
        {
            Debug.LogWarning(message);  // 경고 메시지일 경우 경고 로그로 출력
        }
        else
        {
            Debug.Log(message);  // 그 외 메시지는 일반 로그로 출력
        }
    }

    // 채널 상태가 변경될 때 호출되는 콜백
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);  // 채팅 상태가 변경될 때 상태를 로그로 출력
    }



    // 서버에 연결되었을 때 호출되는 콜백
    public void OnConnected()
    {
        AddLine("서버에 연결되었습니다.");  // 연결 성공 메시지 출력

        // 채팅 채널을 Photon 방 이름으로 설정
        _currentChannelName = PhotonNetwork.CurrentRoom.Name;

        // 연결된 후, 채널에 가입
        _chatClient.Subscribe(new string[] { _currentChannelName }, 0);  // 채널에 가입
    }



    // 서버에 연결이 끊어졌을 때 호출되는 콜백
    public void OnDisconnected()
    {
        Debug.Log("연결 안됨");
        // 연결 끊어진 이유 확인을 위한 로그 추가
        Debug.Log("디스커넥트 이유: " + _chatClient.DebugOut.ToString());
        AddLine("서버에 연결이 끊어졌습니다.");  // 연결 끊어짐 메시지 출력
    }



    // 채널에서 메시지가 수신되었을 때 호출되는 콜백 channelName 채널이름, senders 사용자 이름, messages 메시지 내용
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < messages.Length; i++)
        {
            // 각 메시지와 보낸 사람을 출력
            AddLine(string.Format("{0} : {1}", senders[i], messages[i].ToString()));
        }
    }



    // 개인 메시지가 수신되었을 때 호출되는 콜백
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage : " + message);  // 개인 메시지 내용 로그로 출력
        AddLine(string.Format("<color=magenta>{0} : {1}</color>", sender, message.ToString()));
    }



    // 사용자 상태 업데이트가 있을 때 호출되는 콜백
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status : " + string.Format("{0} is {1}, Msg : {2} ", user, status, message));
        // 상태 변경에 대한 로그 출력
    }



    // 채널 구독이 완료되었을 때 호출되는 콜백 channels 구독한 채널 이름 results는 구독 성공 여부
    public void OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("채널 입장 ({0})", string.Join(",", channels)));  // 채널에 입장한 후, 채널 이름 출력
    }



    // 채널 구독이 취소되었을 때 호출되는 콜백
    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("채널 퇴장 ({0})", string.Join(",", channels)));  // 채널에서 나갈 때, 퇴장한 채널 이름 출력
    }



    // 사용자가 채널에 가입했을 때 호출되는 콜백
    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"{user}님이 채널 '{channel}'에 가입했습니다!");
    }



    // 사용자가 채널에서 퇴장했을 때 호출되는 콜백
    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"{user}님이 채널 '{channel}'에 퇴장했습니다!");
    }
}