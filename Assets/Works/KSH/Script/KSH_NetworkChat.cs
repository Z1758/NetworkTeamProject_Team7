using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using Photon.Chat.Demo;
using UnityEngine.Networking;

public class KSH_NetworkChat : MonoBehaviour,IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat 클라이언트 객체

    private string _userName;        // 사용자 이름
    private string _currentChannelName;  // 현재 채팅 채널 이름
    public TMP_InputField InputField { get; private set; }  // 유저가 메시지를 입력하는 InputField UI 요소
    public Text OutputText { get; private set; }    // 메시지가 출력되는 Text UI 요소

    public string ChatID { get; private set; }

    private void Start()
    {
        // 백그라운드에서 실행 허용
        Application.runInBackground = true;

        // 사용자 이름을 로컬플레이어 이름으로 설정
        _userName = PhotonNetwork.LocalPlayer.NickName;

        // ChatID 설정 값 적용
        ChatID = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat;

        // 기본 채널 설정
        _currentChannelName = "Channel 001";

        // ChatClient 생성, IChatClientListener를 `this`로 전달
        _chatClient = new ChatClient(this);

        //// 채팅 서버의 지역 설정 EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // 서버 연결을 시도
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(ChatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat 서버에 연결 시도

        // 연결 시도 메시지 출력 {0}에 부분에 nserName 표기
        AddLine(string.Format("연결시도 : {0}", _userName));
    }

    public void AddLine(string lineString)
    {
        OutputText.text += lineString + "\r\n";  // outputText에 메시지를 추가
    }

    private void Update()
    {
        // 서버에 연결을 유지하고 지속적으로 호출하여 수신 메세지를 받습니다.
        _chatClient.Service();
    }


    // 메시지를 입력한 후 엔터 키가 눌렸을 때 호출되는 함수
    public void Input_OnEndEdit(string text)
    {
        // 연결 상태가 'ConnectedToFrontEnd'일 때만 메시지 전송 가능
        if (_chatClient.State == ChatState.ConnectedToFrontEnd)
        {
            // 채널에 메시지를 전송
            _chatClient.PublishMessage(_currentChannelName, InputField.text);

            // 메시지 입력 필드 초기화
            InputField.text = "";
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
        
    }



    // 서버에 연결되었을 때 호출되는 콜백
    public void OnConnected()
    {
        Debug.Log("연결");
        AddLine("서버에 연결되었습니다.");  // 연결 성공 메시지 출력

        // 연결된 후, 채널에 가입
        _chatClient.Subscribe(new string[] { _currentChannelName }, 10);  // 채널에 가입
    }



    // 서버에 연결이 끊어졌을 때 호출되는 콜백
    public void OnDisconnected()
    {
        
    }



    // 채널에서 메시지가 수신되었을 때 호출되는 콜백 channelName 채널이름, senders 사용자 이름, messages 메시지 내용
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        
    }



    // 개인 메시지가 수신되었을 때 호출되는 콜백
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }



    // 사용자 상태 업데이트가 있을 때 호출되는 콜백
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }



    // 채널 구독이 완료되었을 때 호출되는 콜백 channels 구독한 채널 이름 results는 구독 성공 여부
    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }



    // 채널 구독이 취소되었을 때 호출되는 콜백
    public void OnUnsubscribed(string[] channels)
    {
        
    }



    // 사용자가 채널에 가입했을 때 호출되는 콜백
    public void OnUserSubscribed(string channel, string user)
    {
        
    }



    // 사용자가 채널에서 퇴장했을 때 호출되는 콜백
    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }
}
