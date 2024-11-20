using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HPView : MonoBehaviour
{
    [SerializeField] Slider hpSlider;
    [SerializeField] StatusModel model;

    private void Awake()
    {
        hpSlider = GetComponent<Slider>();
    }

    public void SetModel(StatusModel model)
    {
     
        this.model = model;
        model.OnChangedHpEvent += SetHPSlider;
        hpSlider.maxValue = model.MaxHP;
        hpSlider.value = model.MaxHP;
    }

    private void OnDisable()
    {
        if (model != null)
            model.OnChangedHpEvent -= SetHPSlider;
    }

    public void SetHPSlider(float hp)
    {
        hpSlider.value = hp;
     
    }
}
