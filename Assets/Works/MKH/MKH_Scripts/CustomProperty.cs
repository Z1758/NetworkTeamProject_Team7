using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
// Hashtable 기능을 PhotonHashtable 이라는 이름으로 사용을 하고 싶을 때 쓰는 코드(using 쓰임)
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;


public static class MKH_CustomProperty
{
    // 가비지 컬랙터를 줄이기 위한 코드
    private static PhotonHashtable customProperty = new PhotonHashtable();

    public const string READY = "Ready";

    public static void SetReady(this Player player, bool ready)
    {
        // 계속 추가를 방지하기 한 클리어
        customProperty.Clear();
        // 처음 방 들어왔을 때 Ready를 false로 해줌
        customProperty.Add(READY, ready);
        player.SetCustomProperties(customProperty);
    }

    public static bool GetReady(this Player player)
    {
        PhotonHashtable customProperty = player.CustomProperties;
        // Ready 버튼 눌렀을 때
        if(customProperty.ContainsKey(READY))
        {
            return (bool)customProperty[READY];
            
        }
        else
        {
            return false;
        }
    }
}
