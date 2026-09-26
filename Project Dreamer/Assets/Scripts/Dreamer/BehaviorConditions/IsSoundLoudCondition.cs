using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Is Sound Loud", story: "Is the [sound] loud? Find [TargetPosition] if not", category: "Conditions", id: "ac1c4ddc709185b46f6b94ab4f6a5022")]
public partial class IsSoundLoudCondition : Condition
{
    [SerializeReference] public BlackboardVariable<Sound> Sound;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    //Sound passed in as a gameobject by physical script
    //Get the sound type and any other needed data.

    public override bool IsTrue()
    {
        if (Sound.Value.SoundType == SoundType.Loud) { 
            return true; 
        }

        //Update target sound location, used to navigate to
            //Since nav to gameobject will stop at the trigger edge 
        TargetPosition.Value = Sound.Value.gameObject.transform.position;
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
