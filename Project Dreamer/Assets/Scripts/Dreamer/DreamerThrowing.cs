using UnityEngine;

public class DreamerThrowing : BaseThrowing
{
    [SerializeField] GameObject _debugItem;

    protected override void Start()
    {
        base.Start();

        SetHeldItem(_debugItem);
        ThrowItemAtTry(new Vector3(-4, 0, 4), Vector3.up); //throwing path is blocked in the test scene; this should not throw the item
    }

    /// <summary>
    /// Throws the currently held item at the given position.
    /// Does not check for obstructions or distance.
    /// </summary>
    /// <param name="position">The position to throw the item at.</param>
    public void ThrowItemAtForced(Vector3 position)
    {
        //The surface normal is only used in the validity check, so it can be set to zero here.
        CreateThrowCurve(position, Vector3.zero);
        ThrowItem();
    }

    /// <summary>
    /// Attempts to throw the currently held item at the given position and surface normal.
    /// If the throw is obstructed, out of range, or otherwise invalid, the item will not be thrown.
    /// </summary>
    /// <param name="position">The position to throw the item at.</param>
    /// <param name="normal">The normal of the surface the item is being thrown at.</param>
    /// <param name="maxRange">The maximum allowed range of the throw, measuring from the thrower's position to the position parameter</param>
    /// <returns>True if the throw was successful, false if it was blocked or otherwise invalid.</returns>
    public bool ThrowItemAtTry(Vector3 position, Vector3 normal, float maxRange = Mathf.Infinity)
    {
        if (Vector3.Distance(position, transform.position) <= maxRange && CreateThrowCurve(position, normal))
        {
            ThrowItem();
            return true;
        }
        return false;
    }
}
