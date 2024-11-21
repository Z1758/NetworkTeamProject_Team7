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
        if (monster == null)
            monster = GetComponentInParent<MonsterController>();


    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Hitbox hitbox))
        {
            //hitbox.ChangeLayer();
            hitbox.HitEffect();
            monster.TakeDamage(hitbox.GetAtk(), hitbox.GetSoundEffect());
        }

    }

}
