using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockFXTrigger : MonoBehaviour
{
    
    private FMOD.Studio.EventInstance oldClock;

    // Start is called before the first frame update
    void Start()
    {
        oldClock = FMODUnity.RuntimeManager.CreateInstance("event:/tickingClock");
        oldClock.start();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //changing global parameter in fmod. you have to check global paramter in fmod edit paramter window
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TaxiTrigger", 1);
        }
    }
}
