using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSH_LeaveButton : MonoBehaviour
{
    [SerializeField] GameObject _leaveGameObject;
    private Coroutine _leave;

    private void Start()
    {
        _leaveGameObject.SetActive(false);
        _leave = StartCoroutine(LeaveButtonOn());
    }

    IEnumerator LeaveButtonOn()
    {
        // 3초 동안 대기
        yield return new WaitForSeconds(1.5f);
        _leaveGameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        StopCoroutine(LeaveButtonOn());
    }
}
