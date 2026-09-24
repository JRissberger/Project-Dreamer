using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Item Interact", story: "[Star] interacts with [Item]", category: "Action", id: "86647ccb64837fc09b42fd9d6ded30dd")]
public partial class ItemInteractAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<GameObject> Item;

    //This is for any unique interactions with items (ie memories, anything more specific)
        //Easiest way would be to have an override method from a parent, or a categorization system that can be accessed. 

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

