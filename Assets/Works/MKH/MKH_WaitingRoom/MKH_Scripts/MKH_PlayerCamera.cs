using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MKH_PlayerCamera : MonoBehaviour
{
    [SerializeField] Transform player;

    [SerializeField] float rotateSpeed;
    [SerializeField ]Vector3 offSet;

    float yAngle;

    private void Update()
    {
        yAngle += Input.GetAxis("Mouse X") * rotateSpeed;
        float mouseX = Input.GetAxis("Mouse X") * rotateSpeed;
        transform.rotation = Quaternion.Euler(0, yAngle, 0);
    }

    private void LateUpdate()
    {
        transform.position = player.position + offSet;
    }


}
