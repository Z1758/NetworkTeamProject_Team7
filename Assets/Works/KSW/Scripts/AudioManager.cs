using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class AudioManager : MonoBehaviour
{
    [Serializable]
    public class SoundKeys
    {
        public int num;
        public List<string> keys = new List<string>();
       
    }

    // 싱글톤
    private static AudioManager instance;

    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource voiceSource;

    public List<string> commonKeys = new List<string>() { "CommonSound"};
    public List<string> bgmKey = new List<string>() { "BGM" };
    [SerializeField] SoundKeys[] playerKeys;
    [SerializeField] SoundKeys[] monsterKeys;

    AsyncOperationHandle<IList<AudioClip>> playerSoundLoadHandle;
    AsyncOperationHandle<IList<AudioClip>> monsterSoundLoadHandle;
    AsyncOperationHandle<IList<AudioClip>> commonSoundLoadHandle;
    AsyncOperationHandle<IList<AudioClip>> bgmSoundLoadHandle;

    [SerializeField] public Dictionary<string, AudioClip> monsterSoundDic;
    [SerializeField] public Dictionary<string, AudioClip> playerSoundDic;
    [SerializeField] public Dictionary<string, AudioClip> commonSoundDic;
    [SerializeField] public Dictionary<string, AudioClip> bgmDic;

    StringBuilder soundStringBuilder = new StringBuilder();
    public static AudioManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {
          
            instance = this;

            monsterSoundDic = new Dictionary<string, AudioClip>();
            playerSoundDic = new Dictionary<string, AudioClip>();
            commonSoundDic = new Dictionary<string, AudioClip>();
            bgmDic = new Dictionary<string, AudioClip>();

            LoadPlayerSounds();
            LoadMonsterSounds();
            LoadCommonSounds();
            LoadBGM();
        }
        else
        {
            Destroy(this);
        }
    }

    #region 한꺼번에 로딩
    public void LoadPlayerSounds()
    {
        foreach(SoundKeys key  in playerKeys)
        {
            playerSoundLoadHandle = Addressables.LoadAssetsAsync<AudioClip>(
              key.keys,
              addressable =>
              {
                  soundStringBuilder.Clear();
                  soundStringBuilder.Append($"Player{key.num}/");

                  if (addressable)
                  {
                      soundStringBuilder.Append(addressable.name);
                      playerSoundDic.Add(soundStringBuilder.ToString(), addressable);
                  }


              }, Addressables.MergeMode.Union,
              false);
            playerSoundLoadHandle.Completed += LoadSoundHandle_Completed;
        }
        
          
        
    }
    public void LoadMonsterSounds()
    {
        foreach (SoundKeys key in monsterKeys)
        {
            monsterSoundLoadHandle = Addressables.LoadAssetsAsync<AudioClip>(
              key.keys,
              addressable =>
              {
                  soundStringBuilder.Clear();
                  soundStringBuilder.Append($"Monster{key.num}/");

                  if (addressable)
                  {
                      soundStringBuilder.Append(addressable.name);
                      monsterSoundDic.Add(soundStringBuilder.ToString(), addressable);
                  }


              }, Addressables.MergeMode.Union,
              false);
            monsterSoundLoadHandle.Completed += LoadSoundHandle_Completed;
        }



    }
    public void LoadCommonSounds()
    {

        commonSoundLoadHandle = Addressables.LoadAssetsAsync<AudioClip>(
            commonKeys,
            addressable =>
            {
                
                soundStringBuilder.Clear();
                //추후에 변경
                soundStringBuilder.Append("Common/");

                if (addressable)
                {
                    soundStringBuilder.Append(addressable.name);
                    commonSoundDic.Add(soundStringBuilder.ToString(), addressable);
                }


            }, Addressables.MergeMode.Union,
            false);
        commonSoundLoadHandle.Completed += LoadSoundHandle_Completed;

    }

    public void LoadBGM()
    {

        bgmSoundLoadHandle = Addressables.LoadAssetsAsync<AudioClip>(
            bgmKey,
            addressable =>
            {

                soundStringBuilder.Clear();
               

                if (addressable)
                {
              
                    soundStringBuilder.Append(addressable.name);
                    
                    bgmDic.Add(soundStringBuilder.ToString(), addressable);
                }

            }, Addressables.MergeMode.Union,
            false);
        bgmSoundLoadHandle.Completed += LoadSoundHandle_Completed;

       

    }
    #endregion

    #region 사운드 파일 전달
    public AudioClip GetMonsterSoundDic(int characterNumber,string str)
    {
        soundStringBuilder.Clear();
        soundStringBuilder.Append($"Monster{characterNumber}/");
        soundStringBuilder.Append(str);

        string resultStr = soundStringBuilder.ToString();

        monsterSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip is null)
        {
            Debug.Log($"{resultStr} 몬스터 사운드 초기화");
            monsterSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
           clip = monsterSoundDic[resultStr];
        }

        return clip;
    }

 
    public AudioClip GetPlayerSoundDic(int characterNumber, string str)
    {
        soundStringBuilder.Clear();
        soundStringBuilder.Append($"Player{characterNumber}/");
        soundStringBuilder.Append(str);

        string resultStr = soundStringBuilder.ToString();

        playerSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip is null)
        {
            Debug.Log("플레이어 사운드 초기화");
            playerSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = playerSoundDic[resultStr];
            Debug.Log(clip.name);
        }

        return clip;
    }

    public AudioClip GetCommonSoundDic(string str)
    {
        soundStringBuilder.Clear();
        soundStringBuilder.Append($"Common/");
        soundStringBuilder.Append(str);

        string resultStr = soundStringBuilder.ToString();

        commonSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip is null)
        {
            Debug.Log("일반 사운드 초기화");
            commonSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = commonSoundDic[resultStr];
        }

        return clip;
    }
    #endregion
    private void LoadSoundHandle_Completed(AsyncOperationHandle<IList<AudioClip>> operation)
    {

        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("사운드 에셋 로딩 실패");
        }
        else
        {
            Debug.LogWarning("사운드 에셋 로딩 완료");
        }

 
    }
    private void OnDestroy()
    {
        Addressables.Release(monsterSoundLoadHandle);

        Addressables.Release(playerSoundLoadHandle);

        Addressables.Release(commonSoundLoadHandle);
    }


    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayBGM(int num)
    {
        bgmSource.clip = bgmDic[$"Stage{num}"];
        bgmSource.Play();
    }

    public void PlayClearBGM()
    {
        bgmSource.clip = bgmDic["Victory"];
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
