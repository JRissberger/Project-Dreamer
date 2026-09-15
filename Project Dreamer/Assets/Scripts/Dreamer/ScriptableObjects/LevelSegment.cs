using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSegment", menuName = "Scriptable Objects/LevelSegment")]
public class LevelSegment : ScriptableObject
{
    //Central location of the level segment
    private Vector3 centralPosition;

    //Size of the segment 
    private float radius;

    //Is this level segment included in wander pathing
    private bool isActive;
}
