using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.Auth;
using Photon.Pun.Demo.Cockpit;

public class MKH_LoginPanel : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;

    [SerializeField] MKH_NickNamePanel nickNamePanel;
    [SerializeField] MKH_VerifyPanel verifyPanel;


    
    // 로그인
    public void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;

        MKH_FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
                CheckUserInfo();
                OnConnectedToMaster();
            });
    }

    // 유저 정보 체크
    private void CheckUserInfo()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        if (user == null)
            return;

        if (user.IsEmailVerified == false)
        {
            // 이메일 인증 진행
            verifyPanel.gameObject.SetActive(true);
        }
        else if (user.DisplayName == "")
        {
            // 닉네임 설정 진행
            nickNamePanel.gameObject.SetActive(true);
        }
        else
        {
            // 접속 진행
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("접속에 성공했다!");
        Debug.Log(PhotonNetwork.LocalPlayer.NickName);
        PhotonNetwork.LoadLevel("MKH_WaitingScene");
    }
}
