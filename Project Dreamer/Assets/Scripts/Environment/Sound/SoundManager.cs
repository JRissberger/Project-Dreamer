using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    //Holds a list of current heard sounds for the behavior tree to access.
    public List<Sound> HeardSounds = new List<Sound>();

    //All sounds currently in the scene
        //TODO: Keep an eye on this and confirm if it's needed
    private List<Sound> AllSounds = new List<Sound>();

    void Start()
    {

    }

    void Update()
    {
        
    }

    //Places a sound at a given location and assigns a type to it
    void SpawnSound(Vector3 pos, SoundType type)
    {
        //Spawn sound PREFAB

        //Assign type
            //IMPORTANT: As of right now this is separate since Sound still inherits from MonoBehavior
            //Might change so type is part of the constructor--need to check if that impacts anything else though

        //Assign manager reference

        //Add spawned sound to all sounds list
    }
}
