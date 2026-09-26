using System;
using NUnit.Framework;
using Unity.Behavior;
using UnityEngine;
using System.Collections.Generic;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Sound Heard", story: "Sound heard? [Star] [SoundManager] [TargetSound]", category: "Conditions", id: "0d54f36f5442cb9cf2faf000615d9f0f")]
public partial class SoundHeardCondition : Condition
{
    [SerializeReference] public BlackboardVariable<SoundManager> SoundManager;
    [SerializeReference] public BlackboardVariable<GameObject> Star;
    [SerializeReference] public BlackboardVariable<Sound> TargetSound;

    public override bool IsTrue()
    {
        List<Sound> soundList = SoundManager.Value.HeardSounds;

        //Are there current sounds in hearing range?
        if (soundList.Count > 0)
        {
            Debug.Log(soundList.Count);

            Sound closestSound = soundList[0];
            float distance = Mathf.Infinity;

            //Compare sounds, find the shortest distance
            for (int i = 0; i < soundList.Count; i++)
            {
                //Get the distance between the current sound and Star
                float currDistance = Vector3.Distance(Star.Value.transform.position, soundList[i].gameObject.transform.position);

                //If it's less than the current min, update
                if (currDistance < distance)
                {
                    distance = currDistance;
                    closestSound = soundList[i];
                }
            }

            //Update blackboard with target sound
            TargetSound.Value = closestSound;

            return true;

        }

        //List is empty, nothing heard
        return false;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
