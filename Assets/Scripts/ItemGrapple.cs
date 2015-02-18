// ---------------------------------------------------------------------------
// ItemGrapple.cs
// 
// Allows the player to grapple onto items using the triggers on the controller
// or the spacebar on PC
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ItemGrapple : MonoBehaviour
{
    private GameObject m_dirNotif;
    private LineRenderer m_itemLineRenderer;
    private bool m_isGrappling = false;
    private GameObject m_grappleTarget;
    private float m_startGrappleDistance;
    public float m_minimumGrappleDistance = 3.0f;
    private Vector3 m_grappleTargetForce = Vector3.zero;
    private bool m_applyTargetForce = false;
    public float m_forceMultiplier;

    void Start()
    {
        //Init refs
        m_itemLineRenderer = transform.FindChild("ItemSpringJoint").GetComponent<LineRenderer>();
        m_itemLineRenderer.enabled = false;
        m_dirNotif = GetComponent<PlayerControl>().GetDirNotif();
    }

    void Update()
    {
        GrappleItem();
    }

    void LateUpdate()
    {
        if(m_applyTargetForce)
        {
            float D = Vector3.Distance(transform.position, m_grappleTarget.transform.position) - m_startGrappleDistance;
            m_grappleTarget.rigidbody.AddForce(D * m_grappleTargetForce * m_forceMultiplier * Time.deltaTime, ForceMode.Impulse);
            m_applyTargetForce = false;
        }
    }

    private void GrappleItem()
    {
        //Get trigger input
        float triggerInput = Input.GetAxis("GrappleItem");
        bool mouseInput = Input.GetMouseButton(0);

        //If there is no trigger input and we are grappling something
        //then we need to let it go
        if ((triggerInput != 1) && (triggerInput != -1) && !mouseInput)
        {
            m_isGrappling = false;
            m_grappleTarget = null;
            m_itemLineRenderer.enabled = false;
            return;
        }

        //If we arent already grappling an item, see if we can grab a nearby item
        if (!m_isGrappling)
        {
            //Player cant start a new grapple if they aren't pressing a direction with thumbstick
            //m_dirNotif is disabled when no input is received, so just check that
            if (!m_dirNotif.activeInHierarchy)
                return;

            //Grab a list of item grapple targets
            GameObject[] l_grappleItems = GameObject.FindGameObjectsWithTag("GrappleItem");
            //Find the closest object
            GameObject l_closestGrappleTarget = GameObject.Find("System").GetComponent<GetClosestObject>().GetClosest(m_dirNotif, l_grappleItems);
            //Check if this object is close enough to start a new grapple
            float l_closestGrappleDistance = Vector3.Distance(m_dirNotif.transform.position, l_closestGrappleTarget.transform.position);

            if (l_closestGrappleDistance <= m_minimumGrappleDistance)
            {
                //Start a new grapple with the closest target
                m_isGrappling = true;
                m_startGrappleDistance = Vector3.Distance(transform.position, l_closestGrappleTarget.transform.position);
                m_grappleTarget = l_closestGrappleTarget;
                //Set up the line renderer
                m_itemLineRenderer.enabled = true;
                m_itemLineRenderer.SetPosition(0, transform.position);
                m_itemLineRenderer.SetPosition(1, m_grappleTarget.transform.position);
            }
        }
        else
        {//Continue current tether
            //Update line renderer positions
            m_itemLineRenderer.SetPosition(0, transform.position);
            m_itemLineRenderer.SetPosition(1, m_grappleTarget.transform.position);
            //Get distance between grapple target and player
            float l_gd = Vector3.Distance(transform.position, m_grappleTarget.transform.position);
            //If the object is further away than it should be, apply some force so it catches up
            if (l_gd > m_startGrappleDistance)
            {
                //Find the direction of the force we will apply
                // and store it to apply in the LateUpdate function
                m_grappleTargetForce = Vector3.Normalize(transform.position - m_grappleTarget.transform.position);
                m_applyTargetForce = true;
            }
        }
    }
}