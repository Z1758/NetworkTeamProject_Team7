using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class HPView : SliderView
{


    public override void SetModel(StatusModel model)
    {

        this.model = model;
        model.OnChangedHpEvent += SetSlider;
        slider.maxValue = model.MaxHP;
        slider.value = model.MaxHP;
    }

    private void OnDisable()
    {
        if (model != null)
            model.OnChangedHpEvent -= SetSlider;
    }
}
