using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAnimationHashNumber
{
    Wait, Run, Atk, Dodge, Down, Hit, Skill1,Skill2,Skill3,Skill4, Size
}

public class PlayerController : MonoBehaviourPun
{

    public enum PlayerState { Wait, Run, Attack, Hit, Down, Dodge, Dead, InputWait, Size }
    [SerializeField] PlayerState curState = PlayerState.Wait;
    private State[] states = new State[(int)PlayerState.Size];

    [SerializeField] GameObject playerHurtbox;
    [SerializeField] StatusModel model;

    [SerializeField] public Animator animator;
    [SerializeField] public int[] animatorParameterHash;
    public int skillNumberHash;

    // Todo : 추후에 분리
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;

    [SerializeField] public bool isFixed;
    [SerializeField] bool isMoveAni;

    [SerializeField] public Rigidbody rigid;

    [SerializeField] PlayerCamera playerCamera;

    // Todo : 추후에 분리
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference atk;
    [SerializeField] private InputActionReference skill;
    [SerializeField] private InputActionReference dodge;
    Vector3 dir;

    public Vector2 moveInputVec;

    Vector3 vertical;
    Vector3 horizontal;


    // todo : 추후에 다른 방식으로 정리
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip downSound;
    private void OnEnable()
    {
        if (photonView.IsMine == false)
            return;
        move.action.performed += MoveInput;
        move.action.canceled += MoveCancleInput;

        atk.action.started += AttackInput;
        skill.action.started += SkillInput;
        dodge.action.started += DodgeInput;
    }
    private void OnDisable()
    {
        if (photonView.IsMine == false)
            return;
        move.action.performed -= MoveInput;
        move.action.canceled -= MoveCancleInput;

        atk.action.started -= AttackInput;
        skill.action.started -= SkillInput;
        dodge.action.started -= DodgeInput;
    }
    private void Awake()
    {

        if (photonView.IsMine == false)
            return;
        model = GetComponent<StatusModel>();
        rigid = GetComponent<Rigidbody>();
        gameObject.AddComponent<AudioListener>();
        SetCamera();

        SetStates();

        SetAnimationHash();
    }

    private void SetCamera()
    {
        playerCamera = Camera.main.GetComponentInParent<PlayerCamera>();
        playerCamera.target = transform;
        playerCamera.pc = this;
        playerCamera.SetOffset();
    }

    private void SetAnimationHash()
    {
        animatorParameterHash = new int[(int)PlayerAnimationHashNumber.Size];
        for (int i = 0; i < animatorParameterHash.Length; i++)
        {
            AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];
            animatorParameterHash[i] = animatorControllerParameter.nameHash;
        }


    }

    private void SetStates()
    {
        states[(int)PlayerState.Wait] = new WaitState(this);
        states[(int)PlayerState.Run] = new RunState(this);
        states[(int)PlayerState.Attack] = new AttackState(this);
        states[(int)PlayerState.Hit] = new HitState(this);
        states[(int)PlayerState.Down] = new DownState(this);
        states[(int)PlayerState.Dodge] = new DodgeState(this);
        states[(int)PlayerState.Dead] = new DeadState(this);
        states[(int)PlayerState.InputWait] = new InputWaitState(this);
    }

    private void Start()
    {

        if (photonView.IsMine == false)
            return;
        states[(int)curState].EnterState();
      
        Debug.Log(PhotonNetwork.LocalPlayer.GetPlayerNumber());
    }

    private void OnDestroy()
    {
        if (photonView.IsMine == false)
            return;
        states[(int)curState].ExitState();
    }


    public void ChangeState(PlayerState state, bool flag)
    {
        if (photonView.IsMine == false)
            return;
        if (isFixed)
            return;

        isFixed = flag;
        states[(int)curState].ExitState();
        curState = state;
        states[(int)curState].EnterState();
    }

    private void Update()
    {
  

        if (photonView.IsMine == false)
        {
            rigid.velocity = Vector3.zero;
            return;

        }
    
            if (isFixed)
        {
            if (isMoveAni)
            {
                return;
            }
            rigid.velocity = Vector3.zero;
            return;
        }


        states[(int)curState].UpdateState();

        if (moveInputVec != Vector2.zero)
        {
            ChangeState(PlayerState.Run, false);


        }


        if (Input.GetMouseButtonDown(0))
        {


          

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {


         

        }
       
        
    }
    public void SkillInput(InputAction.CallbackContext value)
    {

        int index = value.action.GetBindingIndexForControl(value.control);
        Debug.Log(index);
    }

    public void AttackInput(InputAction.CallbackContext value)
    {
        if (PlayerState.InputWait == curState)
        {
                isFixed = false;
             
        }

        ChangeState(PlayerState.Attack, true);

    }
    public void DodgeInput(InputAction.CallbackContext value)
    {
        ChangeState(PlayerState.Dodge, true);
    }

    public void MoveInput(InputAction.CallbackContext value)
    {
        moveInputVec = value.ReadValue<Vector2>();
        InputDir();


    }

    
    public void MoveCancleInput(InputAction.CallbackContext value)
    {

        moveInputVec = value.ReadValue<Vector2>();
        if (moveInputVec == Vector2.zero)
        {
            ChangeState(PlayerState.Wait, false);

        }
    }

    public void AniEnd()
    {
        isFixed = false;
        ChangeState(PlayerState.Wait, false);
    }

    public void AttackEnd()
    {
        isFixed = false;
        ChangeState(PlayerState.InputWait, true);

    }
    public void DodgeEnd()
    {
        if (PlayerState.Down == curState || PlayerState.Hit == curState)
        {
            return;
        }
        isFixed = false;
        ChangeState(PlayerState.Wait, false);

    }
    public void EnterTriggerAni(bool down)
    {
       
        isFixed = false;
        AudioSource.PlayClipAtPoint(damageSound, transform.position + (Vector3.forward * 5));
        if (down)
        {
            AudioSource.PlayClipAtPoint(downSound, transform.position + (Vector3.forward*5));
            ChangeState(PlayerState.Down, true);
        }
        else
        {
           
            AudioSource.PlayClipAtPoint(hitSound, transform.position+ (Vector3.forward*5));
            ChangeState(PlayerState.Hit, true);

        }
    }

    public void InputDir()
    {




        vertical.x = playerCamera.transform.forward.x;
        vertical.z = playerCamera.transform.forward.z;

        horizontal.x = playerCamera.transform.right.x;
        horizontal.z = playerCamera.transform.right.z;


        vertical = vertical.normalized;
        horizontal = horizontal.normalized;


        dir = vertical * moveInputVec.y + horizontal * moveInputVec.x;

        dir.Normalize();
    }




    public void Move()
    {


        rigid.velocity = dir * speed;
        Rotate();



    }

    public void Rotate()
    {
     
        Quaternion lookRot = Quaternion.LookRotation(dir);

    
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);


    }
    public void MoveAni()
    {
        isMoveAni = true;
    }

    public void EndMoveAni()
    {
        isMoveAni = false;
        rigid.velocity = Vector3.zero;
    }




    public void TakeDamage(float damage, bool down, Vector3 target)
    {

        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllViaServer, damage, down, target);
    }
    [PunRPC]
    public void TakeDamageRPC(float damage, bool down, Vector3 target)
    {
        if (playerHurtbox.layer == (int)LayerEnum.DISABLE_BOX)
        {
            Debug.Log("DODGE!");
            return;
        }

        transform.LookAt(target);

        isFixed = false;
      
        if (down)
        {
            ChangeState(PlayerState.Down, true);

            animator.SetTrigger(animatorParameterHash[ (int)PlayerAnimationHashNumber.Down]);
        }
        else
        {
            ChangeState(PlayerState.Hit, true);
            animator.SetTrigger(animatorParameterHash[(int)PlayerAnimationHashNumber.Hit]);
        }



        Debug.Log("OUCH!!!!!!!!!!!!!!" + damage);
        model.HP -= (float)damage;
    }

    public void FreezingCheck()
    {
        if(curState != PlayerState.Wait)
        {
            isFixed = false;
            ChangeState(PlayerState.Wait, false);
        }
        
    }
}
