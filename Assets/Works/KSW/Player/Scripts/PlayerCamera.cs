using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform mainCamera;

    private PlayerController pc;
    private Transform target;
   
    Vector3 offset;


    // 캐릭터 생성 체크
    bool isEndLoading;
  


    private Vector2 mouseDelta;

    public float rotateSpeed;

    // 카메라 줌
    List<Collider> wallCols = new List<Collider>();
    Vector3 mainCameraOffSet;
    Vector3 zoomOffSet;

    // 화면 흔들림
    bool isShake;
    Coroutine shakeRoutine;
    float shakeTime;


    public void SetComponent(PlayerController playerController)
    {
        target = playerController.transform;
        pc = playerController;
    }
    public void SetOffset()
    {
        offset = transform.position - target.position;
        offset.x = 0;
        offset.z = 0;

        mainCameraOffSet = mainCamera.localPosition;
        zoomOffSet = mainCameraOffSet +  new Vector3(0, -1, 4f);

        isEndLoading = true;
    }

    public void LookAround(InputAction.CallbackContext value)
    {
        
        if (!isEndLoading)
            return;
        mouseDelta = value.ReadValue<Vector2>();
        Vector3 angle = transform.rotation.eulerAngles;




        float maxAngle = angle.x - mouseDelta.y * rotateSpeed * Time.deltaTime;

        //위에서 아래 보는 각도
        if (maxAngle < 180f)
        {
            maxAngle = Mathf.Clamp(maxAngle, -1f, 40f);
        }
        //아래에서 위 보는 각도
        else
        {
            maxAngle = Mathf.Clamp(maxAngle, 340f, 361f);
        }


        transform.rotation = Quaternion.Euler(maxAngle, angle.y + mouseDelta.x * rotateSpeed * Time.deltaTime, angle.z);


        pc.InputDir();
    }


    private void OnTriggerEnter(Collider other)
    {
        wallCols.Add(other);
        mainCamera.localPosition = zoomOffSet;
        
       
    }

    private void OnTriggerExit(Collider other)
    {
        wallCols.Remove(other);

        if(wallCols.Count == 0 ) 
            mainCamera.localPosition = mainCameraOffSet;
    }


    private void LateUpdate()
    {
        if (!isEndLoading)
            return;

        //화면 흔들림
        if (isShake)
        {
            Shake();
        }
        else
        {
            if (!target)
                return;
            transform.position = target.position + offset;
        }

    }


    private void Shake()
    {
        if(!target) return;
        

        float ranX = Random.Range(-0.05f, 0.05f);
        float ranY = Random.Range(-0.05f, 0.05f);
        float ranZ = Random.Range(-0.05f, 0.05f);
        Vector3 shake = new Vector3(ranX, ranY, ranZ);

        transform.position = target.position + offset + shake;
    }

    public void StartShake(float time)
    {
        isShake = true;
        shakeTime = time;


        if (shakeRoutine  is not null)
        {
            StopCoroutine(shakeRoutine);
        }
        shakeRoutine = StartCoroutine(ShakeTime());
    }

  

    IEnumerator ShakeTime()
    {
        while (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;
            yield return null;

        }

        isShake = false;
    }



}
