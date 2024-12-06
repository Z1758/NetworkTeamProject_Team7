using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FractureObjectManager : MonoBehaviour
{
    [SerializeField] float spawnX;
    [SerializeField] float spawnZ;
    [SerializeField] int spawnCount;


    [SerializeField] List<string> objList ;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            SpawnObject();
        }
    }


    public void SpawnObject()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            float x = Random.Range(-spawnX, spawnX);
            float z = Random.Range(-spawnZ, spawnZ);
            Vector3 randomPos = new Vector3(x, 0, z);
            if ( x > -20 && x <20 )
            {
                if (z > 20 && z < 45)
                {
                    i--;
                    continue;
                }
            }


            PhotonNetwork.Instantiate($"GameObject/FracObjects/{objList[Random.Range(0, objList.Count)]}", randomPos, Quaternion.Euler(-90, Random.Range(0, 360), 0));
        }
    }

  
}
