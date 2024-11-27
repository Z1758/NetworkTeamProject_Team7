using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendItem : MonoBehaviour
{
    public string FriendName { get; private set; }
    [SerializeField] TMP_Text _friendNameText; // 친구 이름을 표시하는 텍스트
    [SerializeField] TMP_Text _statusText;      // 친구 상태를 표시하는 텍스트
    [SerializeField] Button _removeButton;       // 친구 삭제 버튼

    [SerializeField] KSH_NetworkLobbyChat _lobbyChat;

    // 친구 항목 초기화
    public void Setup(string friendName, string initialStatus, KSH_NetworkLobbyChat lobbyChat)
    {
        FriendName = friendName; // 이름 저장
        _friendNameText.text = friendName; // UI에 이름 표시
        UpdateStatus(initialStatus); // 초기 상태 설정

        _lobbyChat = lobbyChat;
        _removeButton.onClick.AddListener(OnRemoveButtonClicked);
    }

    // 상태 업데이트
    public void UpdateStatus(string status)
    {
        _statusText.text = status; // 상태 텍스트 업데이트
    }

    // 삭제 버튼 클릭 시 호출되는 메서드
    private void OnRemoveButtonClicked()
    {
        _lobbyChat.RemoveFriend(FriendName); // 메인 스크립트의 RemoveFriend 호출
    }
}
