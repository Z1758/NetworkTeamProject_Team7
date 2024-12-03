using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class StatusView : MonoBehaviour
{
    [SerializeField] TMP_Text statusText;
    StringBuilder sb = new StringBuilder(); 

    [SerializeField]StatusModel statusModel;

    public void SetModel(StatusModel model)
    {
        statusModel = model;
        statusModel.OnChangedStatusEvent += ChangeStatus;

        ChangeStatus();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (statusModel)
        {
            statusModel.OnChangedStatusEvent += ChangeStatus;
            ChangeStatus();
        }
    }

    private void OnDisable()
    {
        if(statusModel)
            statusModel.OnChangedStatusEvent -= ChangeStatus;
    }

    public void ChangeStatus()
    {
        sb.Clear();

        sb.Append("공격력 : ");
        sb.Append(statusModel.Attack);
        sb.Append("\n");

        sb.Append("최대 체력 : ");
        sb.Append(statusModel.MaxHP);
        sb.Append("\n");


        sb.Append("최대 스태미나 : ");
        sb.Append(statusModel.MaxStamina);
        sb.Append("\n");

        sb.Append("스태미나 회복력 : ");
        sb.Append(string.Format("{0:N2}", statusModel.RecoveryStaminaMag));
        sb.Append("\n");

        sb.Append($"회피 소비 스태미나 : ");
        sb.Append(string.Format("{0:N2}", statusModel.ConsumeStamina));
        sb.Append("\n");

        sb.Append("공격속도 : ");
        sb.Append(string.Format("{0:N2}", statusModel.AttackSpeed));
        sb.Append("\n");

        sb.Append("이동속도 : ");
        sb.Append(string.Format("{0:N2}", statusModel.MoveSpeed));
        sb.Append("\n");

        sb.Append("치명타 확률 : ");
        sb.Append(string.Format("{0:N2}", statusModel.CriticalRate));
        sb.Append("\n");

        sb.Append("치명타 데미지 배율 : ");
        sb.Append(string.Format("{0:N2}", statusModel.CriticalDamageRate));
        sb.Append("\n");

        statusText.text = sb.ToString();
    }
}
