
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


public class PlayerState
{
    private State playerState;

    public PlayerState(State defaultState)
    {
        playerState = defaultState;
    }

    public void SetState(State state)
    {
        playerState.ExitState();

        playerState = state;

        playerState.EnterState();
    }
    public State GetState()
    {
        return playerState;
    }
    public void Enter()
    {
        playerState.EnterState();
    }

    public void Update()
    {
        playerState.UpdateState();
    }


    public void Exit()
    {
        playerState.ExitState();
    }
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
        controller.animator.SetBool("Wait", true);

    }

    public override void ExitState()
    {
        controller.animator.SetBool("Wait", false);
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
        controller.animator.SetBool("Run", true);
    }

    public override void ExitState()
    {
        controller.animator.SetBool("Run", false);
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

       
        controller.animator.SetBool("Atk", true);


    }

    public override void ExitState()
    {
        controller.animator.SetBool("Atk", false);
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
        controller.animator.SetBool("Dodge", true);

    }

    public override void ExitState()
    {
        controller.animator.SetBool("Dodge", false);
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

