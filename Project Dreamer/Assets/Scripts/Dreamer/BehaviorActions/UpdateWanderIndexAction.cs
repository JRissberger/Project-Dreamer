using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UpdateWanderIndex", story: "[WanderIndex] gets updated.", category: "Action", id: "e3b071ab14abb1c358c5dc1518a3c74d")]
public partial class UpdateWanderIndexAction : Action
{
    [SerializeReference] public BlackboardVariable<int> WanderIndex;
    [SerializeReference] public BlackboardVariable<MapSegmentManager> MapSegmentManager;
    protected override Status OnStart()
    {
        //Increments the wander index by 1
        WanderIndex.Value = WanderIndex.Value += 1;
        //Checks if outside list bounds, rolls over to 0 if so
        if (MapSegmentManager.Value.MapSegments.Count <= WanderIndex.Value)
        {
            WanderIndex.Value = 0;
        }
        Debug.Log($"Wander Index: {WanderIndex.Value}");
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

