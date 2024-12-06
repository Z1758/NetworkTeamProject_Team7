using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using static UnityEngine.GraphicsBuffer;

public enum PlayerAnimationHashNumber
{
    Wait, Run, Atk, Dodge, Down, Hit, Skill1,Skill2,Skill3,Skill4, Skill5, Size
}

public class PlayerController : MonoBehaviourPun
{
    [Header("유저 이름")]
    [SerializeField] TMP_Text nameText;
    public enum PlayerState { Wait, Run, Attack, Hit, Down, Dodge, Dead, InputWait, Skill, Size }
    [Header("플레이어 상태")]
    [SerializeField] PlayerState curState = PlayerState.Wait;
    private State[] states = new State[(int)PlayerState.Size];

    [Header("플레이어 피격 콜라이더")]
    [SerializeField] GameObject playerHurtbox;

  
    // 애니메이터 파라미터 해싱
    [HideInInspector] public int[] animatorParameterHash;

    [HideInInspector] public int skillNumberHash;
    
    // 현재 사용한 스킬
    [HideInInspector]public int skillNumber;


    [Header("플레이어 회전 속도")]
    [SerializeField] float rotateSpeed;

    [Header("디버그 확인")]
    [SerializeField] public bool isFixed;
    [SerializeField] bool isMoveAni;
    [SerializeField] bool isOnChat;

    [Header("필수 컴포넌트")]
    [SerializeField] public Animator animator;
    [SerializeField] public Rigidbody rigid;
    [SerializeField] public StatusModel model;
    [SerializeField] PlayerCamera playerCamera;
    
    [SerializeField] PlayerInputSystem inputSystem;
    [SerializeField] GameObject[] weapon;
    WHS_Inventory inventory;

    Vector3 dir;

    Vector2 moveInputVec;

    Vector3 vertical;
    Vector3 horizontal;




    private void OnEnable()
    {
        if (photonView.IsMine == false)
            return;
        SetInputSystem(true);
    }
    private void OnDisable()
    {
        if (photonView.IsMine == false)
            return;
        SetInputSystem(false);
        if (freezingCheckCoroutine != null)
        {
            StopCoroutine(freezingCheckCoroutine);
        }

    }
    private void Awake()
    {
        SetAnimationHash();
        if (photonView.IsMine == false)
            return;

        SetComponent();
        SetCamera();

        SetStates();

    }

    

    private void SetComponent()
    {
        inputSystem = GetComponent<PlayerInputSystem>();
        model = GetComponent<StatusModel>();
        rigid = GetComponent<Rigidbody>();
       
        gameObject.AddComponent<AudioListener>();
        inventory = GetComponent<WHS_Inventory>();
    }

   
    public void SetInputSystem(bool active)
    {
        if (active)
        {
            inputSystem.move.action.performed += MoveInput;
            inputSystem.move.action.canceled += MoveCancleInput;

            inputSystem.atk.action.started += AttackInput;
            inputSystem.skill.action.started += SkillInput;
            inputSystem.dodge.action.started += DodgeInput;
        }
        else
        {
            inputSystem.move.action.performed -= MoveInput;
            inputSystem.move.action.canceled -= MoveCancleInput;

            inputSystem.atk.action.started -= AttackInput;
            inputSystem.skill.action.started -= SkillInput;
            inputSystem.dodge.action.started -= DodgeInput;
        }
    }

    public void Victory(Transform point)
    {
        isFixed = false;
        ChangeState(PlayerState.Wait, true);



        isFixed = true;
        for (int i = 0; i < animatorParameterHash.Length; i++)
        {
            animator.SetBool(animatorParameterHash[i], false);
        }

        for(int i = 0; i < weapon.Length; i++)
        {
            weapon[i].SetActive(false);
        }



        rigid.velocity = Vector3.zero;
        animator.Play("Victory");
        transform.SetPositionAndRotation(point.position, point.rotation);
        
        SetInputSystem(false);
      

    }

    private void SetCamera()
    {
        playerCamera = Camera.main.GetComponentInParent<PlayerCamera>();
        playerCamera.SetComponent(this);
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
        states[(int)PlayerState.Skill] = new SkillState(this);
    }

    private void Start()
    {
      
        
       // TestGameScene.Instance.players.Add(this);
        GameScene.Instance.players.Add(this);


        nameText.text = photonView.Owner.NickName;
        if (photonView.IsMine == false)
            return;

       
        states[(int)curState].EnterState();
     


        freezingCheckCoroutine = StartCoroutine(DodgeFreezingCheck());
      

    }
    
    private void OnDestroy()
    {
        if (photonView.IsMine == false)
            return;
     
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            isOnChat = !isOnChat;
        }

        if(isOnChat)
        {
            return;
        }

     
        ChangeCoolTime();


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death") && model.HP > 0)
        {
            animator.Play("StandUp");
            playerHurtbox.layer = (int)LayerEnum.PLAYER_HURT_BOX;
            gameObject.layer = (int)LayerEnum.PLAYER;
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

        RecoveryStamina();
        states[(int)curState].UpdateState();

        if (moveInputVec != Vector2.zero)
        {
            ChangeState(PlayerState.Run, false);


        }


        
    }

    public void ChangeCoolTime()
    {
        for (int i = 0; i < model.SkillCoolTime.Length; i++)
        {

            float cool = model.GetCurrentSkillCoolTime(i);
            if (cool > 0)
            {
                model.SetCurrentSkillCoolTime(i, cool - Time.deltaTime);
            }
        }
    }

    public void SkillInput(InputAction.CallbackContext value)
    {
        if (isOnChat)
            return;

        skillNumber = value.action.GetBindingIndexForControl(value.control);

        if(model.GetCurrentSkillCoolTime(skillNumber) > 0)
        {
            return;
        }

        switch (skillNumber)
        {
            case 0:
                {
                    skillNumberHash = animatorParameterHash[(int)PlayerAnimationHashNumber.Skill1];
                }
                break;
            case 1:
                {
                    skillNumberHash = animatorParameterHash[(int)PlayerAnimationHashNumber.Skill2];
                }
                break;
            case 2:
                {
                    skillNumberHash = animatorParameterHash[(int)PlayerAnimationHashNumber.Skill3];
                }
                break;
            case 3:
                {
                    skillNumberHash = animatorParameterHash[(int)PlayerAnimationHashNumber.Skill4];
                }
                break;
            case 4:
                {
                    skillNumberHash = animatorParameterHash[(int)PlayerAnimationHashNumber.Skill5];
                }
                break;
        }

        ChangeState(PlayerState.Skill, true);

    }

    public void AttackInput(InputAction.CallbackContext value)
    {
        if (isOnChat)
            return;
        if (PlayerState.InputWait == curState)
        {
                isFixed = false;
             
        }

        ChangeState(PlayerState.Attack, true);

    }
    public void DodgeInput(InputAction.CallbackContext value)
    {
        if (isOnChat)
            return;
        if ( model.Stamina < model.ConsumeStamina)
        {
            return;
        }
        ChangeState(PlayerState.Dodge, true);
    }

    public void MoveInput(InputAction.CallbackContext value)
    {
        if (isOnChat)
            return;
        moveInputVec = value.ReadValue<Vector2>();
        InputDir();


    }

    
    public void MoveCancleInput(InputAction.CallbackContext value)
    {
       
        moveInputVec = value.ReadValue<Vector2>();
        InputDir();
        if (moveInputVec == Vector2.zero)
        {
            ChangeState(PlayerState.Wait, false);

        }
    }

    public void RecoveryStamina()
    {
        if (model.Stamina < model.MaxStamina)
        {
            model.Stamina += Time.deltaTime * model.RecoveryStaminaMag;
        }
        if (model.Stamina > model.MaxStamina)
        {
            model.Stamina = model.MaxStamina;
        }
    }
    public void AniEnd()
    {
        freezingCnt = 0;

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
        freezingCnt = 0;
        isFixed = false;
        ChangeState(PlayerState.Wait, false);

    }
    public void EnterTriggerAni(bool down)
    {
       
        isFixed = false;
        ResetAtkSpeed();

        if (down)
        {
      
            
            ChangeState(PlayerState.Down, true);
        }
        else
        {
            rigid.velocity = Vector3.zero;
        
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
       
        rigid.velocity = dir * model.MoveSpeed;
       
        Rotate();



    }

    public void Rotate()
    {
     
        Quaternion lookRot = Quaternion.LookRotation(dir);

    
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, rotateSpeed * Time.deltaTime);


    }

    public void ImmediateRotate()
    {
        if (dir == Vector3.zero)
            return;

        Quaternion lookRot = Quaternion.LookRotation(dir);


        transform.rotation = lookRot;


    }

    public void ResetAtkSpeed()
    {
        animator.SetFloat("Speed", model.AttackSpeed);
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


    AudioClip hitSound;
    GameObject hurtEffect;
    Vector3 hurtEffectPos;
    public void TakeDamage(float damage, bool down, Vector3 target, string soundName, string effectName, Vector3 effectPos, int modelNumber)
    {
        
        
       // photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllViaServer, damage, down, target);
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, damage, down, target, soundName, effectName, effectPos , modelNumber);


    }


    [PunRPC]
    public void TakeDamageRPC(float damage, bool down, Vector3 target, string soundName, string effectName, Vector3 effectPos, int modelNumber)
    {
        if (photonView.IsMine == false)
            return;

        if (model.HP <= 0)
        {
            Debug.Log("PlayerDeath");
        }

        if (playerHurtbox.layer == (int)LayerEnum.DISABLE_BOX)
        {
            Debug.Log("DODGE!");
            hitSound = null;
            hurtEffect = null;
            hurtEffectPos = Vector3.zero;
            return;
        }
        else
        {
            hitSound = AudioManager.GetInstance().GetMonsterSoundDic(modelNumber, soundName);
            hurtEffect = EffectManager.GetInstance().GetEffectDic(effectName);
            hurtEffectPos = effectPos;
        }
        if (hitSound != null)
        {
            AudioManager.GetInstance().PlaySound(hitSound);
           // audioSource.PlayOneShot(hitSound);
            if (hurtEffectPos.y < 1f)
            {
                hurtEffectPos.y += 1.5f;
            }

            Instantiate(hurtEffect, hurtEffectPos, transform.rotation);

        }


        photonView.RPC(nameof(TakeDamageResult), RpcTarget.All, damage, down, target);



    }

    [PunRPC]
    void TakeDamageResult(float damage, bool down, Vector3 target)
    {
        transform.LookAt(target);

        isFixed = false;
        Debug.Log("OUCH!!!!!!!!!!!!!!" + damage);
        model.HP -= damage;

        if (model.HP <= 0f)
        {
            Dying();
            return;
        }


        if (down)
        {
            ChangeState(PlayerState.Down, true);

            animator.SetTrigger(animatorParameterHash[(int)PlayerAnimationHashNumber.Down]);

        }
        else
        {

            ChangeState(PlayerState.Hit, true);

            animator.SetTrigger(animatorParameterHash[(int)PlayerAnimationHashNumber.Hit]);

        }
    }

    void Dying()
    {

        ChangeState(PlayerState.Dead, true);
        animator.Play("Death");
        playerHurtbox.layer = (int)LayerEnum.DISABLE_BOX;
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;
        rigid.velocity = Vector3.zero;
    }

    

    public void Revive()
    {
        if(photonView.IsMine)
        inventory.UpgradePotion();

       
        if(model.HP <= 0f)
        {
            animator.Play("StandUp");
        }

        if(model.HP + model.MaxHP * 0.5f > model.MaxHP)
        {
            model.HP = model.MaxHP;
        }
        else
        {

            model.HP += model.MaxHP * 0.5f;
        }


      
        playerHurtbox.layer = (int)LayerEnum.PLAYER_HURT_BOX;
        gameObject.layer = (int)LayerEnum.PLAYER;
    }

    // 애니메이션 프리징 예외 처리

    public int freezingCnt = 0;

    public void FreezingCheck()
    {
        if(curState != PlayerState.Wait)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Wait"))
            {
                freezingCnt++;
                if (freezingCnt > 5)
                {
                    FreezingOut();
                }
            }
            
        }
        
    }



    public float dodgeFreezingCnt = 0;

    Coroutine freezingCheckCoroutine;
    WaitForSeconds checkTime = new WaitForSeconds(0.1f);

    IEnumerator DodgeFreezingCheck()
    {
        while (true)
        {
            yield return checkTime;
            if (curState == PlayerState.Dodge)
            {
                dodgeFreezingCnt += 0.1f;

                if (dodgeFreezingCnt > 2)
                {
                    FreezingOut();


                }

            }
            else
            {
                dodgeFreezingCnt = 0;
            }
        }
    }

    void FreezingOut()
    {
        ResetAtkSpeed();
        
        isFixed = false;
        ChangeState(PlayerState.Wait, false);
    }

}
