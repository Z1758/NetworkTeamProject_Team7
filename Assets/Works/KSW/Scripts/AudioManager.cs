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


    [SerializeField] public Dictionary<string, AudioClip> monsterVoiceDic;
    [SerializeField] public Dictionary<string, AudioClip> playerVoiceDic;
    [SerializeField] public Dictionary<string, AudioClip> effectSoundDic;

    StringBuilder voiceStringBuilder = new StringBuilder();
    StringBuilder soundStringBuilder = new StringBuilder();
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

            monsterVoiceDic = new Dictionary<string, AudioClip>();
            playerVoiceDic = new Dictionary<string, AudioClip>();
            effectSoundDic = new Dictionary<string, AudioClip>();
        }
        else
        {
            Destroy(this);
        }
    }


    public AudioClip GetMonsterVoiceDic(int characterNumber,string str)
    {
        voiceStringBuilder.Clear();
        voiceStringBuilder.Append($"Monster{characterNumber}/");
        voiceStringBuilder.Append(str);

        string resultStr = voiceStringBuilder.ToString();

        monsterVoiceDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
           monsterVoiceDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
           clip = monsterVoiceDic[resultStr];
        }

        return clip;
    }

    public AudioClip GetPlayerVoiceDic(int characterNumber, string str)
    {
        voiceStringBuilder.Clear();
        voiceStringBuilder.Append($"Player{characterNumber}/");
        voiceStringBuilder.Append(str);

        string resultStr = voiceStringBuilder.ToString();

        playerVoiceDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
            playerVoiceDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = playerVoiceDic[resultStr];
        }

        return clip;
    }
    public AudioClip GetSoundVoiceDic(string str)
    {
        soundStringBuilder.Clear();
        soundStringBuilder.Append("SoundEffect/");
        soundStringBuilder.Append(str);

        string resultStr = soundStringBuilder.ToString();

        playerVoiceDic.TryGetValue(resultStr, out AudioClip clip);
        if (clip == null)
        {
            playerVoiceDic.Add(resultStr, Addressables.LoadAssetAsync<AudioClip>(resultStr).WaitForCompletion());
            clip = playerVoiceDic[resultStr];
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
