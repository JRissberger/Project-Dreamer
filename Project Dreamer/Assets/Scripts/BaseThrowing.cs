using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Splines;

/// <summary>
/// Contains base functionality for throwing objects.
/// Should not be used directly.
/// See DreamerThrowing for a basic implementation with a hook for throwing an object.
/// </summary>
public class BaseThrowing : MonoBehaviour
{
    [Header("Throwing Settings")]

    [Tooltip("The position of the \"hand\" where the item is held and thrown from, relative to this object's transform.")]
    [SerializeField] protected Vector3 _handOffset = Vector3.zero;

    [Tooltip("The number of segments to break the throwing path into when calculating if it is blocked. Higher numbers are more precise but may reduce performance.")]
    [SerializeField] private int _curveRaycastSegments = 10;

    [Tooltip("Controls the height of the curve that a thrown object travels along. Not 1-to-1 with Unity units; this controls a specific point defining a Bezier curve, and the final height of the parabola will be lower than this.")]
    [SerializeField] private float _throwCurveHeight = 1.0f;

    [Tooltip("How fast an object travels when thrown.")]
    [SerializeField] private float _throwSpeed = 1.0f;

    [Header("Curve Display")]

    [Tooltip("Whether to display the curve that displays the path of a thrown item.")]
    [SerializeField] private bool _drawCurve = false;

    [Tooltip("The thickness of the curved line that is optionally displayed when throwing.")]
    [SerializeField] private float _curveDisplayWidth = 0.01f;

    [Tooltip("The number of segments to draw for the curved line that is optionally displayed when throwing. Higher numbers look better but may reduce performance.")]
    [SerializeField] private int _curveDisplaySegments = 50;

    //Called when an object is finished being thrown and lands on the ground (or another surface).
    [System.NonSerialized] public UnityEvent _throwLandEvent = new UnityEvent();

    private BezierCurve? _throwCurve;
    protected GameObject _heldItem = null;
    private LineRenderer _lineRenderer;

    protected virtual void Start()
    {
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.widthMultiplier = _curveDisplayWidth;
        _lineRenderer.enabled = false;
    }

    /// <summary>
    /// Gets the currently held item.
    /// </summary>
    /// <returns>The held item.</returns>
    public GameObject GetHeldItem()
    {
        return _heldItem;
    }

    /// <summary>
    /// Sets the currently held item, parenting it to this object's Transform.
    /// If an item is already being held, it will be dropped in favor of the new one.
    /// </summary>
    /// <param name="item">The GameObject to set as the held item.</param>
    public void SetHeldItem(GameObject item)
    {
        if (item.CompareTag("Throwable"))
        {
            if (GetHeldItem())
            {
                DropItem();
            }

            _heldItem = item;
            _heldItem.transform.SetParent(transform, false);
            _heldItem.transform.localPosition = _handOffset;
        }
        else
        {
            Debug.LogWarning("Tried to call SetHeldItem on an item not tagged as \"Throwable\"! This item will be ignored.");
        }
    }

    /// <summary>
    /// Drops the currently held item, unparenting it from this object's Transform and putting it at the scene root.
    /// </summary>
    public void DropItem()
    {
        _heldItem.transform.SetParent(null, true);
        _heldItem = null;
    }

    /// <summary>
    /// Creates and checks the curve that indicates a thrown object's path.
    /// </summary>
    /// <param name="position">Where the object is being thrown</param>
    /// <param name="normal">The normal of the surface the object is being thrown at</param>
    /// <returns>False if the throw path is blocked; true otherwise</returns>
    public bool CreateThrowCurve(Vector3 position, Vector3 normal)
    {
        //Create the Bezier curve
        Vector3 startPos = _handOffset;
        Vector3 endPos = position - transform.position;

        Collider heldCollider = _heldItem.GetComponent<Collider>();
        float heldRadius = 0;
        if (heldCollider)
        {
            Vector3 heldSize = heldCollider.bounds.extents;
            heldRadius = Mathf.Max(heldSize.x, heldSize.y, heldSize.z);
            //Offset the ending position by the extent of the bounding box, so the spherecast doesn't clip into the surface being thrown at
            //TODO: Need a better way to check this! Right now, thrown objects with oblong dimensions might stop short of their target.
            endPos += normal * heldRadius;
        }

        //TODO: Look closer at this formula if the throwing curve is too unrealistic
        // Right now it just takes the point between the start and end and slides it upwards
        Vector3 midPos = (startPos + endPos) / 2 + Vector3.up * _throwCurveHeight;
        _throwCurve = new BezierCurve(startPos, midPos, endPos);

        bool isBlocked = false;
        //These two variables are used when drawing the line if it is blocked
        float blockedAtProgress = 0;

        //Check if the curve is blocked by raycasting between points on it
        for (int i = 0; i < _curveRaycastSegments; i++)
        {
            float progressStart = i / (float)_curveRaycastSegments;
            float progressEnd = (i + 1) / (float)_curveRaycastSegments;
            //Stop a bit short of the surface we're throwing at, so it isn't detected as blocking the throw
            progressEnd = Mathf.Min(progressEnd, 0.99f);

            Vector3 raycastStart = CurveUtility.EvaluatePosition(_throwCurve.Value, progressStart);
            Vector3 raycastEnd = CurveUtility.EvaluatePosition(_throwCurve.Value, progressEnd);
            //Curve is in local space; raycast uses world space
            raycastStart += transform.position;
            raycastEnd += transform.position;

            Vector3 raycastRay = raycastEnd - raycastStart;

            RaycastHit hitInfo;

            bool raycastResult = false;
            
            if (heldCollider)
            {
                raycastResult = Physics.SphereCast(raycastStart, heldRadius, raycastRay, out hitInfo, Vector3.Magnitude(raycastRay), LayerMask.GetMask(LayerMask.LayerToName(0)));
            }
            else
            {
                raycastResult = Physics.Raycast(raycastStart, raycastRay, out hitInfo, Vector3.Magnitude(raycastRay), LayerMask.GetMask(LayerMask.LayerToName(0)));
            }

            if (raycastResult)
            {
                isBlocked = true;
                blockedAtProgress = progressStart;
                break;
            }
        }

        //Draw a line along the curve using the LineRenderer
        if (_drawCurve)
        {
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 0;
            for (int i = 0; i < _curveDisplaySegments + 1; i++)
            {
                float progress = i / (float)_curveDisplaySegments;
                if (isBlocked && progress > blockedAtProgress)
                {
                    break;
                }
                else
                {
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPosition(i, CurveUtility.EvaluatePosition(_throwCurve.Value, progress));
                }
            }
        }
        

        return !isBlocked;
    }

    /// <summary>
    /// Clears the throwing curve and hides its associated line renderer.
    /// </summary>
    protected virtual void ResetThrow()
    {
        _throwCurve = null;
        _lineRenderer.enabled = false;
    }

    /// <summary>
    /// Throws the currently held item along the throwing curve generated by CreateThrowCurve().
    /// Does not check for the path being blocked (this is handled by CreateThrowCurve()).
    /// Does nothing and logs a warning if there is no held item or no current throwing curve.
    /// </summary>
    protected void ThrowItem()
    {
        if (_heldItem && _throwCurve.HasValue)
        {
            GameObject throwItem = _heldItem;
            DropItem();
            StartCoroutine(ThrowItemCoroutine(throwItem, _throwCurve.Value));
            ResetThrow();
        }
        else if (!_heldItem)
        {
            Debug.LogWarning("ThrowItem() was called without any held item! This call will be ignored.");
        }
        else
        {
            Debug.LogWarning("ThrowItem() was called without a throw curve set! This call will be ignored.");
        }
    }

    /// <summary>
    /// Coroutine for moving the thrown item along a path.
    /// </summary>
    /// <param name="item">The item to throw.</param>
    /// <param name="throwCurve">The curve to throw the item along.</param>
    /// <returns></returns>
    private IEnumerator ThrowItemCoroutine(GameObject item, BezierCurve throwCurve)
    {
        for (float i = 0; i < 1; i += Time.deltaTime * _throwSpeed)
        {
            item.transform.position = (Vector3)CurveUtility.EvaluatePosition(throwCurve, i) + transform.position;
            yield return new WaitForEndOfFrame();
        }
        item.transform.position = (Vector3)throwCurve.P3 + transform.position;
        _throwLandEvent.Invoke();
    }
}
