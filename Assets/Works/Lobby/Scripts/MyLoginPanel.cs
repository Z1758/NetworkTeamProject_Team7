using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WebSocketSharp;

public class MyLoginPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] NickNamePanel nickNamePanel;
    [SerializeField] VerifyPanel verifyPanel;
    public void Login()
    {
        string email = emailInputField.text;
        string password = passwordInputField.text;



        BackendManager.Auth.SignInWithEmailAndPasswordAsync(email, password)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    PopupPanel.Instance.SetPopupText("Email doesn't exist or password is incorrect");
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.Log($"User signed in successfully: {result.User.DisplayName} ({result.User.UserId})");
                CheckUserInfo();
            });
    }

    public void CheckUserInfo()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;

        Debug.Log($"Display Name : {user.DisplayName}");
        Debug.Log($"Email : {user.Email}");
        Debug.Log($"Email verified : {user.IsEmailVerified}");
        Debug.Log($"User ID : {user.UserId}");

        if(user.IsEmailVerified == false)
        {
            verifyPanel.gameObject.SetActive(true);
        }
        else if(user.DisplayName == "")
        {
            nickNamePanel.gameObject.SetActive(true);
            
        }
        else
        {
            // 접속 진행
            PhotonNetwork.LocalPlayer.NickName = user.DisplayName;
            PhotonNetwork.LocalPlayer.SetNickName(user.DisplayName);
            PhotonNetwork.LocalPlayer.SetPassWord(passwordInputField.text);
            PhotonNetwork.ConnectUsingSettings();
        }
    }
}
