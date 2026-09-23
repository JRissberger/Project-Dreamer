using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Item Pushable", story: "Is [Item] pushable?", category: "Conditions", id: "1055c5b1919c0c09d7a26ae2594ea048")]
public partial class ItemPushableCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Item;

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
