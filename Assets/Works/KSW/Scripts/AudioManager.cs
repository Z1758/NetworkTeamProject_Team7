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

    [SerializeField] AudioClip[,] monsterFootStep;
    [SerializeField] int monsterFootStepSize;

    [SerializeField] public Dictionary<string, AudioClip> monsterVoiceDic;


    [SerializeField] AudioClip[] playerHitVoices;
    [SerializeField] AudioClip[] playerDownVoices;

    StringBuilder voiceStringBuilder = new StringBuilder();

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
            SetVoice();
        }
        else
        {
            Destroy(this);
        }
    }

    private void SetVoice()
    {
        StringBuilder stringBuilder = new StringBuilder();

        string hitPath = "/Hit";
        string downPath = "/Down";
        for (int i = 0; i < playerHitVoices.Length; i++)
        {
           

            string path = $"Player{i+1}";

            stringBuilder.Append(path);
            stringBuilder.Append(hitPath);
            playerHitVoices[i] = Addressables.LoadAssetAsync<AudioClip>(stringBuilder.ToString()).WaitForCompletion();

            stringBuilder.Clear();

            stringBuilder.Append(path);
            stringBuilder.Append(downPath);
            playerDownVoices[i] = Addressables.LoadAssetAsync<AudioClip>(stringBuilder.ToString()).WaitForCompletion();
        }

        stringBuilder.Clear();

        monsterFootStep = new AudioClip[monsterFootStepSize, 2];
        string mobFootStepPath = "/FootStep";

        for (int i = 0; i < monsterFootStep.GetLength(0); i++)
        {
            for (int j = 0; j < 2; j++)
            {


                string path = $"Monster{i + 1}";

                stringBuilder.Append(path);
                stringBuilder.Append(mobFootStepPath);
                stringBuilder.Append(j+1);
                monsterFootStep[i, j] = Addressables.LoadAssetAsync<AudioClip>(stringBuilder.ToString()).WaitForCompletion();

                stringBuilder.Clear();

            }
        }



        monsterVoiceDic = new Dictionary<string, AudioClip>();

        
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

    public AudioClip GetFootStep(int characterNum, int audioNum)
    {
        return monsterFootStep[characterNum,audioNum];
    }

    public void PlaySound(AudioClip clip)
    {
        soundSource.PlayOneShot(clip);
    }

    public void PlayVoice(AudioClip clip)
    {
        voiceSource.PlayOneShot(clip);
    }

    public void PlayHitVoice(int num)
    {
        voiceSource.PlayOneShot(playerHitVoices[num]);
    }
    public void PlayDownVoice(int num)
    {
        voiceSource.PlayOneShot(playerDownVoices[num]);
    }
    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }
}
