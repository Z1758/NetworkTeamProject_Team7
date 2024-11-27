using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class StaminaView : SliderView
{

    private void Update()
    {
        if (!isEndLoading)
            return;

        if (slider.value > currentValue)
        {
            slider.value -= Time.deltaTime * (model.MaxStamina * 0.5f);
            if (slider.value < currentValue)
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
        model.OnChangedStaminaEvent += SetSlider;
        model.OnChangedMaxStaminaEvent += SetSliderMax;
        currentValue = model.MaxStamina;
        slider.maxValue = model.MaxStamina;
        slider.value = model.MaxStamina;
        isEndLoading = true;
    }

    private void OnDisable()
    {
        if (model) { 
        model.OnChangedStaminaEvent -= SetSlider;
        model.OnChangedMaxStaminaEvent -= SetSliderMax;
      }
    }

  
}
