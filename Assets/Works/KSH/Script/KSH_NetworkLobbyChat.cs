using ExitGames.Client.Photon;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkLobbyChat : MonoBehaviour, IChatClientListener
{
    private ChatClient _chatClient;  // Photon Chat 클라이언트 객체

    private string _userName;        // 사용자 이름
    private string _currentChannelName;  // 현재 채팅 채널 이름
    private string _privateReceiver = "";    // 개인 메시지 수신자
    [SerializeField] TMP_InputField _inputField;  // 유저가 메시지를 입력하는 InputField UI 요소
    [SerializeField] Text _outputText;        // 메시지가 출력되는 Text UI 요소

    private string _friendInputField; // 친구 이름 입력 필드
    [SerializeField] GameObject _friendListContent; // 친구 목록이 표시될 Scroll View의 Content
    [SerializeField] GameObject _friendItemPrefab; // 친구 항목에 사용할 Prefab


    DatabaseReference _userDataRef;
    DatabaseReference _friend;

    // 친구 상태를 저장하는 딕셔너리 (친구 이름 -> 상태)
    private Dictionary<string, string> _friendStatuses = new Dictionary<string, string>();

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
        _currentChannelName = "Channel 001";

        // ChatClient 생성, IChatClientListener를 `this`로 전달
        _chatClient = new ChatClient(this);

        //// 채팅 서버의 지역 설정 EU, US, ASIA
        // chatClient.ChatRegion = "ASIA";

        // 서버 연결을 시도
        Debug.Log(_chatClient.AppId);
        _chatClient.Connect(_chatID, "1.0", new AuthenticationValues(_userName));  // Photon Chat 서버에 연결 시도

        // 연결 시도 메시지 출력 {0}에 부분에 nserName 표기
        AddLine(string.Format(" 연결시도 : {0}", _userName));

        // 사용자 ID
        string uid = BackendManager.Auth.CurrentUser.UserId;

        // 데이터베이스 자료 위치 지정
        _userDataRef = BackendManager.Database.RootReference.Child("UserData").Child(uid);
        _friend = _userDataRef.Child("Friend");

        LoadFriendsFromFirebase();
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

    // 친구 추가 메서드
    public void AddFriend()
    {
        // 입력된 친구 이름 가져오기
        string friendName = _friendInputField;

        // 이름이 비어 있으면 반환
        if (string.IsNullOrEmpty(friendName)) return;

        // 친구 유효성 검사 후 추가
        //if ( TODO 포톤 챗에 사용자들 정보 불러오기 필요)
        //{
        //    AddLine($"친구 '{friendName}'이(가) 존재하지 않거나 오프라인입니다.");
        //    return;
        //}

        // Photon Chat에 친구 추가 요청
        _chatClient.AddFriends(new string[] { friendName });

        // UI에 친구 항목 추가
        AddFriendToUI(friendName, "대기 중");
        
        // 상태를 딕셔너리에 저장
        _friendStatuses[friendName] = "대기 중";

        // Firebase에 친구 목록 저장
        SaveFriendsToFirebase();

        // 입력 필드 초기화
        _friendInputField = "";
    }

    // 친구 제거 메서드
    public void RemoveFriend(string friendName)
    {
        // Photon Chat에서 친구 제거 요청
        _chatClient.RemoveFriends(new string[] { friendName });

        // UI에서 친구 항목 제거
        foreach (Transform child in _friendListContent.transform)
        {
            FriendItem friendItem = child.GetComponent<FriendItem>();
            if (friendItem != null && friendItem.FriendName == friendName)
            {
                Destroy(child.gameObject); // UI 항목 삭제
                break;
            }
        }

        // 로컬 상태 삭제
        _friendStatuses.Remove(friendName);

        // Firebase에서 친구 데이터 삭제
        _friend.Child(friendName).RemoveValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"친구 {friendName} 데이터 삭제 성공!");
            }
            else
            {
                Debug.LogError($"친구 {friendName} 데이터 삭제 실패: {task.Exception}");
            }
        });
    }

    // Firebase에 친구 목록 저장
    private void SaveFriendsToFirebase()
    {
        foreach (var friend in _friendStatuses)
        {
            // 친구 이름을 키로 사용하여 상태를 저장
            _friend.Child(friend.Key).SetValueAsync(friend.Value).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"친구 {friend.Key} 상태 저장 성공!");
                }
                else
                {
                    Debug.LogError($"친구 {friend.Key} 상태 저장 실패: {task.Exception}");
                }
            });
        }
    }

    // Firebase에서 친구 목록 불러오기
    private void LoadFriendsFromFirebase()
    {
        _friend.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("GetValueAsync encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;

            // 가져온 데이터가 존재하지 않는 경우 처리.
            if (!snapshot.Exists)
            {
                Debug.Log("데이터가 존재하지 않습니다.");
                return;
            }

            // 데이터가 존재하면 각 자식을 순회하여 처리.
            foreach (DataSnapshot child in snapshot.Children)
            {
                string friendName = child.Key;       // 친구 이름(Key).
                string friendStatus = child.Value.ToString(); // 친구 상태(Value).

                // 이미 UI와 로컬 딕셔너리에 친구가 존재하는지 확인
                if (_friendStatuses.ContainsKey(friendName))
                {
                    Debug.Log($"이미 로컬에 존재하는 친구: {friendName}, 상태: {friendStatus}");
                    continue; // 이미 존재하는 경우 건너뜀
                }

                // UI에 친구 항목 추가.
                AddFriendToUI(friendName, friendStatus);

                // 로컬 딕셔너리에 친구 상태 저장.
                _friendStatuses[friendName] = friendStatus;
            }

            Debug.Log("데이터 불러오기 성공");
        });
    }

    // UI에 친구 추가
    private void AddFriendToUI(string friendName, string status)
    {
        GameObject friendItemObject = Instantiate(_friendItemPrefab, _friendListContent.transform);
        FriendItem friendItem = friendItemObject.GetComponent<FriendItem>();
        friendItem.Setup(friendName, status, this); // 메인 스크립트(this)를 전달
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


    // 친구 추가 이름 설정
    public void FriendName(string valueIn)
    {
        _friendInputField = valueIn; // 입력된 값을 privateReceiver에 저장
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

        //// 채팅 채널을 Photon 방 이름으로 설정
        //_currentChannelName = PhotonNetwork.CurrentRoom.Name;

        // 연결된 후, 채널에 가입
        _chatClient.Subscribe(new string[] { _currentChannelName }, 0);  // 채널에 가입
        _chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }



    // 서버에 연결이 끊어졌을 때 호출되는 콜백
    public void OnDisconnected()
    {
        Debug.Log("연결 안됨");
        // 연결 끊어진 이유 확인을 위한 로그 추가
        Debug.Log("디스커넥트 이유: " + _chatClient.DebugOut.ToString());
        AddLine("서버에 연결이 끊어졌습니다.");  // 연결 끊어짐 메시지 출력
        _chatClient.SetOnlineStatus(ChatUserStatus.Offline);
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
    // string user (사용자 ID), int status(현재 상태 코드), bool gotMessage(사용자 정의 메세지 보낼지 여부), object message(상태 변경 시 사용자 정의 메세지)
    // 현재 상태 코드 1 = 온라인, 0 = 오프라인
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        string statusMessage; // 상태 메시지를 저장할 변수

        // 상태 코드에 따라 상태 메시지 설정
        switch (status)
        {
            case ChatUserStatus.Online:
                statusMessage = "온라인";
                break;
            case ChatUserStatus.Offline:
                statusMessage = "오프라인";
                break;
            default:
                statusMessage = $"상태 코드: {status}";
                break;
        }

        // 추가 메시지가 있으면 상태 메시지에 포함
        if (message != null)
        {
            statusMessage += $" (메시지: {message})";
        }

        // UI에서 상태 업데이트
        foreach (Transform child in _friendListContent.transform)
        {
            FriendItem friendItem = child.GetComponent<FriendItem>();
            if (friendItem != null && friendItem.FriendName == user)
            {
                friendItem.UpdateStatus(statusMessage); // 상태를 UI에 반영
                break; // 해당 친구를 찾았으므로 루프 종료
            }
        }

        // 딕셔너리에서 상태 업데이트
        if (_friendStatuses.ContainsKey(user))
        {
            _friendStatuses[user] = statusMessage; // 기존 상태를 업데이트
        }
        else
        {
            _friendStatuses.Add(user, statusMessage); // 새로운 상태를 추가
        }

        // 디버그 출력 또는 로그 UI에 추가
        AddLine($"친구 {user} 상태 업데이트: {statusMessage}");
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