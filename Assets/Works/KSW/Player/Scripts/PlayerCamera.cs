using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static MirzaBeig.ParticleSystems.Demos.CameraShake;
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] public PlayerController pc;
    [SerializeField] public Transform target;
    [SerializeField] private Transform mainCamera;


    [SerializeField] Vector3 offset;
 
    [SerializeField] float smoothTime;
  

    private Vector2 mouseDelta;

    public float rotateSpeed;



    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
       
    }

    public void SetOffset()
    {
        offset = transform.position - target.position;
        offset.x = 0;
        offset.z = 0;
  
    }

    public void LookAround(InputAction.CallbackContext value)
    {
        
        if (target == null)
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

    [Header("화면 흔들림")]
    [SerializeField] bool isShake;
    Coroutine shakeRoutine;
    float shakeTime;

    private void Update()
    {
        
        if (target == null)
            return;
        //화면 흔들림
        if (isShake)
        {
            Shake();
        }
        else
        {
            transform.position = target.position + offset ;
        }

    }

    private void Shake()
    {

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
        if (shakeRoutine  != null)
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
