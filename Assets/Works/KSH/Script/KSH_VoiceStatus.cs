using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Voice.PUN;

public class KSH_VoiceStatus : MonoBehaviour
{
    [SerializeField] Image _voiceStatusImage;
    private PhotonVoiceView _photonVoiceView;


    private void Awake()
    {
        // »óÀ§ °´Ã¼¿¡¼­ PhotonVoiceView¸¦ °¡Á®¿È
        this._photonVoiceView = this.GetComponentInParent<PhotonVoiceView>();
    }

    void Update()
    {
        _voiceStatusImage.enabled = this._photonVoiceView.IsSpeaking;
        _voiceStatusImage.enabled = this._photonVoiceView.IsSpeaking;
    }
}