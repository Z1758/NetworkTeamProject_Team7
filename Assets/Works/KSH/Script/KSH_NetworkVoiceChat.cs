using Photon.Voice.PUN;
using Photon.Voice.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_NetworkVoiceChat : MonoBehaviour
{
    private PunVoiceClient _punVoiceClient;
    private Recorder _recorder;
    [SerializeField] Button _voiceSwitch;

    private void Awake()
    {
        // PunVoiceClient의 싱글톤 인스턴스 가져오기
        this._punVoiceClient = PunVoiceClient.Instance;
    }

    void Start()
    {
        // 게임 씬에 존재하는 Recorder 인스턴스 찾기
        if (this._recorder == null)
        {
            // Voice 오브젝트에 있는 Recorder 인스턴스 가져오기
            this._recorder = FindObjectOfType<Recorder>();

            if (this._recorder != null)
            {
                Debug.Log("Recorder 인스턴스를 성공적으로 불러왔습니다.");
                this._recorder.RecordingEnabled = true; // 녹음 활성화
                this._recorder.TransmitEnabled = true;  // 데이터 전송 활성화
            }
            else
            {
                Debug.LogError("Recorder를 찾을 수 없습니다. Voice 오브젝트가 씬에 존재하는지 확인하세요.");
            }
        }

        if (this._voiceSwitch != null)
        {
            this._voiceSwitch.onClick.AddListener(this.VoiceSwitchOnClick);
        }
    }

    void Update()
    {
        // 'T' 키로 마이크 ON/OFF 전환
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (this._recorder != null)
            {
                // TransmitEnabled 상태를 토글
                this._recorder.TransmitEnabled = !this._recorder.TransmitEnabled;
                Debug.Log("마이크 상태: " + (this._recorder.TransmitEnabled ? "ON" : "OFF"));
            }
        }
    }

    private void VoiceSwitchOnClick()
    {
        // 현재 Photon Voice의 클라이언트 상태 확인
        if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
        {
            // Voice 클라이언트가 현재 룸에 연결된 상태라면 연결 해제
            this._punVoiceClient.Disconnect();
        }
        else if (this._punVoiceClient.ClientState == Photon.Realtime.ClientState.PeerCreated ||
                 this._punVoiceClient.ClientState == Photon.Realtime.ClientState.Disconnected)
        {
            // Voice 클라이언트가 초기화 상태이거나 연결되지 않았다면 서버에 연결하고 룸에 참여
            this._punVoiceClient.ConnectAndJoinRoom(); // 서버 연결 후 룸에 자동으로 입장
        }
    }
}
