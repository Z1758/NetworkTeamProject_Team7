using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualEffect : MonoBehaviour
{
    [SerializeField] Transform point;


    private void Start()
    {
        transform.SetParent(null);
    }

    private void OnEnable()
    {
       transform.position = point.position;
    }


}
