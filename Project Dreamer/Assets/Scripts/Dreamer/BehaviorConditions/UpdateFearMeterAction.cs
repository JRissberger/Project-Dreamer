using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Update Fear Meter", story: "Update [FearMeter] by [Amount]", category: "Action", id: "9f26c744092177f94020907d41f9ecc6")]
public partial class UpdateFearMeterAction : Action
{
    [SerializeReference] public BlackboardVariable<float> FearMeter;
    [SerializeReference] public BlackboardVariable<float> Amount;

    //Increments the fear meter by the amount specified in amount

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

