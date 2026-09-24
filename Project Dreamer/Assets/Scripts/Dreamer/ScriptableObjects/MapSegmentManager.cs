using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MapSegmentManager", menuName = "Scriptable Objects/MapSegmentManager")]
public class MapSegmentManager : ScriptableObject
{
    [SerializeField] private List<MapSegment> mapSegments;

    //Allows access to the list for behavior actions and gizmo display
    public List<MapSegment> MapSegments {  get { return mapSegments; } }
}
