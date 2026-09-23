using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Push Object", story: "[Star] pushes [Item]", category: "Action", id: "4dfae0f44a29097c615ed266d0fc65ad")]
public partial class PushObjectAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> Item;

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

