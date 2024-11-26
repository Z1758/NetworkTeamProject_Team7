using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureObject : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] GameObject frag;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(frag.name);
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFadeOut(Collider collider, GameObject obj, Vector3 vec )
    {
        Debug.Log(obj.name);
        StartCoroutine(FadeOut());
    }
    
    public void LLELEL(Collider collider, GameObject obj, Vector3 vec)
    {
        Debug.Log(frag.name);
       frag.SetActive(true);
        gameObject.SetActive(false);
    }

    IEnumerator FadeOut()
    {
        while (true)
        {
            Color color = meshRenderer.material.color;
            color.a -= Time.deltaTime;
            meshRenderer.material.color = color;
 
            yield return new WaitForSeconds(0.1f);

        }
    }
}
