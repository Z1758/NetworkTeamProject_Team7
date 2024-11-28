using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConnectionManager : MonoBehaviourPunCallbacks
{
    public static ConnectionManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log(this.gameObject.name + "1");
        }
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
        print(nameof(OnClickConnect));
        Debug.Log(this.gameObject.name + "2");
    }
}
