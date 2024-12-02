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
      
    }

    private void Update()
    {
        if (statusModel)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                ChangeStatus();
            }
        }
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
        sb.Append(statusModel.RecoveryStaminaMag);
        sb.Append("\n");

        sb.Append("회피 소비 스태미나 : ");
        sb.Append(statusModel.ConsumeStamina);
        sb.Append("\n");

        sb.Append("공격속도 : ");
        sb.Append(statusModel.AttackSpeed);
        sb.Append("\n");

        sb.Append("이동속도 : ");
        sb.Append(statusModel.MoveSpeed);
        sb.Append("\n");

        sb.Append("치명타 확률 : ");
        sb.Append(statusModel.CriticalRate);
        sb.Append("\n");

        sb.Append("치명타 데미지 배율 : ");
        sb.Append(statusModel.CriticalDamageRate);
        sb.Append("\n");

        statusText.text = sb.ToString();
    }
}
