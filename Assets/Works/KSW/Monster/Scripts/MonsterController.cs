using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[System.Serializable]
public class MonsterPattern
{
    public string pattern;
    public float range;
}


public class MonsterController : MonoBehaviourPun, IPunObservable
{
    [Header("필수 컴포넌트")]
    [SerializeField] Animator animator;
    List<PlayerController> pc_s;
    [SerializeField] Rigidbody rigid;
    [SerializeField] StatusModel model;
    [SerializeField] MonsterHurtBox monsterHurtBox;

    [Header("상태")]
    [SerializeField] bool isFixed;
    [SerializeField] bool isMoveAni;

    [SerializeField] bool isDie;
    [SerializeField] bool isFurious;

    [Header("동기화")]
    [SerializeField] float lag;
    [SerializeField] float time;
    [SerializeField] float aniStateTime;

    // 타겟
    Transform target;

    [Header("몬스터 패턴")]
    [SerializeField] MonsterPattern[] patterns;
    [SerializeField] int nextPattern;

    [Header("광폭화 마테리얼")]
    [SerializeField] SkinnedMeshRenderer furiousRenderer;


    //   [Header("애니메이션 해싱")]
     int[] animtionHash;

    int waitHash;
    int runHash;
    int readyHash;
    int[] animatorParameterHash;

    int runParameterHash;
    int waitParameterHash;
    int atkEndParameterHash;

    int currentHash;

    // 코루틴 캐싱
    WaitForSeconds cooldown = new WaitForSeconds(0.5f);
    Coroutine cooldownCoroutine;

    WaitForSeconds lagWFS = new WaitForSeconds(0.08f);

    private void Awake()
    {
       
    }

    private void SetStatus()
    {
       int stage = GameScene.Instance.currentStage - 1;
      //  int stage = TestGameScene.Instance.currentStage - 1;
        model.MaxHP = model.MaxHP + (model.MaxHP * 1f * stage);
        model.HP = model.MaxHP;
        model.Attack = model.Attack + (model.Attack * 0.4f * stage);
        model.MoveSpeed = model.MoveSpeed + (model.MoveSpeed * 0.05f * stage);

        model.AttackSpeed = model.AttackSpeed + (0.05f * stage);

        animator.speed = model.AttackSpeed;

        Debug.Log("디버그ㅡㅡㅡㅡㅡㅡㅡㅡㅡㅡ"+GameScene.Instance.currentStage);
        Debug.Log("몬스터 스탯 세팅");
    }

    private void SetComponent()
    {
        animator = GetComponent<Animator>();
        pc_s = new List<PlayerController>();
        rigid = GetComponent<Rigidbody>();
        monsterHurtBox = GetComponentInChildren<MonsterHurtBox>();
        model = GetComponent<StatusModel>();
    }

    private void SetAniHash()
    {
           animtionHash = new int[patterns.Length];
        animatorParameterHash = new int[patterns.Length];

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < patterns.Length; i++)
        {
            stringBuilder.Clear();
            stringBuilder.Append("Base Layer.");
            stringBuilder.Append(patterns[i].pattern);
                  animtionHash[i] = Animator.StringToHash(stringBuilder.ToString());

            AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];
            animatorParameterHash[i] = animatorControllerParameter.nameHash;
        }

        for (int i = 0; i < animator.parameterCount; i++)
        {

            AnimatorControllerParameter animatorControllerParameter = animator.parameters[i];

            if (animatorControllerParameter.name == "Run")
            {
                runParameterHash = animatorControllerParameter.nameHash;
            }
            if (animatorControllerParameter.name == "Wait")
            {
                waitParameterHash = animatorControllerParameter.nameHash;
            }
            if (animatorControllerParameter.name == "AtkEnd")
            {
                atkEndParameterHash = animatorControllerParameter.nameHash;
            }

        }

        waitHash = Animator.StringToHash("Base Layer.Wait");
        runHash = Animator.StringToHash("Base Layer.Run");
        readyHash = Animator.StringToHash("Base Layer.ReadyAction");

    }

    public void FindPlayers()
    {
        pc_s.Clear();
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject obj in objects)
        {
            if (obj.layer != (int)LayerEnum.DISABLE_BOX)
            {
                Debug.Log(obj.name);
                pc_s.Add(obj.GetComponent<PlayerController>());

            }
        }

        //게임오버시
        if (pc_s.Count == 0)
        {
            GameScene.Instance.GameOverResult();
        }
        TargetChange();


    }

    public void TargetChange()
    {
        if (photonView.IsMine == false)
            return;
        int ran = Random.Range(0, pc_s.Count);
        if (pc_s.Count == 0)
            return;
        if (!pc_s[ran])
        {
            return;
        }
        else if (pc_s[ran].gameObject.layer == (int)LayerEnum.DISABLE_BOX)
        {
            target = null;
            FindPlayers();
        }
        else
        {
            target = pc_s[ran].transform;
        }


    }

    private void Start()
    {

        SetComponent();

        FindPlayers();

        SetAniHash();
        SetStatus();


        animator.speed = model.AttackSpeed;

        SetNextPattern();
        if (photonView.IsMine == false)
            StartCoroutine(CheckAniLag());



    }

    private void Update()
    {
        if (Time.timeScale < 0.1)
            return;
        SetAniTime();

        if (isDie)
        {
            StopAllCoroutines();
            SynchronizationDeath();
            monsterHurtBox.gameObject.layer = (int)LayerEnum.DISABLE_BOX;
            rigid.velocity = Vector3.zero;
            return;
        }
        if (model.HP <= 0)
        {
            Dying();
        }

        TraceMonster();



    }

    IEnumerator CheckAniLag()
    {
        while (true)
        {
            yield return lagWFS;
            SynchronizationChechk();
            /*
                        else if (Mathf.Abs(lag) > 0.05f)
                        {
                            Debug.Log("렉 발생");
                            for (int i = 0; i < patterns.Length; i++)
                            {


                                if (animator.GetCurrentAnimatorStateInfo(0).IsName(patterns[i].pattern) )
                                {
                                    animator.Play(animtionHash[i], 0, aniStateTime);
                                }else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Furious"))
                                {
                                    animator.Play("Furious", 0, aniStateTime);
                                }

                            }

                        }*/


        }
    }

    public void Synchronization()
    {
          
            
            animator.Play(currentHash, 0, aniStateTime);
            
        

    }
    public void SynchronizationDeath()
    {
        if (currentHash != animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            Debug.Log("사망 동기화");

            animator.Play("Death");

        }

    }
    public void SynchronizationChechk()
    {
        /*
        if(Mathf.Abs(lag) > 0.5f )
        {
            if (currentHash != animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
            {
                if(animator.GetCurrentAnimatorStateInfo(0).fullPathHash == furiHash)
                {

                }
                if (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == readyHash)
                {

                }
              

            }
         //  Synchronization();
        }
        else if (Mathf.Abs(lag) > 0.06f && currentHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
            Synchronization();
        }*/

     
        if (runHash == currentHash || waitHash == currentHash )
        {
            return;
        }



        if (Mathf.Abs(lag) > 0.07f && currentHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {

            Debug.Log($"{lag} 동기화1");
            Synchronization();
        }
        else if (Mathf.Abs(lag) > 0.3f && currentHash != animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
        {
         
            for (int i = 0; i < patterns.Length; i++)
            {


                if (animtionHash[i] == currentHash)
                {
                    if (readyHash == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
                    {
                        Debug.Log("레디 액션 동기화");
                    
                    }
                    else
                    {
                        Debug.Log("공격 동기화 겹침!");
                        return;
                    }

                  
                }
 
            }

            Debug.Log($"{lag} 동기화2");
            Synchronization();
        }

    }
    public void SetAniTime()
    {

        if (photonView.IsMine == false)
        {
            time = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            lag = time - aniStateTime;
            return;
        }
        time = 0;
        lag = 0;
        aniStateTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
      
    }

    public void TraceMonster()
    {

        if (isFixed)
        {
            if (isMoveAni)
            {
                return;
            }
            rigid.velocity = Vector3.zero;
            return;
        }

        // 광폭화 체크
        if (model.HP <= model.MaxHP * 0.5 && isFurious == false)
        {
            Furious();
            return;
        }

        // 소유권자가 아니면 리턴
        if (photonView.IsMine == false)
            return;



        // 타겟 찾기
        if (!target)
        {
            FindPlayers();
            return;
        }


        // 범위 체크
        if (MonsterCheckRange())
        {
            return;
        }
        // 몬스터 이동
        MonsterMove();
    }

    private void MonsterMove()
    {
        animator.SetBool(runParameterHash, true);
        animator.SetBool(waitParameterHash, false);
        transform.LookAt(target);
        rigid.velocity = transform.forward * model.MoveSpeed;
    }

    private bool MonsterCheckRange()
    {
        if ((target.position - transform.position).sqrMagnitude < patterns[nextPattern].range)
        {
            rigid.velocity = Vector3.zero;


            animator.SetBool(runParameterHash, false);
            animator.SetBool(waitParameterHash, true);
            MonsterAttack();

            return true;
        }
        return false;
    }

    public void MonsterAttack()
    {

        transform.LookAt(target);
        isFixed = true;



        animator.SetBool(animatorParameterHash[nextPattern], true);

        animator.SetBool(atkEndParameterHash, false);

        SetNextPattern();
    }

    public void SetNextPattern()
    {
        for (int i = 0; i < patterns.Length; i++)
        {
            int ran = Random.Range(0, patterns.Length);
            if (nextPattern != ran)
            {
                nextPattern = ran;
                return;
            }
        }
        nextPattern = 0;
    }

    public void PatternReset()
    {
        if (cooldownCoroutine != null)
        {
            StopCoroutine(cooldownCoroutine);
        }

        cooldownCoroutine = StartCoroutine(AttackCooldown());


        for (int i = 0; i < animatorParameterHash.Length; i++)
        {

            animator.SetBool(animatorParameterHash[i], false);
        }

        animator.SetBool(atkEndParameterHash, true);

        TargetChange();
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

    IEnumerator AttackCooldown()
    {
        yield return cooldown;
        isFixed = false;
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFixed);
            stream.SendNext(aniStateTime);
            stream.SendNext(currentHash);

        }

        else if (stream.IsReading)
        {

            isFixed = (bool)stream.ReceiveNext();
            aniStateTime = (float)stream.ReceiveNext();
            currentHash = (int)stream.ReceiveNext();
        }

    }

    public void TakeDamage(float damage, AudioClip clip)
    {
        if (clip)
        {
            AudioManager.GetInstance().PlaySound(clip);
            // audioSource.PlayOneShot(clip);
        }
        //  photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllViaServer, damage);
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.All, damage);
    }
    [PunRPC]
    public void TakeDamageRPC(float damage)
    {
        if (model.HP <= 0)
        {
            Debug.Log("MonsterDeath");
            return;
        }

        Debug.Log("HIT!!!!!!!!!!!!!!" + damage);
        model.HP -= (float)damage;


    }


    private void Dying()
    {
        StopAllCoroutines();
        animator.speed = 1.0f;
        animator.Play("Death");
        PatternReset();
        isDie = true;
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;

        rigid.velocity = Vector3.zero;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");



        foreach (GameObject obj in objects)
        {
            obj.GetComponent<PlayerController>().Revive();


        }


        //임시
       // TestGameScene.Instance.ClearBoss(gameObject);

        GameScene.Instance.ClearBoss(gameObject);
    }



    public void Furious()
    {
        if (isDie)
            return;

        foreach (Material mat in furiousRenderer.materials)
        {
            mat.EnableKeyword("_EMISSION");
        }

        isFixed = true;
        isFurious = true;
        rigid.velocity = Vector3.zero;
        animator.Play("Furious");
        animator.SetBool(atkEndParameterHash, false);
        model.AttackSpeed += 0.2f;
        model.MoveSpeed += model.MoveSpeed * 0.3f;
        animator.speed = model.AttackSpeed;
       
    }
}
