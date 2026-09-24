using System.Runtime.CompilerServices;
using UnityEngine;

//This script is exclusively used to render the range for all the map segments
//A class has to inherit from monobehavior to draw gizmos, which neither the manager or segment code does. 
public class MapGizmoRenderer : MonoBehaviour
{
    //Reference to manager
    [SerializeField] private MapSegmentManager segmentManager;
  
    public void OnDrawGizmos()
    {

        //Drawing non wireframe stuff requires using handles which is editor only
        //This check makes sure the code doesn't compile when building (although ideally we pull this out before that)
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.green;

        //Loop through the class list
        for (int i = 0; i < segmentManager.MapSegments.Count; i++)
        {
            MapSegment segment = segmentManager.MapSegments[i];

            //Set up GUI style/content and declare
            GUIContent number = new GUIContent("");
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.black;
            style.fontSize = 18;
            style.fontStyle = FontStyle.Bold;


            //Only draw if the segment is currently active
            if (segment.IsActive)
            {
                //Show radius
                UnityEditor.Handles.DrawSolidDisc(segment.CenterLoc, Vector3.up, segment.Radius);

                //Number in list
                number = new GUIContent($"{i}");
                UnityEditor.Handles.Label(segment.CenterLoc, number, style);
            }
        }
#endif
    }
}
