using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class SoundManager : Singleton<SoundManager>
{
    private FMOD.Studio.EventInstance m_stereoInstance;

    void Start()
    {
    }

    public void PlayIntroDialogueClickSound()
    {
        if (RandomEvent(0.2f)) // 20 % chance to succeed
            FMODUnity.RuntimeManager.PlayOneShot("event:/CheersClicking");
        else
            FMODUnity.RuntimeManager.PlayOneShot("event:/Clicking");
    }

    public void PlayDialogueClickSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Clicking");
    }

    // BOOLS
    public void UpdateSoundsBoolean(string parameterName, bool boolValue)
    {
        if (parameterName.Equals("STEREO_IS_PLAYING") && boolValue)
        {
            if (GameManager.Instance.State.STEREO_IS_ON)
            {
                UnityEngine.Debug.Log("Fmod: Stereo Music Start");
                m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/StereoSpeakerMusic");
                m_stereoInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
                m_stereoInstance.start();
            }
        }
        else if (parameterName.Equals("STEREO_IS_PLAYING") && !boolValue)
        {
            if (GameManager.Instance.State.STEREO_IS_ON)
            {
                UnityEngine.Debug.Log("Fmod: Stereo Music Stop");
                m_stereoInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
        }
        else if (parameterName.Equals("STEREO_IS_ON") && boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Switch On");
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoON");
        }
        else if (parameterName.Equals("STEREO_IS_ON") && !boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Switch Off");
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
        }
        else if (parameterName.Equals("STEREO_BASS_BOOST") && boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Bass Boost On");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 1);
        }
        else if (parameterName.Equals("STEREO_BASS_BOOST") && !boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Stereo Bass Boost Off");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 0);
        }
        else if (parameterName.Equals("HAS_EARS") && boolValue)
        {
            UnityEngine.Debug.Log("Fmod: Got Ears");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("hasEars", 1);
        }
        else if (parameterName.Equals("INTRO_FINISHED") && boolValue)
        {
            // Player has clicked the last conversation option in intro cutscene.
        }
        else if (parameterName.Equals("HAS_NOSE") && boolValue)
        {
            // The bedroom dialogue has started.
        }
    }

    // INTS
    public void UpdateSoundsInteger(string parameterName, int intValue)
    {
        if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 1)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 1");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 1);
        }
        else if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 2)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 2");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 2);
        }
        else if (parameterName.Equals("INTRO_DRUNKENNESS_LEVEL") && intValue == 3)
        {
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 2");
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Chug", 3);
        }
    }

    private bool RandomEvent(float successPercentage)
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= successPercentage)
            return true;
        return false;
    }
}
