using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerifyPanel : MonoBehaviour
{
    [SerializeField] NickNamePanel nickNamePanel;
    Coroutine checkVerifyRoutine;
    private void OnEnable()
    {
        SendVerifyMail();
    }

    private void OnDisable()
    {
        if(checkVerifyRoutine != null)
        {
            StopCoroutine(checkVerifyRoutine);
        }
    }


    private void SendVerifyMail()
    {
        FirebaseUser user = BackendManager.Auth.CurrentUser;
        if (user == null)
            return;

        user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SendEmailVerificationAsync was canceled.");
                gameObject.SetActive(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SendEmailVerificationAsync encountered an error: " + task.Exception);
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Email sent successfully.");
            checkVerifyRoutine =  StartCoroutine(CheckVerifyRoutine());
        });
    }

    IEnumerator CheckVerifyRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(3.0f);
        while (true)
        {
            BackendManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task =>
            {
                if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
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

                    if (BackendManager.Auth.CurrentUser.IsEmailVerified == true)
                    {
                        FirebaseUser user = BackendManager.Auth.CurrentUser;
                        if (user.DisplayName == "")
                        {
                            nickNamePanel.gameObject.SetActive(true);
                        }
                        
                        gameObject.SetActive(false);
                    }

                }
            });

            yield return delay;

        }
    }
}
