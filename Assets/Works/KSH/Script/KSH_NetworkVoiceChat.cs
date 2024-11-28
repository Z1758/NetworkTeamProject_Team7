//using Photon.Voice.PUN;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class KSH_NetworkVoiceChat : MonoBehaviour
//{
//    private PunVoiceClient punVoiceClient;

//    private void Awake()
//    {
//        this.punVoiceClient = PunVoiceClient.Instance;
//    }


//    private void VoiceSwitchOnClick()
//    {
//        // 현재 Photon Voice의 클라이언트 상태 확인
//        if (this.punVoiceClient.ClientState == Photon.Realtime.ClientState.Joined)
//        {
//            // Voice 클라이언트가 현재 룸에 연결된 상태라면 연결 해제
//            this.punVoiceClient.Disconnect();
//        }
//        else if (this.punVoiceClient.ClientState == Photon.Realtime.ClientState.PeerCreated ||
//                 this.punVoiceClient.ClientState == Photon.Realtime.ClientState.Disconnected)
//        {
//            // Voice 클라이언트가 초기화 상태이거나 연결되지 않았다면 서버에 연결하고 룸에 참여
//            this.punVoiceClient.ConnectAndJoinRoom(); // 서버 연결 후 룸에 자동으로 입장
//        }
//    }
//}
