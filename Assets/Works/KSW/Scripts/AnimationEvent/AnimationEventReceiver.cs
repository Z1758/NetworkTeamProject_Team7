
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;



public class AnimationEventReceiver : MonoBehaviourPun
{
    [SerializeField] StatusModel model;
  
    [SerializeField] Transform objectTransform;

    [SerializeField] LayerEnum hitboxLayerEnum;
    [SerializeField] LayerEnum hurtboxLayerEnum;
    //  [SerializeField] LayerMask hitboxLayerMask;


    int hitboxLayer;
    int hurtboxLayer ;
    int colliderDisableLayer = (int)LayerEnum.DISABLE_BOX;


    [SerializeField] AudioSource audioSource;

    [SerializeField] Rigidbody rigid;

    [SerializeField] GameObject[] projectiles;
    [SerializeField] GameObject[] hitboxes;
    [SerializeField] GameObject hurtbox;
    [SerializeField] ParticleSystem[] effects;
    [SerializeField] List<AnimationEvent> animationEvents = new();
    [SerializeField] LayerMask aoeRayMask;

    private void Awake()
    {
        //  hitboxLayer = Mathf.RoundToInt(Mathf.Log(hitboxLayerMask.value, 2));
      
        audioSource = GetComponent<AudioSource>();
        hitboxLayer = (int)hitboxLayerEnum;
        hurtboxLayer = (int)hurtboxLayerEnum;
    }

    public void OnAnimationEventTriggered(string eventName)
    {
      
        AnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);

        matchingEvent?.OnAnimationEvent?.Invoke();

    }

    public void ControllMoveAnimation(float speed)
    {
        rigid.velocity = objectTransform.forward * speed;

    }

    public void ActiveHitboxAnimation(int num, bool active)
    {
        if (photonView.IsMine == false)
            return;

        hitboxes[num].layer = active ? hitboxLayer : colliderDisableLayer;

    }

    public void ActiveHurtboxAnimation(bool active)
    {
        if (photonView.IsMine == false)
            return;

        hurtbox.layer = active ? hurtboxLayer : colliderDisableLayer;

    }

    public void ActiveProjectileAnimation(int num)
    {
        projectiles[num].SetActive(true);
    }

    public void ResetColider()
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.layer = colliderDisableLayer;
        }
    }

    public void PlaySound(string str)
    {
        AudioClip clip = null;
        if (model.ModelType == ModelType.PLAYER)
        {
            clip = AudioManager.GetInstance().GetPlayerSoundDic(model.CharacterNumber, str);
        }
        else if (model.ModelType == ModelType.ENEMY)
        {
            clip = AudioManager.GetInstance().GetMonsterSoundDic(model.CharacterNumber, str);
        }
        audioSource.PlayOneShot(clip);
    }

    public void PlayCommonSound(string str)
    {
        AudioClip clip = null;
        clip = AudioManager.GetInstance().GetCommonSoundDic( str);
        audioSource.PlayOneShot(clip);
    }

    public void ActiveEffect(int num)
    {
        effects[num].gameObject.SetActive(true);
        effects[num].Play();
     //   effects[num].SetActive(true);
    }

    public void AOERayCast(int num)
    {
 
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up*2, transform.parent.forward, out hit, 20f, aoeRayMask))
        {
            if (hit.collider.tag == "Enemy" || hit.collider.tag == "Player")
            {
                projectiles[num].transform.position = hit.transform.position;
            }
            else
            {
                projectiles[num].transform.position = hit.point;
            }
            Debug.Log(hit.collider.name);
        }
        else
        {
            projectiles[num].transform.position = transform.position;
            projectiles[num].transform.rotation = transform.parent.rotation;
            projectiles[num].transform.Translate(Vector3.forward* 20);
        }
    }
}
