using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    // ΩÃ±€≈Ê
    private static AudioManager instance;

    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource voiceSource;


    [SerializeField] public Dictionary<string, AudioClip> monsterSoundDic;
    [SerializeField] public Dictionary<string, AudioClip> playerSoundDic;
    [SerializeField] public Dictionary<string, AudioClip> commonSoundDic;

    StringBuilder voiceStringBuilder = new StringBuilder();
    StringBuilder soundStringBuilder = new StringBuilder();
    StringBuilder commonStringBuilder = new StringBuilder();
    public static AudioManager GetInstance()
    {
        Debug.Log("ΩÃ±€≈Ê ∫“∑Øø¿±‚");
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
        }
        else
        {
            Destroy(this);
        }
    }


    public AudioClip GetMonsterSoundDic(int characterNumber,string str)
    {
        voiceStringBuilder.Clear();
        voiceStringBuilder.Append($"Monster{characterNumber}/");
        voiceStringBuilder.Append(str);

        string resultStr = voiceStringBuilder.ToString();

        monsterSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
            monsterSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
           clip = monsterSoundDic[resultStr];
        }

        return clip;
    }

    public AudioClip GetPlayerSoundDic(int characterNumber, string str)
    {
        voiceStringBuilder.Clear();
        voiceStringBuilder.Append($"Player{characterNumber}/");
        voiceStringBuilder.Append(str);

        string resultStr = voiceStringBuilder.ToString();

        playerSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
            playerSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = playerSoundDic[resultStr];
        }

        return clip;
    }

    public AudioClip GetCommonSoundDic(string str)
    {
        commonStringBuilder.Clear();
        commonStringBuilder.Append($"Common/");
        commonStringBuilder.Append(str);

        string resultStr = commonStringBuilder.ToString();

        commonSoundDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
            commonSoundDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = commonSoundDic[resultStr];
        }

        return clip;
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
}
