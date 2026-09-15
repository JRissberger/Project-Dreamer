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


    /* Manager -- holds list of class objects, serialize fields to add in values in editor
     * Then adds all those to the scriptable object at runtime--scriptable object holds a list of these objects.
     * Might be simpler if there'sa way to define/add the class to the inspector--check this!!
     */
}
