using UnityEngine;
using UnityEngine.InputSystem;

public class BaseThrowing : MonoBehaviour
{
    [Tooltip("The camera, used for mouse position calculations. If null, defaults to the main camera (Camera.main, determined on Start()).")]
    [SerializeField] private Camera _camera = null;

    [Tooltip("The prefab to display as the target marker when aiming.")]
    [SerializeField] private GameObject _targetMarkerPrefab = null;

    private InputAction _aimAction;
    private InputAction _throwAction;
    private GameObject _activeMarker = null;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _aimAction = InputSystem.actions.FindAction("Aim");
        _throwAction = InputSystem.actions.FindAction("Throw");
    }

    private void Update()
    {
        if (_aimAction.IsPressed())
        {
            RaycastHit? clickedInfo = GetClickedRaycast(Mouse.current.position.ReadValue(), LayerMask.GetMask(LayerMask.LayerToName(0)));
            if (clickedInfo.HasValue)
            {
                DrawThrowMarker(clickedInfo.Value.point, clickedInfo.Value.normal);
            }
            else
            {
                Destroy(_activeMarker);
            }
        }
        else
        {
            Destroy(_activeMarker);
        }
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
    /// If a marker already exists, it will be moved there; otherwise one will be created.
    /// </summary>
    /// <param name="position">Position of the throwing target marker</param>
    /// <param name="normal">Direction of the marker (for example, upwards if it's on the floor)</param>
    private void DrawThrowMarker(Vector3 position, Vector3 normal) {
        if (_activeMarker == null)
        {
            _activeMarker = Instantiate(_targetMarkerPrefab);
        }
        _activeMarker.transform.position = position;
        _activeMarker.transform.rotation = Quaternion.LookRotation(normal);
    }
}
