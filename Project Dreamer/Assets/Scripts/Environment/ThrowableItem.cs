using UnityEngine;

public class ThrowableItem : MonoBehaviour
{
    [Header("Sound Settings")]

    [Tooltip("The type of sound this object creates when landing. \"None\" will create no sound at all.")]
    [SerializeField] private SoundType _soundType = SoundType.None;

    [Tooltip("How far away this object's landing sound is audible from.")]
    [SerializeField] private float _soundRadius = 1.0f;

    [Tooltip("How long the sound this object makes will last, in seconds.")]
    [SerializeField] private float _soundDuration = 1.0f;

    [Header("Other")]

    [Tooltip("Whether this item disappears after being thrown.")]
    [SerializeField] private bool _willDespawn = false;

    private SoundManager _soundManager;

    private void Start()
    {
        _soundManager = FindAnyObjectByType<SoundManager>();
    }

    /// <summary>
    /// A method to be called when this item lands after being thrown.
    /// This will create a SoundManager sound, with parameters based on the values set in the inspector.
    /// It will also destroy this item if "Will Despawn" is checked.
    /// </summary>
    public void Land()
    {
        if (_soundType != SoundType.None)
        {
            _soundManager.SpawnSound(transform.position, _soundType, _soundRadius, _soundDuration, false);
        }

        LandCustom();

        if (_willDespawn)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// An empty method that subclasses classes can override to implement custom landing behavior for an item.
    /// Called near the end of Land(), after the object has made its sound.
    /// </summary>
    protected virtual void LandCustom()
    {
        //Do nothing
    }
}
