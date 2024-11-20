using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // ΩÃ±€≈Ê
    private static AudioManager instance;

    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource bgmSource;
    [SerializeField] AudioSource voiceSource;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static AudioManager GetInstance()
    {
        Debug.Log("ΩÃ±€≈Ê ∑ŒµÂ");
        return instance;
    }

    void Awake()
    {
        if (instance == null)
        {

            instance = this;
        }
        else
        {
            Destroy(this);
        }
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
