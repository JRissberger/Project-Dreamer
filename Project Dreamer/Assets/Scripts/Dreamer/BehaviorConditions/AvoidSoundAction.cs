using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Avoid Sound", story: "Pick [location] away from [sound]", category: "Action", id: "624646695930296b563165d504071e80")]
public partial class AvoidSoundAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Location;
    [SerializeReference] public BlackboardVariable<Sound> Sound;
    [SerializeReference] public BlackboardVariable<GameObject> Star;

    protected override Status OnStart()
    {
        //Find most efficient direction to put distance between Star and the sound

        //Direction vector opposite of the loud sound
        Vector3 soundDir = Sound.Value.gameObject.transform.position - Star.Value.transform.position;
        Debug.Log(soundDir);

        //Normalize and invert 
        Vector3 runDir = Vector3.Normalize(soundDir) * -1;
        Debug.Log("Inverted: " + runDir);

            //Normalized, calc actual distance on top of this

            //Probably want a little bit of angle variation for realism's sake

            //Potentially a speed increase for Star to show they're running?

        //Update target location
        Location.Value = soundDir;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

