using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // The meshes for different body parts, set in editor
    public GameObject m_noseMesh;
    public GameObject m_eyesMesh;
    public GameObject m_earsMesh;
    public GameObject m_handsMesh;
    public GameObject m_legsMesh;

    public GameObject m_moveIndicator;
    public Volume m_postProcessing;
    public VolumeProfile m_defaultPPP;
    public VolumeProfile m_nearSightedPPP;

    // Offset positions for the armature with and without legs
    private Vector3 m_nosePosition;
    private Vector3 m_nosePositionWithoutLegs;

    private GameObject m_currentIndicator;

    private NavMeshAgent m_navMeshAgent;
    private Interactable m_currentTargetInteractable;
    private Interactable m_currentHovered;

    private Animator m_animator;

    private bool m_movementEnabled = true;

    public void OnTargetInteractableReached()
    {
        m_navMeshAgent.SetDestination(transform.position);
    }

    // Sets a flag that determines whether the player is registering movement commands
    public void SetMovementActive(bool active)
    {
        SetWalkingAnimation(false);
        m_movementEnabled = active;
        m_navMeshAgent.enabled = active;
    }

    public void TravelToWaypoint(Vector3 location)
    {
        m_navMeshAgent.enabled = false;
        transform.position = location;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();

        // Save offsets
        m_nosePosition = m_noseMesh.transform.localPosition;
        m_nosePositionWithoutLegs = m_nosePosition - Vector3.up * 2f;

        UpdateBodyParts();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_navMeshAgent.enabled)
        {
            if (!m_navMeshAgent.pathPending)
            {
                if (m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
                {
                    if (!m_navMeshAgent.hasPath || m_navMeshAgent.velocity.sqrMagnitude == 0f)
                    {
                        SetWalkingAnimation(false);
                        SpawnIndicator(Vector3.zero, true);
                    }
                }
            }
        }

        // If movement is disabled, return
        if (!m_movementEnabled) return;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit))
        {
            if (hit.transform.tag == "Interactable")
            {
                if (m_currentHovered != null && m_currentHovered.gameObject != hit.transform.gameObject)
                {
                    CancelCurrentHover();
                }
                if (m_currentHovered == null)
                {
                    Interactable script = hit.transform.GetComponent<Interactable>();
                    script.OnHoverEnter();
                    m_currentHovered = script;
                }
            }
            else if (m_currentHovered != null)
            {
                CancelCurrentHover();
            }
            // Movement only
            if (Input.GetMouseButtonDown(1))
            {
                SpawnIndicator(hit.point);
                m_navMeshAgent.SetDestination(hit.point);
                CancelCurrentInteractionTarget();

                SetWalkingAnimation(true);
            }
            // Interaction
            else if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform.tag == "Interactable")
                {
                    SpawnIndicator(hit.point);
                    CancelCurrentInteractionTarget();

                    Interactable obj = hit.transform.GetComponent<Interactable>();
                    obj.OnClicked();
                    m_navMeshAgent.SetDestination(obj.m_interactionPoint.transform.position);

                    m_currentTargetInteractable = obj;

                    SetWalkingAnimation(true);
                }
            }
        }
    }

    private void SetWalkingAnimation(bool active)
    {
        if (active)
        {
            m_animator.Play("Walk");
        }
        else
        {
            m_animator.Play("Idle");
        }
    }

    public void UpdateBodyParts()
    {
        if (m_currentTargetInteractable != null)
        {
            if (m_currentTargetInteractable is CollectableInteractable interactable)
            {
                interactable.OnInteractionEvent();
            }
        }

        if (GameManager.Instance.GetStateValue<bool>("HAS_EYES"))
            m_postProcessing.profile = m_defaultPPP;
        else
            m_postProcessing.profile = m_nearSightedPPP;
        if (GameManager.Instance.GetStateValue<bool>("HAS_LEGS"))
            m_noseMesh.transform.localPosition = m_nosePosition;
        else
            m_noseMesh.transform.localPosition = m_nosePositionWithoutLegs;
        m_eyesMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_EYES"));
        m_earsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_EARS"));
        m_handsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_HAND"));
        m_legsMesh.SetActive(GameManager.Instance.GetStateValue<bool>("HAS_LEGS"));
    }

    private void SpawnIndicator(Vector3 point, bool cancel = false)
    {
        if (m_currentIndicator != null) Destroy(m_currentIndicator);
        if (!cancel) m_currentIndicator = GameObject.Instantiate(m_moveIndicator, point, Quaternion.identity);
    }

    // Called when player clicks something else to cancecl current pathing towards interaction target
    private void CancelCurrentInteractionTarget()
    {
        if (m_currentTargetInteractable != null)
        {
            m_currentTargetInteractable.CancelClicked();
            m_currentTargetInteractable = null;
        }
    }

    private void CancelCurrentHover()
    {
        if (m_currentHovered != null)
        {
            m_currentHovered.OnHoverExit();
            m_currentHovered = null;
        }
    }
}