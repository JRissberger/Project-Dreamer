using UnityEngine;
using Unity.Cinemachine;
using Unity.Mathematics;    
using UnityEngine.Splines;

public class TrialCamera : MonoBehaviour
{
    [SerializeField] Vector3 cam3Displacement = new Vector3(11, 9, 0);
    CinemachineSplineDolly spline;
    bool moving;
    int goal = 0;
    float startPos;
    float time = 0;
    float timeToMove = 2f;


    void Start()
    {
        spline = this.GetComponent<CinemachineSplineDolly>();
    }

    void Update()
    {
        if (moving)
        {
            time += Time.deltaTime * (1/timeToMove);
            float t = time * time / (2.0f * ((time * time) - time) + 1.0f);
            
            spline.CameraPosition = Mathf.Lerp(startPos, goal, t);
            Debug.Log(t);

            // If camera has reached the knot (finish transition)
            if (Mathf.Max(spline.CameraPosition, goal) - Mathf.Min(spline.CameraPosition, goal) <= 0.005) { 
                moving = false; 
                time = 0;
                Debug.Log("Reached the end, goal:" + goal + ", position:" + spline.CameraPosition);
                spline.CameraPosition = goal;
                timeToMove = 2f;
            }
        }
    }

    public void MoveTo(int index, float t)
    {
        moving = true;
        startPos = spline.CameraPosition;

        if (t > 0)
            timeToMove = t;

        // If the camera is mid-transition handle time and goal assignment
        if (time > 0) {
            timeToMove *= time;
            if (index > 0)
                goal = (int)spline.CameraPosition + 1;
            else
                goal = (int)spline.CameraPosition;
        }

        else { goal += index; }
        time = 0;
        Debug.Log("Moving Camera to " + goal);
    }
}