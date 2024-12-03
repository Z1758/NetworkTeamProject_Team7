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
       
        if(model.MaxHP != slider.maxValue)
        {
            slider.maxValue = model.MaxHP;
        }

        if (slider.value > model.HP)
        {
            slider.value -= Time.deltaTime * (model.MaxHP * 0.5f);
            if (slider.value < model.HP)
            {
                slider.value = model.HP;
            }
        }
        else if (slider.value < currentValue && model.ModelType == ModelType.PLAYER)
        {
            if (model.ModelType == ModelType.PLAYER)
            {

                slider.value += Time.deltaTime * (model.MaxHP * 0.3f);
                if (slider.value > currentValue)
                {
                    slider.value = currentValue;
                }
            }
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
