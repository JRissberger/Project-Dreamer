using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Monster In LOS", story: "Monster in LOS?", category: "Conditions", id: "244d7821cc94d8ba48362423c5847e47")]
public partial class MonsterInLosCondition : Condition
{
    //Monster object/list updated via external script
    //Fairly simple just checking if it's not null/empty, just can't be a comparison node
        //Since there's no way to check value of > 0 or not null. Can only compare object to object.

    public override bool IsTrue()
    {
        return false ;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
