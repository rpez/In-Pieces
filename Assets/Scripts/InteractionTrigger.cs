using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : MonoBehaviour
{
    private Interactable parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.transform.GetComponent<PlayerController>().OnTargetInteractableReached();
            parent.OnInteract(other.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
