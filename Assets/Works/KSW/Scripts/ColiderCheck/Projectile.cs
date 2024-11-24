using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [Header("투사체")]
    [SerializeField] Transform projectilePoint;
    [SerializeField] Rigidbody rigid;

   
    [SerializeField] float speed;
    [SerializeField] float returnTime;

    [Header("범위 공격 체크")]
    [SerializeField] bool isAOE;
    [SerializeField] bool hasParent;


    WaitForSeconds returnTimeWFS;
    Coroutine returnTimeCoroutine;

    private void Awake()
    {
        returnTimeWFS = new WaitForSeconds(returnTime);
 
    
    }

    private void OnEnable()
    {
            MoveProjectile();
        returnTimeCoroutine = StartCoroutine(ReturnTimeRoutine());


    }
    private void Start()
    {
        if(!hasParent)
        transform.SetParent(null);
    }

    private void MoveProjectile()
    {
        if (isAOE)
        {
            return;
        }

        transform.SetPositionAndRotation(projectilePoint.position, projectilePoint.rotation);
        rigid.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAOE)
            return;
        if (returnTimeCoroutine != null)
        {
            StopCoroutine(returnTimeCoroutine);
        }
        
        DisableProjectile();
    }
 
    IEnumerator ReturnTimeRoutine()
    {
        
        yield return returnTimeWFS;
        DisableProjectile();
    }

    
    private void DisableProjectile()
    {
        rigid.velocity = Vector3.zero;


        gameObject.SetActive(false);
    }



    private void OnDisable()
    {
      
        StopAllCoroutines();
    }

}
