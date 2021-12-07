using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;


public class LivingRoom : MonoBehaviour
{
    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //changing global parameter in fmod. you have to check global paramter in fmod edit paramter window
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("inLivingRoom", 1);

            UnityEngine.Debug.Log("inBedroom");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("inLivingRoom", 0);
            UnityEngine.Debug.Log("NotInBedroom");
        }
    }
}
