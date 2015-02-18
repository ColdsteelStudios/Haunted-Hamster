// ---------------------------------------------------------------------------
// GetClosestObject.cs
// 
// Takes in a target and a list of items
// Returns an item from the list which is closest to the target
// 
// Original Author: Harley Laurie
// ---------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class GetClosestObject : MonoBehaviour
{
    public GameObject GetClosest(GameObject a_target, GameObject[] a_list)
    {
        GameObject closestItem = a_list[0];
        float closestDistance = Vector3.Distance(a_target.transform.position, a_list[0].transform.position);

        foreach ( GameObject Object in a_list )
        {
            float D = Vector3.Distance(Object.transform.position, a_target.transform.position);
            if ( D < closestDistance )
            {
                closestItem = Object;
                closestDistance = D;
            }
        }
        return closestItem;
    }
}