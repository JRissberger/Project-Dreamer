using Unity.Behavior;
using UnityEngine;

public enum SoundType
{
    Soft,
    Loud,
    None //Fallback
}
//IMPORTANT: rough outline for ai behavior  
//Might be better to not have it branch off monogame, just so we can have a constructor with the enum.
    //Would have to move range calc to sound manager (reasonable! and probably better!)
public class Sound : MonoBehaviour  
{
    [SerializeField] private SoundType soundType = SoundType.None;
    public SoundType SoundType { get { return soundType; } set { soundType = value;  } }

    private GameObject soundManager;
    private GameObject dreamer;
    private BehaviorGraphAgent behaviorTree;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundManager = GameObject.Find("SoundManager");
        dreamer = GameObject.Find("Star");
        behaviorTree = dreamer.GetComponent<BehaviorGraphAgent>();

        //Is sound in range
        float distance = Vector3.Distance(transform.position, dreamer.transform.position);
        Debug.Log(distance);
        if (distance < 10)
        {
            Debug.Log("in range");

            //Add this sound to the dreamer's list
            soundManager.GetComponent<SoundManager>().HeardSounds.Add(this);
            Debug.Log(soundManager.GetComponent<SoundManager>().HeardSounds);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
