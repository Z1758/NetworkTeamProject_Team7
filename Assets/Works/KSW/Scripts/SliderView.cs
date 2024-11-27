using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using UnityEngine.UI;
public class SliderView : MonoBehaviour
{
    [SerializeField] protected Slider slider;

    protected StatusModel model;

    // 캐릭터 생성 체크
    protected bool isEndLoading;


    [SerializeField]protected float currentValue;
 
    

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }
 

    public virtual void SetModel(StatusModel model)
    {

        this.model = model;
        isEndLoading = true;
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
