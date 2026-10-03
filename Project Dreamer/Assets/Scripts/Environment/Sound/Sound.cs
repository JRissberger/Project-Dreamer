using Unity.Behavior;
using UnityEngine;

public enum SoundType
{
    Soft,
    Loud,
    None //Fallback
}

public class Sound : MonoBehaviour
{
    [SerializeField] private SoundType soundType = SoundType.None;
    public SoundType SoundType { get { return soundType; } set { soundType = value; } }

    private SoundManager soundManager;
    public SoundManager SoundManager { set { soundManager = value; } }

    private AudioSource audioSource;
    [Tooltip("Audio clip to play if this is a loud sound")]
    [SerializeField] private AudioClip audioClipLoud;
    [Tooltip("Audio clip to play if this is a soft sound")]
    [SerializeField] private AudioClip audioClipSoft;

    [Tooltip("The visual effect that indicates this sound's range")]
    [SerializeField] private GameObject effect;
    [Tooltip("Material for the sound's visual effect (loud sound)")]
    [SerializeField] private Material materialLoud;
    [Tooltip("Material for the sound's visual effect (soft sound)")]
    [SerializeField] private Material materialSoft;

    //Is the sound persistent
    private bool isPersistent = false;
    public bool IsPersistent { get { return isPersistent; } set { isPersistent = value; } }

    //Timer for duration of how long it should be around
    private float timer = 0;
    public float Timer { get { return timer; } set { timer = value; } }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayAudio();

        if (soundType == SoundType.Loud)
        {
            effect.GetComponent<MeshRenderer>().material = materialLoud;
        }
        else if (soundType == SoundType.Soft)
        {
            effect.GetComponent<MeshRenderer>().material = materialSoft;
        }
    }

    void Update()
    {
        //Call timer update if not persistent
        if (!isPersistent)
        {
            UpdateTimer();
        }
    }

    //Adjusts the audible range of the sound (modifying trigger radius)
    public void UpdateAudibleRange(float range)
    {
        //Get spherecollider, update radius
        SphereCollider collider = this.GetComponent<SphereCollider>();
        collider.radius = range;

        //Double range for the effect because its scale is based on diameter, not radius
        effect.transform.localScale = new Vector3(range * 2, range * 2, range * 2);
    }


    //Adds sound to heard list if star enters range
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

    //Update timer, destroy object when timer runs out
    private void UpdateTimer()
    {
        timer -= Time.deltaTime;

        //Destroy self if timer is at 0
        if (timer <= 0)
        {
            soundManager.HeardSounds.Remove(this);
            Destroy(this.gameObject);
        }
    }

    //Play the audio for this sound (loud or soft)
    private void PlayAudio()
    {
        if (soundType == SoundType.Loud)
        {
            audioSource.PlayOneShot(audioClipLoud);
        }
        else if (soundType == SoundType.Soft)
        {
            audioSource.PlayOneShot(audioClipSoft);
        }
    }
}
