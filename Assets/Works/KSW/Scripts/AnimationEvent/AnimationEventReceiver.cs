
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;



public class AnimationEventReceiver : MonoBehaviourPun
{
    [SerializeField] int characterNumber;

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
    [SerializeField] List<AnimationEvent> animationEvents = new();


    private void Awake()
    {
        //  hitboxLayer = Mathf.RoundToInt(Mathf.Log(hitboxLayerMask.value, 2));
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
        AudioClip clip = AudioManager.GetInstance().GetMonsterVoiceDic(characterNumber ,str);
        audioSource.PlayOneShot(clip);
    }


}
