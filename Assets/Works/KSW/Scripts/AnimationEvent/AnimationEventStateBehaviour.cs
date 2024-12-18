using System;
using UnityEngine;
[Flags]
public enum AnimationType
{

    HITBOX = 1 << 0,
    HURTBOX = 1 << 1,
    MOVE = 1 << 2,
    AUDIO = 1 << 3,
    COLLIDER_RESET = 1 << 4, 
 

}
public class AnimationEventStateBehaviour : StateMachineBehaviour
{
    public string eventName;

    [Range(0f, 1f)] public float triggerTime;

    [SerializeField] AnimationType animationType;
    bool hasTriggered;
    bool hasLoopTriggered;
    public AudioClip clip;
    public float moveVelocity;
    public int colliderNum;
    public bool colliderActive;
    public bool isLoop;

    AnimationEventReceiver receiver;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        hasTriggered = false;
        

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float currentTime = stateInfo.normalizedTime % 1f;
        if (isLoop && currentTime < 0.03f)
        {
            hasLoopTriggered = true;
        }

        if (!hasTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator);
            hasTriggered = true;
           
        }else if (hasLoopTriggered && currentTime >= triggerTime)
        {
            NotifyReceiver(animator);
            hasLoopTriggered = false;
       

        }

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      
        /*
         if (animationType.HasFlag(AnimationType.END))
         {

             NotifyReceiver(animator);
             receiver.ResetColider();
         }
         */
    }

    void NotifyReceiver(Animator animator)
    {
       

        if (receiver == null)
        {
            receiver = animator.GetComponent<AnimationEventReceiver>();
        }

        if (receiver != null)
        {
            receiver.OnAnimationEventTriggered(eventName);



            if (animationType.HasFlag(AnimationType.HITBOX))
            {
                receiver.ActiveHitboxAnimation(colliderNum, colliderActive);
            }
            if (animationType.HasFlag(AnimationType.HURTBOX))
            {
                receiver.ActiveHurtboxAnimation(colliderActive);
            }
            if (animationType.HasFlag(AnimationType.AUDIO))
            {
                receiver.PlaySound(clip);
            }
            if (animationType.HasFlag(AnimationType.MOVE))
            {
                receiver.ControllMoveAnimation(moveVelocity);
            }
            if (animationType.HasFlag(AnimationType.COLLIDER_RESET))
            {
                receiver.ResetColider();
            }
            
        }
    }
}
