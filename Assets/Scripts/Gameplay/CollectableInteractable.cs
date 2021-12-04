using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableInteractable : Interactable
{
    public string m_trackedState;

    // Called when something meaningful happens in interaction, disables the interactable
    public void OnInteractionEvent()
    {
        if (GameManager.Instance.GetStateValue<bool>(m_trackedState))
        {
            gameObject.GetComponent<Collider>().enabled = false;
            m_mesh.SetActive(false);
        }
    }
}
