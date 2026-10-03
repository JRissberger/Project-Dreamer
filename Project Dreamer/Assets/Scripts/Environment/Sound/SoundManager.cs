using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class SoundManager : MonoBehaviour
{
    //Holds a list of current heard sounds for the behavior tree to access.
        //Note. Would this make more sense to be attached to Star?? Something to consider.
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
       /*
        //DEBUG -- Clicking to place sounds in scene, remove later
            //Bypasses inputsystem, will likely need to be commented out when merging
        if (mouse.leftButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity))
            {
                newSoundPos = hitResult.point;
                SpawnSound(newSoundPos, SoundType.Loud, 5, 0.5f, false);
            }
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitResult, Mathf.Infinity))
            {
                newSoundPos = hitResult.point;
                SpawnSound(newSoundPos, SoundType.Soft, 5, 0.5f, false);
            }
        }
       */
    }

    //Places a sound at a given location and assigns a type to it
    public void SpawnSound(Vector3 pos, SoundType type, float audibleRange, float duration, bool isPersistent)
    {
        //Spawn sound prefab
        GameObject newSound = Instantiate(soundPrefab, pos, Quaternion.identity);
        Sound sound = newSound.GetComponent<Sound>();

        //Assign type
        sound.SoundType = type;

        //Assign manager reference
        sound.SoundManager = this;

        //Set audible range
        sound.UpdateAudibleRange(audibleRange);

        //Set timer
        sound.Timer = duration;

        //Is the sound persistent
        sound.IsPersistent = isPersistent;
    }
}
