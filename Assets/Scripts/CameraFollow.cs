// ---------------------------------------------------------------------------
// CameraFollow.cs
// 
// Lerps camera to follow the player around the level
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public GameObject m_followTarget;
    public float m_followSpeed;

    void Update()
    {
        Vector3 P = transform.position;
        P = Vector3.Lerp(P, m_followTarget.transform.position, m_followSpeed * Time.deltaTime);
        P.z = -8.0f;
        transform.position = P;
    }
}