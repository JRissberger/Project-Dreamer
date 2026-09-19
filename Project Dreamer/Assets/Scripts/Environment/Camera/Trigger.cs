using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class Trigger : MonoBehaviour
{
    [SerializeField] Trigger pair;
    TrialCamera camera;
    [HideInInspector] public bool colliding = false;
    [Tooltip("this should be -1 if it is toward the lower end of the spline position as compared to its pair and 1 if it's on the other")]
    [SerializeField]  int nextPos;
    [Tooltip("if the lerp to the next position should take longer or shorter than the default, write how long in seconds. If not, keep it at 0")]
    [SerializeField]  float timeToMove = 0;
    bool transable = true;

    void Start()
    {
        camera = GameObject.Find("CinemachineCamera").GetComponent<TrialCamera>();
    }

    // Only transition in the desired direction if the player has fully moved from left to right (or vice versa) on the trigger areas

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Capsule" && !pair.colliding)
        {
            transable = false;
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Capsule")
        {
            if (!pair.colliding && transable)
            {
                camera.MoveTo(nextPos, timeToMove);
                pair.transable = true;
            }
            colliding = false;
        }
    }
}