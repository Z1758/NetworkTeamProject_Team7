
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputEventTrace;
using UnityEngine.InputSystem.XR;
using System.Runtime.CompilerServices;

public abstract class State
{
    protected PlayerController controller;

    public State(PlayerController controller)
    {
        this.controller = controller;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();

    public abstract void Dispose();
}




public class WaitState : State
{


    public WaitState(PlayerController controller) : base(controller)
    {
        
    }

    public override void Dispose()
    {
      
    }

    public override void EnterState()
    {
        for (int i = 0; i < controller.animatorParameterHash.Length; i++)
        {
            controller.animator.SetBool(controller.animatorParameterHash[i], false);
        }
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Wait] , true);

    }

    public override void ExitState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Wait], false);
    }

    public override void UpdateState()
    {
        controller.rigid.velocity = Vector3.zero;


    }
}

public class RunState : State
{
    public RunState(PlayerController controller) : base(controller)
    {

    }


    public override void EnterState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Run], true);
    }

    public override void ExitState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Run], false);
    }

    public override void UpdateState()
    {
        controller.Move();
       
    }


    public override void Dispose()
    {
       
    }
}

public class AttackState : State
{
   
    public AttackState(PlayerController controller) : base(controller)
    {
        

    }


    public override void EnterState()
    {

       
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Atk], true);


    }

    public override void ExitState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Atk], false);
    }


    public override void UpdateState()
    {

    }


    public override void Dispose()
    {
       
    }
}

public class SkillState : State
{



    public SkillState(PlayerController controller) : base(controller)
    {


    }


    public override void EnterState()
    {

        controller.model.SetCurrentSkillCoolTime(controller.skillNumber, controller.model.SkillCoolTime[controller.skillNumber]);
        controller.animator.SetBool(controller.skillNumberHash, true);


    }

    public override void ExitState()
    {
        for (int i = 0; i < controller.animatorParameterHash.Length; i++)
        {
            controller.animator.SetBool(controller.animatorParameterHash[i], false);
        }
    }


    public override void UpdateState()
    {

    }


    public override void Dispose()
    {

    }
}

public class InputWaitState : State
{
   
    public InputWaitState(PlayerController controller) : base(controller)
    {
 
    }

 

    public override void EnterState()
    {
       

    }

    public override void ExitState()
    {
       
    }


    public override void UpdateState()
    {


    }



    public override void Dispose()
    {
        
    }
}
public class DodgeState : State
{


    public DodgeState(PlayerController controller) : base(controller)
    {
    }

    public override void EnterState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Dodge], true);
        controller.model.Stamina -= controller.model.ConsumStamina;

    }

    public override void ExitState()
    {
        controller.animator.SetBool(controller.animatorParameterHash[(int)PlayerAnimationHashNumber.Dodge], false);
    }

    public override void UpdateState()
    {

    }


    public override void Dispose()
    {

    }

}
public class DownState : State
{


    public DownState(PlayerController controller) : base(controller)
    {
    }

    public override void EnterState()
    {
     
    }

    public override void ExitState()
    {
       
    }

    public override void UpdateState()
    {
      
    }


    public override void Dispose()
    {

    }

}
public class HitState : State
{


    public HitState(PlayerController controller) : base(controller)
    {
    }

    public override void EnterState()
    {
     
    }

    public override void ExitState()
    {
      
    }

    public override void UpdateState()
    {
        
    }


    public override void Dispose()
    {

    }

}
public class DeadState : State
{

   
    public DeadState(PlayerController controller) : base(controller)
    {
    }

    public override void EnterState()
    {
    
    }

    public override void ExitState()
    {

    }

    public override void UpdateState()
    {

    }


    public override void Dispose()
    {
     
    }

}

