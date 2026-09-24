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
        //Can only set index as an active node
        bool activeNode = false;

        while (!activeNode)
        {
            //Increments the wander index by 1
            WanderIndex.Value = WanderIndex.Value += 1;

            //Checks if outside list bounds, rolls over to 0 if so
            if (MapSegmentManager.Value.MapSegments.Count <= WanderIndex.Value)
            {
                WanderIndex.Value = 0;
            }

            //Is the new map segment an active node?
            activeNode = MapSegmentManager.Value.MapSegments[WanderIndex.Value].IsActive;
        }
        
        
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

