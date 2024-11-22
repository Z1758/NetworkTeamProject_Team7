using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SkillView : MonoBehaviour
{

    [SerializeField] Image[] cooltimeBackgroundImage;
    [SerializeField] Image[] cooltimeImage;
    [SerializeField] StatusModel model;

    public List<string> uiKeys = new List<string>() { "UI" };
    AsyncOperationHandle<IList<Sprite>> uiLoadHandle;

    [SerializeField] public Dictionary<string, Sprite> uiDic = new Dictionary<string, Sprite>();

    StringBuilder uiBuilder = new StringBuilder();

    private void Awake()
    {
      
        uiLoadHandle = Addressables.LoadAssetsAsync<Sprite>(
            uiKeys,
            addressable =>
            {
                uiBuilder.Clear();
                uiBuilder.Append($"UI/");

                if (addressable != null)
                {
                    uiBuilder.Append(addressable.name);
                    uiDic.Add(uiBuilder.ToString(), addressable);
                }


            }, Addressables.MergeMode.Union,
            false);
        uiLoadHandle.Completed += LoadSoundHandle_Completed;
    }

 
    void GetSpriteUI()
    {
       
        for (int i = 0; i < cooltimeImage.Length; i++) {
            uiBuilder.Clear();
            uiBuilder.Append($"UI/Player{model.CharacterNumber}Skill{i+1}");

            cooltimeBackgroundImage[i].sprite = uiDic[uiBuilder.ToString()];
            cooltimeImage[i].sprite = uiDic[uiBuilder.ToString()];

        }
    }

    public void SetModel(StatusModel model)
    {

        this.model = model;
        GetSpriteUI();

        model.OnChangedCoolTimeEvent += SetSkillImage;

        for (int i = 0; i < cooltimeImage.Length; i++)
        {
            cooltimeImage[i].fillAmount = 0;
        }

        SetSprite();
    }
    private void SetSprite()
    {

        for (int i = 0; i < cooltimeBackgroundImage.Length; i++)
        {
            cooltimeBackgroundImage[i].gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (model != null)
            model.OnChangedCoolTimeEvent -= SetSkillImage;
    }

    private void LoadSoundHandle_Completed(AsyncOperationHandle<IList<Sprite>> operation)
    {

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("UI 에셋 로딩 실패");
        }
        else
        {
            Debug.LogWarning("UI 에셋 로딩 완료");
        }
    }
    private void OnDestroy()
    {
        Addressables.Release(uiLoadHandle);

  
    }


    public void SetSkillImage(int num, float value)
    {
        cooltimeImage[num].fillAmount = value/ model.SkillCoolTime[num];

    }
}
