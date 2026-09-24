using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Throw item", story: "Throw [HeldItem] towards [Sound] location.", category: "Action", id: "c0ce95087eb4b9d7039a90ca62f1e1c4")]
public partial class ThrowItemAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> HeldItem;
    [SerializeReference] public BlackboardVariable<GameObject> Sound;

    //May need to access script of held item specifically for throwing. see joseph's comments--public method to call. 

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

