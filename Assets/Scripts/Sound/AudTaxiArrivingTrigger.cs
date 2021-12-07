using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AudTaxiArrivingTrigger : MonoBehaviour
{
    private FMOD.Studio.EventInstance TaxiArraving;

    // Start is called before the first frame update
    void Start()
    {
        TaxiArraving = FMODUnity.RuntimeManager.CreateInstance("event:/TaxiArraving");

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //changing global parameter in fmod. you have to check global paramter in fmod edit paramter window
            TaxiArraving.start();
        }
    }
}