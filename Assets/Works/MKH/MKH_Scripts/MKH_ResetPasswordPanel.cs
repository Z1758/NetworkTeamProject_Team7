using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MKH_ResetPasswordPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInputfield;

    public void SendResetEmail()
    {
        string email = emailInputfield.text;
        MKH_FirebaseManager.Auth.SendPasswordResetEmailAsync(email)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SendPasswordResetEmailAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SendPasswordResetEmailAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("Password reset email sent successfully.");
                gameObject.SetActive(false);
            });
    }
}
