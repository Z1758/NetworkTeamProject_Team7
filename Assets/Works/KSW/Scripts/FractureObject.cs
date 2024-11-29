using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FractureObject : MonoBehaviourPun
{
    MeshRenderer meshRenderer;
    
    MeshRenderer[] meshRenderers;
    Rigidbody[] rigidbodies;



    [SerializeField] GameObject frags;
    [SerializeField] GameObject otherObj;
    [SerializeField] GameObject particleObj;

    [SerializeField] float forcePower;

    [SerializeField] string audioName;


    WaitForSeconds fadeDelay = new WaitForSeconds(0.1f);

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>(true);
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

   
    private void OnTriggerEnter(Collider other)
    {
     

        if ((other.CompareTag("Enemy") && photonView.IsMine) || other.CompareTag("Hitbox"))
        {
            if (other.TryGetComponent(out Hitbox hitbox))
            {


                if (hitbox.GetAngleHit(transform) == false)
                {
                    return;
                }
                hitbox.HitEffect(other.ClosestPoint(transform.position));


            }
            photonView.RPC(nameof(StartFadeOutRPC), RpcTarget.All);
         

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") && photonView.IsMine) || collision.gameObject.CompareTag("Hitbox"))
        {
            if (collision.gameObject.TryGetComponent(out Hitbox hitbox))
            {


                if (hitbox.GetAngleHit(transform) == false)
                {
                    return;
                }
              
                hitbox.HitEffect(collision.contacts[0].point);


            }
            photonView.RPC(nameof(StartFadeOutRPC), RpcTarget.All);


        }
    }
    public void CalledBindObj()
    {
        photonView.RPC(nameof(StartFadeOutRPC), RpcTarget.All);
    }


    [PunRPC]
     void StartFadeOutRPC()
    {
        if (otherObj)
        {
            otherObj.SetActive(false);

        }
        if (particleObj)
        {
            particleObj.transform.SetParent(null);
            particleObj.SetActive(true);
            
        }
        


        if (audioName != "")
        AudioManager.GetInstance().PlaySound(AudioManager.GetInstance().GetCommonSoundDic(audioName));

        meshRenderer.enabled = false;
        gameObject.layer = (int)LayerEnum.DISABLE_BOX;
        frags.SetActive(true);
        
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Vector3 ranPos = Random.insideUnitSphere;
            ranPos.y += 0.3f;
            rigidbodies[i].AddForce(ranPos* forcePower, ForceMode.Impulse);
           
        }

        StartCoroutine(FadeOut());
    }
  
    IEnumerator FadeOut()
    {
        float alpha = 1;
        Color color = meshRenderers[0].material.color;
       
        while (alpha > 0)
        {
            color.a = alpha;

            foreach (MeshRenderer renderer in meshRenderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.color = color;
                }

              
               

            }
            alpha -= 0.04f;
            yield return fadeDelay;

        }

        Destroy(gameObject);
    }
}
