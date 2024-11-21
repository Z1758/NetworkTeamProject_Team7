using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{

    [SerializeField] Image[] cooltimeImage;
    [SerializeField] StatusModel model;

    private void Awake()
    {
       
    }

    public void SetModel(StatusModel model)
    {

        this.model = model;
        model.OnChangedCoolTimeEvent += SetSkillImage;

        for (int i = 0; i < cooltimeImage.Length; i++)
        {
            cooltimeImage[i].fillAmount = 0;
        }
    }

    private void OnDisable()
    {
        if (model != null)
            model.OnChangedCoolTimeEvent -= SetSkillImage;
    }

    public void SetSkillImage(int num, float value)
    {
        cooltimeImage[num].fillAmount = value/ model.SkillCoolTime[num];

    }
}
