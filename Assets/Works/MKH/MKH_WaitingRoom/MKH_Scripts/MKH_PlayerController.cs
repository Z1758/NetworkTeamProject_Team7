using Photon.Pun;
using UnityEngine;

public class MKH_PlayerController : MonoBehaviourPun
{
    [SerializeField] GameObject _camera;

    [SerializeField] CharacterController controller;
    [SerializeField] float moveSpeed;
    [SerializeField] Animator animator;

    private Vector3 inputDir;

    private void Start()
    {
        //Cursor.visible = false;
       //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        inputDir.x = Input.GetAxisRaw("Horizontal");
        inputDir.z = Input.GetAxisRaw("Vertical");

        transform.forward = _camera.transform.forward;
    }

    private  void FixedUpdate()
    {
        // 내 소유권의 네트워크 오브젝트가 아닌 경우
        if (photonView.IsMine == false)
            return;

        if (inputDir == Vector3.zero)
        {
            animator.SetBool("Walk", false);
            return;
        }
        animator.SetBool("Walk", true);

        inputDir *= moveSpeed;

        controller.Move(inputDir.z * _camera.transform.forward * Time.fixedDeltaTime);
        controller.Move(inputDir.x * _camera.transform.right * Time.fixedDeltaTime);

    }

}
