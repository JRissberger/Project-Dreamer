using UnityEngine;
using System;

//Contains the data for an individual map segment for AI pathing
[Serializable]
public class MapSegment
{
    //The central location of the segment
    public Vector3 centerLoc;

    //Size of the segment, from the center
    public float radius;

    //Is this segment available for pathing?
    public bool isActive;
}
