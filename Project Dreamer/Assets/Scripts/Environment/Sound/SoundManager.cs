using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    //Holds a list of current heard sounds for the behavior tree to access.
    public List<Sound> HeardSounds = new List<Sound>();

    //Default sound prefab
    [SerializeField] private GameObject soundPrefab;

    //DEBUG
    Mouse mouse = null;
    Vector3 newSoundPos = Vector3.zero;

    void Start()
    {
        //DEBUG
        mouse = Mouse.current;
    }

    void Update()
    {
       
        //DEBUG -- Clicking to place sounds in scene, remove later
            //Bypasses inputsystem, will likely need to be commented out when merging
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity))
            {
                newSoundPos = hitResult.point;
                SpawnSound(newSoundPos, SoundType.Loud);
            }
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity))
            {
                newSoundPos = hitResult.point;
                SpawnSound(newSoundPos, SoundType.Soft);
            }
        }
    }

    //Places a sound at a given location and assigns a type to it
    void SpawnSound(Vector3 pos, SoundType type)
    {
        //Spawn sound prefab
        GameObject newSound = Instantiate(soundPrefab, pos, Quaternion.identity);

        //Assign type
            //IMPORTANT: As of right now this is separate since Sound still inherits from MonoBehavior
            //Might change so type is part of the constructor--need to check if that impacts anything else though
        newSound.GetComponent<Sound>().SoundType = type;

        //Assign manager reference
    }
}
