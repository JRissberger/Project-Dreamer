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
    public SoundType SoundType { get { return soundType; } set { soundType = value; } }

    private SoundManager soundManager;
    public SoundManager SoundManager { set { soundManager = value; } }

    //Bool for if it's persistent or not
    //Timer for duration of how long it should be around

    void Update()
    {
        //Call timer update if not persistent
    }

    //Adjusts the audible range of the sound (modifying trigger radius)
    public void UpdateAudibleRange(float range)
    {
        //Get spherecollider, update radius
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.radius = range;
    }


    //On trigger enter
    //Check if it's Star (could use tag or just check object directly? tag makes more sense though)
    //Add this sound to the heard sounds list on manager
    private void OnTriggerEnter(Collider other)
    {

        //Check that it's star that entered
        if (other.CompareTag("Star"))
        {
            //Add this sound to the manager's audible sounds list
            soundManager.HeardSounds.Add(this);
        }
    }

    //Removes this sound from audible list when star leaves range
    private void OnTriggerExit(Collider other)
    {
        //Check that it's Star who left
        if (other.CompareTag("Star"))
        {
            //Remove from list
            soundManager.HeardSounds.Remove(this);
        }
    }

    //Update timer
    // -deltatime from timer
    //If it's at or below 0, destroy object (remove from manager list first)
    //NOTE: how to handle if a sound ends as Star's moving towards it? Need to check if BB saves a copy or a reference. Could cause null issue
    //Would the sound need to know if it's being targeted?

    //Are we having the actual sound object play a noise?
    //If so, method here for data surrounding playing said noise
}
