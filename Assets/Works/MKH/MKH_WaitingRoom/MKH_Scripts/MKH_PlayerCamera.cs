using System.Collections;
using System.Collections.Generic;
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
        player.Rotate(Vector3.up * mouseX * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0, yAngle, 0);
        player.forward = transform.forward;
    }

    private void LateUpdate()
    {
        transform.position = player.position + offSet;
    }


}
