using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
public class SliderView : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected StatusModel model;

    protected float currentValue;
 
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
 

    public virtual void SetModel(StatusModel model)
    {

        this.model = model;
    }

    public void SetSlider(float value)
    {
        currentValue = value;
      
    }
    public void SetSliderMax(float value)
    {
        slider.maxValue = value;

    }
}
