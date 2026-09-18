using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class BaseThrowing : MonoBehaviour
{
    [Tooltip("The camera, used for mouse position calculations. If unset, defaults to the main camera (Camera.main, determined on Start()).")]
    [SerializeField] private Camera _camera = null;

    [Tooltip("The position of the \"hand\" where the item is held and thrown from, relative to this object's transform.")]
    [SerializeField] private Vector3 _handOffset = Vector3.zero;

    [Tooltip("The number of segments to break the throwing path into when calculating if it is blocked. Higher numbers are more precise but may reduce performance.")]
    [SerializeField] private int _curveRaycastSegments = 10;

    [Tooltip("Controls the height of the curve that a thrown object travels along. Not 1-to-1 with Unity units; this controls a specific point defining a Bezier curve, and the final height of the parabola will be lower than this.")]
    [SerializeField] private float _throwCurveHeight = 1.0f;

    [Tooltip("How fast an object travels when thrown.")]
    [SerializeField] private float _throwSpeed = 1.0f;

    [Header("Throwing Graphic Settings")]
    [Tooltip("The prefab to display as the target marker when aiming. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _targetMarkerPrefab = null;

    [Tooltip("How much the marker should \"hover\" over the surface it is on, to avoid z-fighting issues.")]
    [SerializeField] private float _targetMarkerHover = 0.01f;

    [Tooltip("The thickness of the curved line displayed when throwing.")]
    [SerializeField] private float _curveDisplayWidth = 0.01f;

    [Tooltip("The number of segments to draw when drawing the curved line indicating a thrown object's path. Higher numbers look better but may reduce performance.")]
    [SerializeField] private int _curveDisplaySegments = 50;

    private InputAction _aimAction;
    private InputAction _throwAction;
    private GameObject _activeMarker = null;
    private BezierCurve _throwCurve;
    private GameObject _heldItem = null;
    private LineRenderer _lineRenderer;

    //For assigning a held item, since picking things up isn't implemented yet
    //[SerializeField] private GameObject _debugItem;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _aimAction = InputSystem.actions.FindAction("Aim");
        _throwAction = InputSystem.actions.FindAction("Throw");
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.widthMultiplier = _curveDisplayWidth;

        //SetHeldItem(_debugItem);
    }

    private void Update()
    {
        if (_heldItem)
        {
            if (_aimAction.IsPressed())
            {
                RaycastHit? clickedInfo = GetClickedRaycast(Mouse.current.position.ReadValue(), LayerMask.GetMask(LayerMask.LayerToName(0)));
                if (clickedInfo.HasValue)
                {
                    Vector3 clickedPoint = clickedInfo.Value.point;
                    Vector3 clickedNormal = clickedInfo.Value.normal;
                    DrawThrowMarker(clickedPoint, clickedNormal);
                    bool validCurve;

                    //Prevent throwing at a wall that is facing away from the thrower
                    //This is done by comparing the surface normal to the vector from the hand to the target point
                    //If these vectors are facing the same way, the dot product will be positive
                    Vector3 dotLeft = clickedPoint - (transform.position + _handOffset);
                    Vector3 dotRight = clickedNormal;
                    //Ignore upwards component - throwing to a higher floor is possible due to gravity
                    dotLeft.y = Mathf.Min(dotLeft.y, 0);
                    dotRight.y = Mathf.Min(dotRight.y, 0);
                    if (Vector3.Dot(dotLeft, dotRight) > 0)
                    {
                        validCurve = false;
                    }
                    else
                    {
                        validCurve = CreateThrowCurve(clickedPoint, clickedNormal);
                    }

                    if (validCurve && _throwAction.WasPressedThisFrame())
                    {
                        ThrowItem();
                        ClearThrowGraphics();
                    }
                }
                else
                {
                    ClearThrowGraphics();
                }
            }
            else
            {
                ClearThrowGraphics();
            }
        }
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
    /// Returns the world position the player clicked on with the mouse,
    /// raycasting from it based on the BaseThrowing script's selected camera.
    /// </summary>
    /// <param name="mousePosition">The position of the mouse on screen.</param>
    /// <param name="clickableLayers">A mask defining which layers will be checked.</param>
    /// <returns>The world position that was clicked on, or null if no object is found.</returns>
    private RaycastHit? GetClickedRaycast(Vector2 mousePosition, LayerMask clickableLayers)
    {
        Ray screenRay = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(screenRay, out hitInfo, Mathf.Infinity, clickableLayers))
        {
            return hitInfo;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Places the throwing target marker on a surface, facing a certain direction.
    /// The marker hovers slightly above the surface to avoid z-fighting issues.
    /// If a marker already exists, it will be moved there; otherwise one will be created.
    /// </summary>
    /// <param name="position">Position of the throwing target marker</param>
    /// <param name="normal">Direction of the marker (for example, upwards if it's on the floor)</param>
    private void DrawThrowMarker(Vector3 position, Vector3 normal) {
        if (_activeMarker == null)
        {
            _activeMarker = Instantiate(_targetMarkerPrefab);
        }
        Vector3 finalPosition = position + normal * _targetMarkerHover;
        _activeMarker.transform.position = finalPosition;
        _activeMarker.transform.rotation = Quaternion.LookRotation(normal);
    }

    /// <summary>
    /// Creates the curve that indicates a thrown object's path, and draws it to the player.
    /// 
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

            Vector3 raycastStart = CurveUtility.EvaluatePosition(_throwCurve, progressStart);
            Vector3 raycastEnd = CurveUtility.EvaluatePosition(_throwCurve, progressEnd);
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
                Debug.Log(raycastEnd);
                break;
            }
        }

        //Draw a line along the curve using the LineRenderer
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
                _lineRenderer.SetPosition(i, CurveUtility.EvaluatePosition(_throwCurve, progress));
            }
        }

        return !isBlocked;
    }

    /// <summary>
    /// Clears the displayed graphics for throwing an item (the marker and the curved path).
    /// </summary>
    private void ClearThrowGraphics()
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }
        _lineRenderer.enabled = false;
    }

    protected void ThrowItem()
    {
        if (_heldItem)
        {
            GameObject throwItem = _heldItem;
            DropItem();
            StartCoroutine(ThrowItemCoroutine(throwItem, _throwCurve));
        }
    }

    private IEnumerator ThrowItemCoroutine(GameObject item, BezierCurve throwCurve)
    {
        for (float i = 0; i < 1; i += Time.deltaTime * _throwSpeed)
        {
            item.transform.position = (Vector3)CurveUtility.EvaluatePosition(throwCurve, i) + transform.position;
            yield return new WaitForEndOfFrame();
        }
        item.transform.position = (Vector3)throwCurve.P3 + transform.position;
    }
}
