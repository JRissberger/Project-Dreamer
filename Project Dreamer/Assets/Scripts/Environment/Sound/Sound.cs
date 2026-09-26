using Unity.Behavior;
using UnityEngine;

public enum SoundType
{
    Soft,
    Loud,
    None //Fallback
}

//Might be better to not have it branch off monogame, just so we can have a constructor with the enum.
    //Would have to move range calc to sound manager (reasonable! and probably better!) (I lied it might stay here)
public class Sound : MonoBehaviour  
{
    [SerializeField] private SoundType soundType = SoundType.None;
    public SoundType SoundType { get { return soundType; } set { soundType = value;  } }

    private GameObject soundManager;
    private GameObject dreamer;

    //Add get/set for soundManager

    //Get/set for radius--set in manager, default of 1 on prefab

    //Bool for if it's persistent or not
    //Timer for duration of how long it should be around

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TODO: better to assign via manager when spawned
        soundManager = GameObject.Find("SoundManager");

        //TODO: This will all get removed
        dreamer = GameObject.Find("Star");

        //Is sound in range
        float distance = Vector3.Distance(transform.position, dreamer.transform.position);

        if (distance < 10)
        {
            //Add this sound to the dreamer's list
            soundManager.GetComponent<SoundManager>().HeardSounds.Add(this);
        }

        //Call radius adjustment
            //TODO: may need a delay, potential race condition with soundmanager 
    }

    void Update()
    {
        //Call timer update
    }

    //Method--adjust trigger radius
    //Get this gameobject's spherecollider
    //Use spherecollider.radius to adjust


    //On trigger enter
    //Check if it's Star (could use tag or just check object directly? tag makes more sense though)
    //Add this sound to the heard sounds list on manager

    //On trigger exit
    //Check if it's star, same as above
    //Remove sound from list
    //IMPORTANT: check to make sure that this doesn't interrupt the current bt branch (ie moving out of range)

    //Update timer
        // -deltatime from timer
        //If it's at or below 0, destroy object (remove from manager list first)
        //NOTE: how to handle if a sound ends as Star's moving towards it? Need to check if BB saves a copy or a reference. Could cause null issue
            //Would the sound need to know if it's being targeted?
}
