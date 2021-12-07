using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPlayerFollower : MonoBehaviour
{
    private GameObject m_player;

    private float offsetX = 0.0f;
    private float offsetY = 1.6f;
    private float offsetZ = 3.0f;

    void Start()
    {
        m_player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = m_player.transform.position + new Vector3(offsetX, offsetY, offsetZ);
    }
}
