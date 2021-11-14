using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject m_mesh;
    public GameObject m_interactionPoint;

    private int m_defaultLayer;

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
        Debug.Log("Hi, " + actor.name + ", I am " + gameObject.name + ". You just interacted with me.");
    }

    public void OnExitInteraction()
    {
        m_interactionPoint.SetActive(false);
    }

    public void OnHoverEnter()
    {
        SetLayerRecursively(m_mesh, 6);
    }

    public void OnHoverExit()
    {
        SetLayerRecursively(m_mesh, m_defaultLayer);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
