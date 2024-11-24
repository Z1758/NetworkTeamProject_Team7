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



public class Hitbox : MonoBehaviour
{
    [Header("필수 컴포넌트")]
    [SerializeField] Animator animator;
    [SerializeField] StatusModel model;


    [SerializeField] HitboxType hitboxType;
   
    [SerializeField] GameObject effectPrefab;
    [SerializeField] string effectName;
    [SerializeField] AudioClip hitSound;
    [SerializeField] string soundName;

    [Header("FOV")]
    [SerializeField] public float radius;
    [SerializeField] public float angle;

    //역경직 시간
    WaitForSeconds atkSlow = new WaitForSeconds(0.2f);

    private void Awake()
    {
        if (model == null)
            model = GetComponentInParent<StatusModel>();


    }


    public void HitEffect(Vector3 vec)
    {
        if (effectPrefab == null)
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
        if (effectPrefab == null)
        {
            effectPrefab = EffectManager.GetInstance().GetEffectDic(effectName);
        }

        return effectPrefab;


    }

    public void AttackFriction()
    {
        // 역경직 테스트
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
        if (hitSound == null)
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

    // 역경직 테스트
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


            return model.Attack;
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
        if (hitboxType.HasFlag(HitboxType.FOV_ATTACK))
        {
            Vector3 target = (hit.transform.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, target) < angle / 2)
            {
               //  float distance = Vector3.Distance(transform.position, target);

               // Debug.DrawRay(transform.position + Vector3.up, target * distance, Color.red, 1.0f);


            }
            else
            {
                return false;
            }
        }

        return true;
    }

    // 에디터 그리기용
    public Vector3 DirFromAngle(float angle, bool global)
    {

        angle += transform.eulerAngles.y;



        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));

    }
}
