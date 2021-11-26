using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class StereoSpeakerAudio : MonoBehaviour
{

    private FMOD.Studio.EventInstance instance;


    // Start is called before the first frame update
    void Start()
    {
        // I initiate the fmod stereo Music instance
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Stereo");
        
    }

    // Update is called once per frame
    void Update()
    {
        // i replaced our bools with buttons A-H, have fun with playing arround
        // i guess in our music manager i can write something like this but instead of using buttons i will use the states
        // .... from the dialig manager.


        //stereo speaker button on sound
        if (Input.GetKeyDown(KeyCode.A))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOn");
        }

        //stereo speaker button off sound
        if (Input.GetKeyDown(KeyCode.S))
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/StereoOff");
        }


        //playButton 
        if (Input.GetKeyDown(KeyCode.D))
        {
            instance.start();
        }

        //stopButton
        if (Input.GetKeyDown(KeyCode.F))
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }


        //BassBoost ON (just a silly FX right now)
        if (Input.GetKeyDown(KeyCode.G))
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 1);
        }

        // BassBoostOFF
        if (Input.GetKeyDown(KeyCode.H))
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("BassBoost", 0);
        }


    }
}
