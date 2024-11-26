using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterHurtBox : MonoBehaviour
{
    [SerializeField] MonsterController monster;

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
        if (monster)
            monster = GetComponentInParent<MonsterController>();


    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {
            //hitbox.ChangeLayer();

            if (hitbox.gameObject.layer == (int)LayerEnum.OTHER_CLIENT_PLAYER_COLLIDER)
                return;

            if (!hitbox.GetAngleHit(transform))
            {
                return;
            }
            hitbox.HitEffect(other.ClosestPoint(transform.position));
            hitbox.AttackFriction();

           
            
            monster.TakeDamage(hitbox.GetAtk(), hitbox.GetSoundEffect());
        }

    }

}
