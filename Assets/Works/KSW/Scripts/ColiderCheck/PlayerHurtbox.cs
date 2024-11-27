using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurtbox : MonoBehaviourPun, IPunObservable
{
    [SerializeField] PlayerController pc;
  
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(gameObject.layer);



        }

        else if (stream.IsReading)
        {

            gameObject.layer = (int)stream.ReceiveNext();


        }
    }

    private void Awake()
    {
        if(!pc)
             pc = GetComponentInParent<PlayerController>();

        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {

            if (hitbox.gameObject.layer == (int)LayerEnum.OTHER_CLIENT_MONSTER_COLLIDER)
                return;
            if (!hitbox.GetAngleHit(transform))
            {
                return;
            }

            Vector3 target = other.transform.position;
            target.y = 0;



            pc.TakeDamage(hitbox.GetAtk(), hitbox.GetDown(), target, hitbox.GetSoundEffect(), hitbox.HitEffect(), other.ClosestPoint(transform.position));
        }

    }

}
