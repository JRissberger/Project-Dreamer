using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "IsItemThrowable", story: "Is [item] throwable?", category: "Conditions", id: "fd2c753c2fcaa36bd98c9a4a7db074e4")]
public partial class IsItemThrowableCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Item;

    //Evaluate by pulling needed script from item object and obtaining needed data.

    public override bool IsTrue()
    {
        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
