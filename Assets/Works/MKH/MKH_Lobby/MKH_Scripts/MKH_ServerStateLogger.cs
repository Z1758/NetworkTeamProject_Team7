using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_ServerStateLogger : MonoBehaviourPunCallbacks 
{
    [SerializeField] ClientState state;

    private void Update()
    {
        if (state == PhotonNetwork.NetworkClientState)      // 예전 상태와 같은 상태면 동작 금지
            return;

        state = PhotonNetwork.NetworkClientState;       // 현재 상태 확인(입장했는지 아닌지와 같은 상황)
        Debug.Log($"[Pun] {state}");
    }
}
