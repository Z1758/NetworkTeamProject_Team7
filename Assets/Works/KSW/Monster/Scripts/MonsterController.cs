using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MonsterPattern
{
    public string pattern;
    public float range;
}


public class MonsterController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] Animator animator;
    [SerializeField] List<PlayerController> pc_s;
    [SerializeField] Rigidbody rigid;
    [SerializeField] AudioSource audioSource;

    [SerializeField] StatusModel model;

    [SerializeField] public float atk;
    [SerializeField] float speed;
    [SerializeField] float range;

    [SerializeField] bool isFixed;
    [SerializeField] bool isMoveAni;
    [SerializeField] bool isDie; 

    [SerializeField] float lag;
    [SerializeField] float time;
    [SerializeField] float aniStateTime;

    Transform target;

    [SerializeField] MonsterPattern[] patterns;
    [SerializeField] int nextPattern;


    int[] animtionHash;
    [SerializeField] int[] animatorParameterHash;

    [SerializeField] int runParameterHash;
    [SerializeField] int waitParameterHash;
    [SerializeField] int atkEndParameterHash;


    WaitForSeconds cooldown = new WaitForSeconds(0.5f);
    Coroutine cooldownCoroutine;

    WaitForSeconds lagWFS = new WaitForSeconds(0.1f);

    private void Awake()
    {
        animator = GetComponent<Animator>();
        pc_s = new List<PlayerController>();
        rigid = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        model = GetComponent<StatusModel>();
        FindPlayers();

        animtionHash = new int[patterns.Length];
        animatorParameterHash = new int[patterns.Length];
        for (int i = 0; i < patterns.Length; i++)
        {
            animtionHash[i] = Animator.StringToHash(patterns[i].pattern);

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


        animator.speed = model.AttackSpeed;
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
        TargetChange();


    }

    public void TargetChange()
    {
        if (photonView.IsMine == false)
            return;
        int ran = Random.Range(0, pc_s.Count);
        if (pc_s.Count == 0)
            return;
        if (pc_s[ran] == null)
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
        SetNextPattern();
        if (photonView.IsMine == false)
            StartCoroutine(CheckAniLag());

    }

    private void Update()
    {

        if (isDie)
            return;

        TraceMonster();


        SetAniTime();
    }

    IEnumerator CheckAniLag()
    {
        while (true)
        {
            yield return lagWFS;




            if (Mathf.Abs(lag) > 0.5f)
            {

            }
            else if (Mathf.Abs(lag) > 0.05f)
            {
                Debug.Log("렉 발생");
                for (int i = 0; i < patterns.Length; i++)
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName(patterns[i].pattern))
                    {
                        animator.Play(animtionHash[i], 0, aniStateTime);
                    }

                }



            }




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

        if (photonView.IsMine == false)
            return;

        if (target == null)
        {
            FindPlayers();
            return;
        }



        if ((target.position - transform.position).sqrMagnitude < patterns[nextPattern].range)
        {
            rigid.velocity = Vector3.zero;


            animator.SetBool(runParameterHash, false);
            animator.SetBool(waitParameterHash, true);
            MonsterAttack();

            return;
        }


        animator.SetBool(runParameterHash, true);
        animator.SetBool(waitParameterHash, false);

        transform.LookAt(target);

        rigid.velocity = transform.forward * speed;

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
        nextPattern = Random.Range(0, patterns.Length);
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
        Debug.Log("패턴 끝");
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

        }

        else if (stream.IsReading)
        {

            isFixed = (bool)stream.ReceiveNext();
            aniStateTime = (float)stream.ReceiveNext();

        }

    }

    public void TakeDamage(float damage, AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
        photonView.RPC(nameof(TakeDamageRPC), RpcTarget.AllViaServer, damage);
    }
    [PunRPC]
    public void TakeDamageRPC(float damage)
    {
        if (model.HP <= 0)
        {
            Debug.Log("MonsterDeath");
        }

        Debug.Log("HIT!!!!!!!!!!!!!!" + damage);
        model.HP -= (float)damage;

        if (model.HP <= 0)
        {

            animator.Play("Death");
            PatternReset();
            isDie = true;
            gameObject.layer = (int)LayerEnum.DISABLE_BOX;
           
        }

    }
}
