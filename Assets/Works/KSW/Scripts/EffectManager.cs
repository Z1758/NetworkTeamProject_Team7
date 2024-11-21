using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;


    AsyncOperationHandle<IList<GameObject>> effectLoadHandle;


    public List<string> effectKeys = new List<string>() { "Effects" };


    [SerializeField] public Dictionary<string, GameObject> effectDic;

    StringBuilder effectStringBuilder = new StringBuilder();

    public static EffectManager GetInstance()
    {
        Debug.Log("이펙트 싱글톤 호출");
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {

            instance = this;
            effectDic = new Dictionary<string, GameObject>();
            LoadEffectObjects();
        }
        else
        {
            Destroy(this);
        }
    }


    void LoadEffectObjects()
    {

       
        effectLoadHandle = Addressables.LoadAssetsAsync<GameObject>(
        effectKeys,
        addressable =>
        {
            effectStringBuilder.Clear();
            //추후에 변경
            effectStringBuilder.Append("Effects/");

            if (addressable != null)
            {
                effectStringBuilder.Append(addressable.name);
                effectDic.Add(effectStringBuilder.ToString(), addressable);
            }


        }, Addressables.MergeMode.Union,
        false);
        effectLoadHandle.Completed += LoadSoundHandle_Completed;
    }


    public GameObject GetEffectDic(string str)
    {
        effectStringBuilder.Clear();
        effectStringBuilder.Append($"Effects/");
        effectStringBuilder.Append(str);

        string resultStr = effectStringBuilder.ToString();

        effectDic.TryGetValue(resultStr, out GameObject obj);
        if (obj == null)
        {
            Debug.Log("이펙트 초기화");
            effectDic.Add(resultStr, Addressables.LoadAssetAsync<GameObject>(resultStr).WaitForCompletion());
            obj = effectDic[resultStr];
    
        }

        return obj;
    }


    private void LoadSoundHandle_Completed(AsyncOperationHandle<IList<GameObject>> operation)
    {

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("이펙트 에셋 로딩 실패");
        }
        else
        {
            Debug.LogWarning("이펙트 에셋 로딩 완료");
        }
    }
}
