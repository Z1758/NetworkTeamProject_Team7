using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public enum ModelType { PLAYER, ENEMY }
public class StatusModel : MonoBehaviourPun, IPunObservable
{

    [SerializeField] int characterNumber;
    [SerializeField] ModelType type;
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;
    [SerializeField] private float attack;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float moveSpeed;

    public int CharacterNumber { get { return characterNumber; } }
    public float HP { get { return hp; } set { hp = value; OnChangedHpEvent?.Invoke(hp); } }

    public float MaxHP { get { return maxHP; } }
    public float Attack { get { return attack; } }
    public float AttackSpeed { get { return attackSpeed; } }
    public float MoveSpeed { get { return moveSpeed; } }

    public ModelType ModelType { get { return type; } }
    public UnityAction<float> OnChangedHpEvent;

    private void OnDisable()
    {
        OnChangedHpEvent = null;
    }

    private void Start()
    {
        OnChangedHpEvent = null;
            switch (type) {
                case ModelType.PLAYER:
                    if (photonView.IsMine)
                    {
                        GameObject.Find("PlayerHPSlider").GetComponent<HPView>().SetModel(this);
                    }
                    break;
                case ModelType.ENEMY:
                    GameObject.Find("MonsterHPSlider").GetComponent<HPView>().SetModel(this);
                    break;

            }
       
        
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);



        }

        else if (stream.IsReading)
        {

            hp = (float)stream.ReceiveNext();


        }
    }
}
