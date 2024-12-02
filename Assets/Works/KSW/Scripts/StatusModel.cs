using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;



public enum ModelType { PLAYER, ENEMY }
public class StatusModel : MonoBehaviourPun, IPunObservable
{
    [Header("캐릭터 번호")]
    [SerializeField] int characterNumber;
    [Header("캐릭터 타입")]
    [SerializeField] ModelType type;
    [Header("체력")]
    [SerializeField] private float maxHP;
    [SerializeField] private float hp;
    [Header("스태미나")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float stamina;
    [Header("소비 스태미나")]
    [SerializeField] private float consumetamina;
    [Header("스태미나 회복력")]
    [SerializeField] private float recoveryStaminaMag;
    [Header("공격력")]
    [SerializeField] private float attack;
    [Header("공격 속도")]
    [SerializeField] private float attackSpeed;
    [Header("이동 속도")]
    [SerializeField] private float moveSpeed;
    [Header("치명타 확률")]
    [SerializeField] private float criticalRate;
    [Header("치명타 데미지 배율")]
    [SerializeField] private float criticalDamageRate;
    [Header("스킬 쿨타임")]
    [SerializeField] private float[] skillCoolTime;
    private float[] currentSkillCoolTime = new float[4];
    public int CharacterNumber { get { return characterNumber; } }
    public float HP { get { return hp; } set { hp = value; OnChangedHpEvent?.Invoke(hp); } }

    public float MaxHP { get { return maxHP; } set { maxHP = value; OnChangedMaxHpEvent?.Invoke(maxHP); OnChangedStatusEvent?.Invoke(); } }

    public float Stamina { get { return stamina; } set { stamina = value; OnChangedStaminaEvent?.Invoke(stamina); } }

    public float MaxStamina { get { return maxStamina; } set { maxStamina = value; OnChangedMaxStaminaEvent?.Invoke(maxStamina); OnChangedStatusEvent?.Invoke(); } }
    public float ConsumeStamina { get { return consumetamina; } set { consumetamina = value; OnChangedStatusEvent?.Invoke(); } }

    public float RecoveryStaminaMag { get { return recoveryStaminaMag; } set { recoveryStaminaMag = value; OnChangedStatusEvent?.Invoke();  } }

    public float Attack { get { return attack; } set { attack = value; OnChangedStatusEvent?.Invoke(); } }
    public float AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; OnChangedStatusEvent?.Invoke(); } }
    public float MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; OnChangedStatusEvent?.Invoke(); } }
    public float CriticalRate { get { return criticalRate; } set { criticalRate = value; OnChangedStatusEvent?.Invoke();  } }

    public float CriticalDamageRate { get { return criticalDamageRate; } set { criticalDamageRate = value; OnChangedStatusEvent?.Invoke(); } }
    public float[] SkillCoolTime { get { return skillCoolTime; } }

    public void SetSkillCoolTime(float value)
    {
        for (int i = 0; i < skillCoolTime.Length; i++)
        {
            skillCoolTime[i] = skillCoolTime[i] - (skillCoolTime[i] * value);

        }
    }

    public void SetCurrentSkillCoolTime(int num, float value)
    {
        currentSkillCoolTime[num] = value;
        OnChangedCoolTimeEvent?.Invoke(num, currentSkillCoolTime[num]);
    }
    public float GetCurrentSkillCoolTime(int num)
    {
        return currentSkillCoolTime[num];
    }

    public ModelType ModelType { get { return type; } }
    public UnityAction<float> OnChangedMaxHpEvent;
    public UnityAction<float> OnChangedHpEvent;
    public UnityAction<float> OnChangedStaminaEvent;
    public UnityAction<float> OnChangedMaxStaminaEvent;
    public UnityAction<int, float> OnChangedCoolTimeEvent;
    public UnityAction OnChangedStatusEvent;
    private void OnDisable()
    {
        OnChangedMaxHpEvent = null;
        OnChangedHpEvent = null;
        OnChangedCoolTimeEvent = null;
        OnChangedStaminaEvent = null;
        OnChangedMaxStaminaEvent = null;
        OnChangedStatusEvent = null;


    }

    private void Start()
    {
        OnChangedMaxHpEvent = null;
        OnChangedHpEvent = null;
        OnChangedCoolTimeEvent = null;
        OnChangedStaminaEvent = null;
        OnChangedMaxStaminaEvent = null;
        OnChangedStatusEvent = null;
        switch (type)
        {
            case ModelType.PLAYER:
                if (photonView.IsMine)
                {
                    GameObject.Find("PlayerHPSlider").GetComponent<HPView>().SetModel(this);
                    GameObject.Find("PlayerStaminaSlider").GetComponent<StaminaView>().SetModel(this);
                    GameObject.Find("SkillPanel").GetComponent<SkillView>().SetModel(this);
                    GameObject.Find("PlayerStatus").GetComponent<StatusView>().SetModel(this);
                  
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
