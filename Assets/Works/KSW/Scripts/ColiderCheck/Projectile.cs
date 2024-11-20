using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] Transform projectilePoint;
    [SerializeField] Rigidbody rigid;
    [SerializeField] float speed;
    [SerializeField] float returnTime;

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
        transform.SetParent(null);
    }

    private void MoveProjectile()
    {
      
        transform.SetPositionAndRotation(projectilePoint.position, projectilePoint.rotation);
        rigid.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(returnTimeCoroutine != null)
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
