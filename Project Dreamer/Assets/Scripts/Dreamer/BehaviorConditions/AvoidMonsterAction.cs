using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AvoidMonster", story: "PIck [Location] away from [monster]", category: "Action", id: "3fd9da74db657e47fed4e7249506c068")]
public partial class AvoidMonsterAction : Action
{
    [SerializeReference] public BlackboardVariable<Vector3> Location;
    [SerializeReference] public BlackboardVariable<GameObject> Monster;

    protected override Status OnStart()
    {
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

