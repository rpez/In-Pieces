using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableTrigger : MonoBehaviour
{
    // Set the gameobject to be disabled on trigger enter in this array in editor
    public GameObject[] m_toDisable;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (GameObject obj in m_toDisable)
            {
                obj.SetActive(false);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            foreach (GameObject obj in m_toDisable)
            {
                obj.SetActive(true);
            }
        }
    }
}
