using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MKH_VerifyPanel : MonoBehaviour
{
    [SerializeField] MKH_NickNamePanel nickNamePanel;

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

    // 이메일 보내기
    private void SendVerifyMail()
    {
        FirebaseUser user = MKH_FirebaseManager.Auth.CurrentUser;
        user.SendEmailVerificationAsync()
            .ContinueWithOnMainThread(task =>
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
                checkVerifyRoutine = StartCoroutine(CheckVerifyRoutine());
            });
    }

    Coroutine checkVerifyRoutine;
    // 이메일 인증 확인 코루틴
    IEnumerator CheckVerifyRoutine()
    {
        WaitForSeconds delay = new WaitForSeconds(3f);

        while(true)
        {
            // 인증여부 새로고침(현재 유저 정보변경 시)
            MKH_FirebaseManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(TaskExtension =>
            {
                if(TaskExtension.IsCanceled)
                {
                    Debug.LogError("ReloadAsync was canceled.");
                    return;
                }
                if(TaskExtension.IsFaulted)
                {
                    Debug.LogError($"ReloadAsync encountered an error : {TaskExtension.Exception.Message}");
                    return;
                }

                //인증 확인
                if (MKH_FirebaseManager.Auth.CurrentUser.IsEmailVerified == true)
                {
                    Debug.Log("인증 확인");
                    nickNamePanel.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                }
            });

            yield return delay;
        }
    }
}
