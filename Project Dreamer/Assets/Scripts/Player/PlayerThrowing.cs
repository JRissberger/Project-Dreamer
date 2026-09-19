using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls the player's ability to aim and throw objects.
/// Should be attached to the player character.
/// </summary>
public class PlayerThrowing : BaseThrowing
{
    [Header("Player")]
    [Tooltip("The maximum distance the player can throw items, in Unity units. Based on the attached object's position, not considering _handOffset.")]
    [SerializeField] private float _throwRadius = 5.0f;

    [Header("Target Marker")]

    [Tooltip("The prefab to display as the target marker when aiming, if the throw is valid. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _validMarkerPrefab = null;

    [Tooltip("The prefab to display as the target marker when aiming, if the throw is NOT valid. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _invalidMarkerPrefab = null;

    [Tooltip("How much the target marker should \"hover\" over the surface it is on, to avoid z-fighting issues.")]
    [SerializeField] private float _targetMarkerHover = 0.01f;

    [Header("Miscellaneous")]

    [Tooltip("The camera, used for mouse position calculations. If unset, defaults to the main camera (Camera.main, determined on Start()).")]
    [SerializeField] private Camera _camera = null;

    //For testing - assigns which object will be picked up when pressing the "1" key
    [SerializeField] private GameObject _debugItem = null;
    private InputAction _debugPickupAction;

    private GameObject _activeMarker = null;
    private InputAction _aimAction;
    private InputAction _throwAction;

    protected override void Start()
    {
        base.Start();

        _aimAction = InputSystem.actions.FindAction("Aim");
        _throwAction = InputSystem.actions.FindAction("Throw");

        _debugPickupAction = InputSystem.actions.FindAction("Debug 1");

        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private void Update()
    {
        if (_aimAction.IsPressed())
        {
            RaycastHit? clickedInfo = GetClickedRaycast(Mouse.current.position.ReadValue(), LayerMask.GetMask(LayerMask.LayerToName(0)));
            if (clickedInfo.HasValue)
            {
                Vector3 clickedPoint = clickedInfo.Value.point;
                Vector3 clickedNormal = clickedInfo.Value.normal;

                if (Vector3.Distance(transform.position, clickedPoint) <= _throwRadius)
                {
                    if (_heldItem)
                    {
                        //Prevent throwing at a wall that is facing away from the thrower
                        //This is done by comparing the surface normal to the vector from the hand to the target point
                        //If these vectors are facing the same way, the dot product will be positive
                        bool validCurve;
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

                        SetThrowMarker(clickedPoint, clickedNormal, validCurve);

                        if (validCurve && _throwAction.WasPressedThisFrame())
                        {
                            ThrowItem();
                        }
                    }
                    else
                    {
                        SetThrowMarker(clickedPoint, clickedNormal, false);
                    }
                }
                else
                {
                    ResetThrow();
                }
                
            }
            else
            {
                ResetThrow();
            }
        }
        else
        {
            ResetThrow();
        }

        if (_debugPickupAction.WasPressedThisFrame())
        {
            SetHeldItem(_debugItem);
        }
    }

    /// <summary>
    /// Returns the world position the player clicked on with the mouse,
    /// raycasting from it based on the script's selected camera.
    /// </summary>
    /// <param name="mousePosition">The position of the mouse on screen.</param>
    /// <param name="clickableLayers">A mask defining which layers will be checked.</param>
    /// <returns>The world position that was clicked on, or null if no object is found.</returns>
    protected RaycastHit? GetClickedRaycast(Vector2 mousePosition, LayerMask clickableLayers)
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
    /// If a marker already exists, it will be removed in favor of the new one.
    /// </summary>
    /// <param name="position">Position of the throwing target marker</param>
    /// <param name="normal">Direction of the marker (for example, upwards if it's on the floor)</param>
    /// <param name="valid">Whether the throw marker should appear "valid" (normal) or "invalid" (tinted red)</param>
    private void SetThrowMarker(Vector3 position, Vector3 normal, bool valid = true)
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }

        if (valid)
        {
            _activeMarker = Instantiate(_validMarkerPrefab);
        }
        else
        {
            _activeMarker = Instantiate(_invalidMarkerPrefab);
        }

            Vector3 finalPosition = position + normal * _targetMarkerHover;
        _activeMarker.transform.position = finalPosition;
        _activeMarker.transform.rotation = Quaternion.LookRotation(normal);
    }

    /// <summary>
    /// Removes the throwing target marker if one exists.
    /// </summary>
    public void RemoveThrowMarker()
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }
    }

    /// <summary>
    /// Clears the throwing marker and curve, and hides the latter's line renderer.
    /// </summary>
    protected override void ResetThrow()
    {
        base.ResetThrow();
        RemoveThrowMarker();
    }
}
