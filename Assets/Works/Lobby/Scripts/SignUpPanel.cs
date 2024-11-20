using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using WebSocketSharp;

public class SignUpPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInputField;
    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_InputField passwordConfirmInputField;
  

    public void SignUp()
    {
        string name = nameInputField.text;
        string email = emailInputField.text;
        string pass = passwordInputField.text;
        string confirm = passwordConfirmInputField.text;

        if (name.IsNullOrEmpty())
        {

            Debug.LogWarning("이름을 입력해주세요");
            return;
        }

        if (email.IsNullOrEmpty())
        {
           
            Debug.LogWarning("이메일을 입력해주세요");
            return;
        }

        if(pass != confirm)
        {
            Debug.LogWarning("패스워드 불일치");
            return;
        }

        BackendManager.Auth.CreateUserWithEmailAndPasswordAsync(email, pass)
            .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                
                PopupPanel.Instance.SetPopupText(task.Exception.InnerException.Message);
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                
                return;
            }

            PopupPanel.Instance.SetPopupText("Available email");
            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.Log($"Firebase user created successfully: {result.User.DisplayName} ({result.User.UserId})");

            nameInputField.text = "";
            emailInputField.text = "";
            passwordInputField.text = "";
            passwordConfirmInputField.text = "";
            
        });

        UserProfile profile = new UserProfile();
        profile.DisplayName = name;

        BackendManager.Auth.CurrentUser.UpdateUserProfileAsync(profile)
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateUserProfileAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                return;
            }

          
        });

        gameObject.SetActive(false);
    }

}

