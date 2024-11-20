using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressableTester : MonoBehaviour
{
    [SerializeField] GameObject cubePrefab;
    void Start()
    {
      // cubePrefab = Addressables.LoadAssetAsync<GameObject>("GameObject/Monster").WaitForCompletion();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
