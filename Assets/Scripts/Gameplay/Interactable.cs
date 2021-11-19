using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject m_interactionPoint;
    public string m_dialogueIdentifier;

    private UIManager m_UI;

    public void OnClicked()
    {
        Debug.Log(gameObject.name + " clicked");
        m_interactionPoint.SetActive(true);
    }

    public void CancelClicked()
    {
        Debug.Log(gameObject.name + " un-clicked");
        m_interactionPoint.SetActive(false);
    }

    public void OnInteract(GameObject actor)
    {
        m_UI.StartConversation(m_dialogueIdentifier);
    }

    public void OnExitInteraction()
    {
        m_interactionPoint.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_UI = (UIManager)FindObjectOfType(typeof(UIManager));
    }
}
