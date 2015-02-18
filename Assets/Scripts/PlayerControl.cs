// ---------------------------------------------------------------------------
// PlayerControl.cs
// 
// Allows player to move around the levels using left joystick / wasd
// Can jump with A button on Xbox and spacebar on PC
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    //References
    private Rigidbody m_rigidBody;

    //Direction notifier
    public GameObject m_directionNotifierPrefab;
    private GameObject m_dirNotif;
    private float m_dirNotifDistance = 1.5f;

    //Movement
    public float m_accelerationStrength = 10.0f; // How fast the player accelerates
    public float m_maxVelocity = 7.0f; // Max movement speed of the player (stops applying movement vector when this is reached)
    public float m_jumpStrength = 15.0f; // Strength of the players jump
    private Vector3 m_movementVector = Vector3.zero; // Velocity to apply in FixedUpdate after getting player input
    private bool m_jump = false; // Jump this frame?

    void Start()
    {
        //Init refs
        m_rigidBody = GetComponent<Rigidbody>();
    }

    //Get input and create movement vector for this frame
    void Update()
    {
        Move();
        Jump();
    }

    //Apply force that was calculated during the previous Update() call
    void FixedUpdate()
    {
        //Apply jump force if recieved
        if (m_jump)
            m_rigidBody.AddForce(new Vector3(0.0f, m_jumpStrength, 0.0f) * Time.fixedDeltaTime, ForceMode.Impulse);
        m_rigidBody.AddForce(m_movementVector * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    public GameObject GetDirNotif()
    {
        if(m_dirNotif == null)
        {
            m_dirNotif = GameObject.Instantiate(m_directionNotifierPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            m_dirNotif.SetActive(false);
        }
        return m_dirNotif;
    }

    private void Move()
    {
        //Get thumbstick input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //Update display notifier
        PlaceDirNotif(v, h);
        //Create force to apply
        m_movementVector = new Vector3(h * m_accelerationStrength, 0.0f, 0.0f);
        //Check for velocity cap
        float V = Vector3.Magnitude(rigidbody.velocity);
        if (V >= m_maxVelocity)
            m_movementVector.x = 0.0f;
        //If no input was recieved we want to slow the player down
    }

    private void Jump()
    {
        //Check for jumping
        if (Input.GetButtonDown("Jump") && IsGrounded())
            m_jump = true;
        else
            m_jump = false;

    }

    //Performs a small raycast to check if there is anything beneath the player
    private bool IsGrounded()
    {
        float l_distToGround = collider.bounds.extents.y;
        return Physics.Raycast(transform.position, -Vector3.up, l_distToGround + 0.3f);
    }

    //Places the direction notifier in the correct position
    //Takes in vertical and horizontal input axes
    private void PlaceDirNotif(float a_v, float a_h)
    {
        //If no input was recieved, turn off the notifier
        if ((a_v == 0) && (a_h == 0))
            m_dirNotif.SetActive(false);
        else
        //if we got input, place it in the correct position
        {
            //Activate the notifier
            m_dirNotif.SetActive(true);
            //Find out where to place it
            Vector3 l_inputAxis = new Vector3(a_h, a_v, 0.0f);
            //Make sure the ball is the correct position away
            Vector3 l_dirNotifPos = transform.position + Vector3.ClampMagnitude(l_inputAxis * 10.0f, m_dirNotifDistance);
            //Update its position
            m_dirNotif.transform.position = l_dirNotifPos;
        }
    }
}