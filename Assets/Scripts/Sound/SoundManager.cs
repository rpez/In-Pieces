using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class SoundManager : Singleton<SoundManager>
{
    // Fmod Event instances
    private FMOD.Studio.EventInstance m_stereoInstance;
    private FMOD.Studio.EventInstance AmbienceMain;
    private FMOD.Studio.EventInstance IntroAmbience;
    private FMOD.Studio.EventInstance oldClock;
    private FMOD.Studio.EventInstance walkingFmod;
    private FMOD.Studio.EventInstance idleFmod;

    void Start()
    {
        walkingFmod = FMODUnity.RuntimeManager.CreateInstance("event:/walkingNose");
        idleFmod = FMODUnity.RuntimeManager.CreateInstance("event:/idleNose");

        // create main ambience instance
        m_stereoInstance = FMODUnity.RuntimeManager.CreateInstance("event:/StereoSpeakerMusic");
        m_stereoInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

    }

    // clicking sounds
    public void PlayIntroDialogueClickSound()
    {
        if (RandomEvent(0.4f)) // 40 % chance to succeed
            FMODUnity.RuntimeManager.PlayOneShot("event:/CheersClicking");
        else
            FMODUnity.RuntimeManager.PlayOneShot("event:/Clicking");
    }

    public void PlayDialogueClickSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Clicking");
    }

    public void PlayWalkingAnimationSound()
    {
        walkingFmod.start();
    }

    public void StopWalkingAnimationSound()
    {
        walkingFmod.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        idleFmod.start();
    }

    // GamesState-BOOLS
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/StereoON");
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
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("IntroFinished", 1);
        }
        // Intro ambience starting
        else if (parameterName.Equals("INTRO_START") && boolValue)
        {
            IntroAmbience = FMODUnity.RuntimeManager.CreateInstance("event:/IntroAmbience");
            IntroAmbience.start();
        }




        else if (parameterName.Equals("HAS_NOSE") && boolValue)
        {
            // The bedroom dialogue has started.
            AmbienceMain = FMODUnity.RuntimeManager.CreateInstance("event:/AmbienceMain");
            AmbienceMain.start();

            oldClock = FMODUnity.RuntimeManager.CreateInstance("event:/tickingClock");
            oldClock.start();

            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BedroomStarts", 1);
        }
    }

    // GamesState-INTS
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
            UnityEngine.Debug.Log("Fmod: Intro Drunkenness Level 3");
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


