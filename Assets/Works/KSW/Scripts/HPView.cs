using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class HPView : SliderView
{
    private void Update()
    {
        if (!isEndLoading)
            return;


        if (slider.value > currentValue)
        {
            slider.value -= Time.deltaTime * (model.MaxHP * 0.5f);
            if (slider.value < currentValue)
            {
                slider.value = currentValue;
            }
        }
        else if (slider.value < currentValue)
        {
            slider.value += Time.deltaTime * (model.MaxHP * 0.3f);
            if (slider.value > currentValue)
            {
                slider.value = currentValue;
            }
        }
        else
        {
            slider.value = currentValue;
        }
    }

    public override void SetModel(StatusModel model)
    {

        this.model = model;
        model.OnChangedHpEvent += SetSlider;
        model.OnChangedMaxHpEvent += SetSliderMax;
        slider.maxValue = model.MaxHP;
        currentValue = model.MaxHP;
        slider.value = model.MaxHP;
        isEndLoading = true;
        
    }

    private void OnDisable()
    {
        if (model)
        {
            model.OnChangedHpEvent -= SetSlider;
            model.OnChangedMaxHpEvent -= SetSliderMax;
        }
    }
}
