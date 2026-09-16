using UnityEngine;
using System;

//Contains the data for an individual map segment for AI pathing
[Serializable]
public class MapSegment
{
    //The central location of the segment
    [SerializeField] private Vector3 centerLoc;
    public Vector3 CenterLoc { get { return centerLoc; } }

    //Size of the segment, from the center
    [SerializeField] private float radius;
    public float Radius { get { return radius; } }

    //Is this segment available for pathing?
    [SerializeField] private bool isActive;
    public bool IsActive { get { return isActive; } set { isActive = value; } }
}
