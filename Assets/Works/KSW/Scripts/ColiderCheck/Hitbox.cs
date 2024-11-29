using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;


[Flags]
public enum HitboxType
{

    DOWN_ATTACK = 1 << 0,
    NOT_FRICTION_ATTACK = 1 << 1,
    FOV_ATTACK = 1 << 2,
    ONCE_HIT = 1 << 3
}



public class Hitbox : MonoBehaviourPun
{
    [Header("�ʼ� ������Ʈ")]
    [SerializeField] Animator animator;
    [SerializeField] protected StatusModel model;

    [Header("��Ʈ�ڽ� Ÿ��")]
    [SerializeField] HitboxType hitboxType;

    [Header("���ݷ� ����")]
    [SerializeField] float multiplier = 1.0f;

    protected GameObject criticalEffectPrefab;
    protected AudioClip criticalSound;

    protected GameObject effectPrefab;
    [SerializeField] protected string effectName;
    AudioClip hitSound;
    [SerializeField] string soundName;

    [Header("FOV")]
    [SerializeField] public float radius;
    [SerializeField] public float angle;

    //������ �ð�
    WaitForSeconds atkSlow = new WaitForSeconds(0.2f);

    private void Awake()
    {
        if (!model)
            model = GetComponentInParent<StatusModel>();

        if (model.ModelType == ModelType.PLAYER)
        {
            criticalEffectPrefab = EffectManager.GetInstance().GetEffectDic("CriticalEffect");
            criticalSound = AudioManager.GetInstance().GetCommonSoundDic("Critical");
        }
    }


    public virtual void HitEffect(Vector3 vec)
    {
        if (effectName == "")
            return;

        if (effectPrefab is null)
        {
            effectPrefab = EffectManager.GetInstance().GetEffectDic(effectName);
        }
        if (vec.y < 1f)
        {
            vec.y += 1.5f;
        }

        Instantiate(effectPrefab, vec, transform.rotation);

    }

    public GameObject HitEffect()
    {
        if (effectName == "")
            return null;

        if (effectPrefab is null)
        {
            effectPrefab = EffectManager.GetInstance().GetEffectDic(effectName);
        }

        return effectPrefab;


    }

    public void AttackFriction()
    {
        // ������ �׽�Ʈ
        if (hitboxType.HasFlag(HitboxType.NOT_FRICTION_ATTACK))
        {
            return;
        }

        animator.SetFloat("Speed", model.AttackSpeed - 0.9f);

        StartCoroutine(SlowSpeed());
    }



    public AudioClip GetSoundEffect()
    {
       
        if (soundName == "")
        {
           
            return null;
        }
        if (hitSound is null)
        {
            if (model.ModelType == ModelType.PLAYER)
            {
                hitSound = AudioManager.GetInstance().GetPlayerSoundDic(model.CharacterNumber, soundName);
            }
            else if (model.ModelType == ModelType.ENEMY)
            {
                hitSound = AudioManager.GetInstance().GetMonsterSoundDic(model.CharacterNumber, soundName);
            }
         
        }

        return hitSound;
    }

    // ������ �׽�Ʈ
    IEnumerator SlowSpeed()
    {
        yield return atkSlow;
        animator.SetFloat("Speed", model.AttackSpeed);
    }

    public float GetAtk()
    {
        if (hitboxType.HasFlag(HitboxType.ONCE_HIT))
        {
            ChangeLayer();
        }

            return model.Attack * multiplier;
    }
    public float GetAtk(Vector3 vec)
    {
        if (hitboxType.HasFlag(HitboxType.ONCE_HIT))
        {
            ChangeLayer();
        }

        if (model.CriticalRate >= UnityEngine.Random.Range(1, 101))
        {

           
            if (vec.y < 1f)
            {
                vec.y += 1.5f;
            }

            Instantiate(criticalEffectPrefab, vec, transform.rotation);

            AudioManager.GetInstance().PlaySound(criticalSound);

            return (model.Attack +  (model.Attack* model.CriticalDamageRate)) * multiplier;
        }



        return model.Attack * multiplier;
    }


    public bool GetDown()
    {
        return hitboxType.HasFlag(HitboxType.DOWN_ATTACK);
    }

    public void ChangeLayer()
    {
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;
    }

    public bool GetAngleHit(Transform hit)
    {
        if (CheckFOVType())
        {
            Vector3 target = (hit.transform.position - transform.position).normalized;
           
            if (Vector3.Angle(transform.forward, target) < angle / 2)
            {
 
                 float distance = Vector3.Distance(transform.position, target);

                Debug.DrawRay(transform.position + Vector3.up, target * distance, Color.red, 1.0f);


            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public bool CheckFOVType()
    {
        return hitboxType.HasFlag(HitboxType.FOV_ATTACK);
    }

    // ������ �׸����
    public Vector3 DirFromAngle(float angle, bool global)
    {

        angle += transform.eulerAngles.y;



        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));

    }
}
