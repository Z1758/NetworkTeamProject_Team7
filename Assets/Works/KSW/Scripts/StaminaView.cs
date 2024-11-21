using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
public class StaminaView : SliderView
{
  

    public override void SetModel(StatusModel model)
    {

        this.model = model;
        model.OnChangedStaminaEvent += SetSlider;
        slider.maxValue = model.MaxStamina;
        slider.value = model.MaxStamina;
    }

    private void OnDisable()
    {
        if (model != null)
            model.OnChangedStaminaEvent -= SetSlider;
    }

  
}
