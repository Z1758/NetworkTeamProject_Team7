using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureSet : MonoBehaviourPun
{

    [SerializeField] FractureObject[] bindObj;
    [SerializeField] float destroyTime = 3.0f;
    [SerializeField] int hp;

    private void Awake()
    {
        bindObj = GetComponentsInChildren<FractureObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hp <= 0)
        {
            return;
        }
       

        if ((other.CompareTag("Enemy") && photonView.IsMine) )
        {

            if (HitboxCheck(other) == false)
            {
                return;
            }
            
           
            hp = 0;

         
        }else if (other.CompareTag("Hitbox"))
        {

            if (HitboxCheck(other) == false)
            {
                return;
            }


            hp--;
        }


        if (hp <= 0)
        {

            foreach (var obj in bindObj)
            {

                obj.CalledBindObj();
            }
            bindObj = null;
            Destroy(gameObject, destroyTime);
        }
    }

    bool HitboxCheck(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {
            if (hitbox.CheckFOVType())
            {
                return false;
            }
            hitbox.HitEffect(other.ClosestPoint(transform.position));


        }

        return true;
    }
     
}
