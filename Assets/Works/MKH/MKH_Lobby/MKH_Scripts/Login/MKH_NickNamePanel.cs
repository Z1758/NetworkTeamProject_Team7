using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MKH_NickNamePanel : MonoBehaviour
{
    [SerializeField] TMP_InputField nickNameInputField;

    public void Confirm()
    {
        string nickName = nickNameInputField.text;
        if (nickName == "")
        {
            Debug.LogWarning("닉네임을 설정해주세요.");
            return;
        }

        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        UserProfile profile = new UserProfile();
        profile.DisplayName = nickName;

        user.UpdateUserProfileAsync(profile)
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

                Debug.Log("User profile updated successfully.");

                PhotonNetwork.LocalPlayer.NickName = nickName;
                PhotonNetwork.ConnectUsingSettings();
                gameObject.SetActive(false);
            });
    }
}
