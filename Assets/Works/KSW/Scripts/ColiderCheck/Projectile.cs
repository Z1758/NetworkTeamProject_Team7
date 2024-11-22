using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Hitbox
{
    [SerializeField] Transform projectilePoint;
    [SerializeField] Rigidbody rigid;
    [SerializeField] float speed;
    [SerializeField] float returnTime;
    [SerializeField] bool isAOE;

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
        if (isAOE)
        {
            return;
        }

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
        if (isAOE)
            return;
        rigid.velocity = Vector3.zero;

       
        gameObject.SetActive(false);
    }

    public void ActiveLayer()
    {
        gameObject.layer = (int)LayerEnum.PLAYER_PROJECTILE;
    }

    private void OnDisable()
    {
      
        StopAllCoroutines();
    }

}
