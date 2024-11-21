using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    // 역경직 테스트
    [SerializeField] Animator animator;

    [SerializeField] StatusModel model;
    [SerializeField] bool down;
    [SerializeField] bool notFriction;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] AudioClip hitSound;
    [SerializeField] string soundName;

    WaitForSeconds atkSlow= new WaitForSeconds(0.2f);

    private void Awake()
    {
        if (model == null)
            model = GetComponentInParent<StatusModel>();

      
    }


    public void HitEffect()
    {
        Instantiate(effectPrefab, transform.position, transform.rotation);



        // 역경직 테스트
        if (notFriction == false)
        {
            animator.SetFloat("Speed", model.AttackSpeed - 0.9f );
        
            StartCoroutine(SlowSpeed());
        }

        
    }

    public AudioClip GetSoundEffect()
    {
        if(soundName == "")
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
        return model.Attack;
    }
    public bool GetDown()
    {
        return down;
    }

    public void ChangeLayer()
    {
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;
    }
}
