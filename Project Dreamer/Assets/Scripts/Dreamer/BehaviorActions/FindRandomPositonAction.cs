using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Random Positon", story: "[Star] looks for random position", category: "Action/Navigation", id: "b54dd42f881b4d44495dae1d196e0f53")]
public partial class FindRandomPositonAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Vector3> TargetPosition;

    protected override Status OnStart()
    {
        //Looks for valid position, possible retry since the random area is a circle and our rooms aren't circular
        bool foundValidTarget = false;
        Vector3 randomPosition = Vector3.zero;

        while (!foundValidTarget)
        {
            //Gets a random position
            //Using fixed radius for now, but could add Star's position to it to have it be in a radius around them instead
            randomPosition = UnityEngine.Random.insideUnitSphere * 15f; //Multiplied by radius

            //Is the position on the navmesh
            if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas))
            {
                foundValidTarget = true;
            }
        }

        //Update blackboard variable
        TargetPosition.Value = randomPosition;
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

