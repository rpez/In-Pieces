using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject m_target;

    private Vector3 m_offset;

    private Vector3 m_playerPosition;

    private GameObject m_player;
    private Transform m_hiddenWall;

    private float m_rotationAmount;
    private float m_mouseSensitivity = 5.0f;
    private float m_keySensitivity = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (!m_player.GetComponent<PlayerController>().m_movementEnabled) return;

        Vector3 m_playerCenter = m_player.GetComponent<Collider>().bounds.center;

        if (Physics.Raycast(transform.position, m_playerCenter - transform.position, out var hit, 3.1f, LayerMask.GetMask("HideWall")))
        {
            m_hiddenWall = hit.transform;
            foreach (Transform child in m_hiddenWall)
                child.gameObject.SetActive(false);
        }
        else if (m_hiddenWall != null)
        {
            foreach (Transform child in m_hiddenWall)
                child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            m_rotationAmount = Input.GetAxis("Mouse X") * m_mouseSensitivity;
            transform.RotateAround(m_player.transform.position, Vector3.up, m_rotationAmount);
            transform.LookAt(m_player.transform.position + Vector3.up * 0.2011f);  // yay for silly magic numbers
        }
        else if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0)
        {
            m_rotationAmount = Input.GetAxisRaw("Horizontal") * m_keySensitivity;
            transform.RotateAround(m_player.transform.position, Vector3.up, m_rotationAmount);
            transform.LookAt(m_player.transform.position + Vector3.up * 0.2011f);  // yay for silly magic numbers
        }
    }
}
