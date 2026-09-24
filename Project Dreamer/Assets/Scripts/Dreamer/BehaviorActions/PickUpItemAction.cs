using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Pick Up Item", story: "[Star] picks up [item]", category: "Action", id: "7fc043eb87c6000d98adb188f0f65f94")]
public partial class PickUpItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> Item;

    //Should update blackboard variables: Is Holding Item, and Current Held Item

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

