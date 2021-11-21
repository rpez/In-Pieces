using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    // Set in editor
    public GameObject m_interactionPoint;
    public string m_dialogueIdentifier;

    protected PlayerController m_interactor;

    protected UIManager m_UI;

    // Called when this object is set as navigation target, enables the interaction trigger
    public void OnClicked()
    {
        m_interactionPoint.SetActive(true);
    }

    // Called when this object is the navigation target, but the navigation is cancelled
    // Disables the interaction trigger
    public void CancelClicked()
    {
        m_interactionPoint.SetActive(false);
    }

    // Called by the interaction trigger when player enters it
    // Initiates whatever will happen when interacted with
    virtual public void OnInteract(PlayerController actor)
    {
        m_interactor = actor;
        m_interactor.SetMovementActive(false);
        m_UI.StartConversation(m_dialogueIdentifier, OnExitInteraction);
    }

    // Called when interaction end, disables the trigger
    virtual public void OnExitInteraction()
    {
        m_interactor.SetMovementActive(true);
        m_interactor = null;
        m_interactionPoint.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}
