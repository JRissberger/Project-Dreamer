using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FindWanderPosition", story: "[Star] finds a [wanderPosition] in the current map segment.", category: "Action", id: "f210b7b0cc9651b9682353065a084e8a")]
public partial class FindWanderPositionAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Vector3> WanderPosition;
    [SerializeReference] public BlackboardVariable<int> WanderIndex;
    [SerializeReference] public BlackboardVariable<MapSegmentManager> MapSegmentManager;

    protected override Status OnStart()
    {
        //Looks for valid position, possible retry since map segments are circles, so some parts may overlap with walls
        bool foundValidTarget = false;
        Vector3 position = Vector3.zero;

        //Get the current map segment according to the wander index
        MapSegment currentSegment = MapSegmentManager.Value.MapSegments[WanderIndex.Value];

        while (!foundValidTarget)
        {
            //Find a random position in the current segment's boundaries
            position = currentSegment.CenterLoc + UnityEngine.Random.insideUnitSphere * currentSegment.Radius;

            //Is the position on the navmesh
            if (NavMesh.SamplePosition(position, out NavMeshHit hit, currentSegment.Radius, NavMesh.AllAreas))
            {
                foundValidTarget = true;
            }
        }

        //Update blackboard variable
        WanderPosition.Value = position;
        return Status.Success;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

