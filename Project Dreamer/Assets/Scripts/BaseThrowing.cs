using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class BaseThrowing : MonoBehaviour
{
    [Tooltip("The camera, used for mouse position calculations. If null, defaults to the main camera (Camera.main, determined on Start()).")]
    [SerializeField] private Camera _camera = null;

    [Tooltip("The position of the \"hand\" where the item is held and thrown from, relative to this object's transform.")]
    [SerializeField] private Vector3 _handOffset = Vector3.zero;

    [Header("Throwing Graphic Settings")]
    [Tooltip("The prefab to display as the target marker when aiming. The prefab should be oriented such that the +Z axis points into the surface the marker is on.")]
    [SerializeField] private GameObject _targetMarkerPrefab = null;

    [Tooltip("How much the marker should \"hover\" over the surface it is on, to avoid z-fighting issues.")]
    [SerializeField] private float _targetMarkerHover = 0.01f;

    [Tooltip("The number of segments to break the throwing path into when calculating if it is blocked. Higher numbers are more precise but may reduce performance.")]
    [SerializeField] private int _splineRaycastSegments = 10;

    [Tooltip("The thickness of the curved line displayed when throwing.")]
    [SerializeField] private float _splineDisplayWidth = 0.01f;

    [Tooltip("The number of segments to draw when drawing the curved line indicating a thrown object's path. Higher numbers look better but may reduce performance.")]
    [SerializeField] private int _splineDisplaySegments = 50;

    private InputAction _aimAction;
    private InputAction _throwAction;
    private GameObject _activeMarker = null;
    private SplineContainer _splineContainer;
    private LineRenderer _lineRenderer;

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _aimAction = InputSystem.actions.FindAction("Aim");
        _throwAction = InputSystem.actions.FindAction("Throw");
        _splineContainer = gameObject.AddComponent<SplineContainer>();
        _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = false;
        _lineRenderer.widthMultiplier = _splineDisplayWidth;
    }

    private void Update()
    {
        if (_aimAction.IsPressed())
        {
            RaycastHit? clickedInfo = GetClickedRaycast(Mouse.current.position.ReadValue(), LayerMask.GetMask(LayerMask.LayerToName(0)));
            if (clickedInfo.HasValue)
            {
                DrawThrowMarker(clickedInfo.Value.point, clickedInfo.Value.normal);
                bool validSpline = CreateThrowSpline(clickedInfo.Value.point, clickedInfo.Value.normal);
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
    /// Creates the spline that indicates a thrown object's path, and draws it to the player.
    /// 
    /// </summary>
    /// <param name="position">Where the object is being thrown</param>
    /// <param name="normal">The normal of the surface the object is being thrown at</param>
    /// <returns>False if the throw path is blocked; true otherwise</returns>
    private bool CreateThrowSpline(Vector3 position, Vector3 normal)
    {
        //Create the spline
        if (_splineContainer.Splines.Count == 0)
        {
            _splineContainer.AddSpline();
        }
        Spline spline = _splineContainer.Spline;
        spline.Clear();
        BezierKnot startKnot = new BezierKnot(_handOffset, Vector3.zero, Vector3.up);
        spline.Add(startKnot);
        BezierKnot endKnot = new BezierKnot(position - transform.position, normal, Vector3.zero);
        spline.Add(endKnot);

        bool isBlocked = false;
        //These two variables are used when drawing the line if it is blocked
        float blockedAtProgress = 0;
        Vector3 blockedAtPoint = Vector3.zero;

        //Check if the spline is blocked by raycasting between points on it
        for (int i = 0; i < _splineRaycastSegments; i++)
        {
            float progressStart = i / (float)_splineRaycastSegments;
            float progressEnd = (i + 1) / (float)_splineRaycastSegments;
            //Stop a bit short of the surface we're throwing at, so it isn't detected as blocking the throw
            progressEnd = Mathf.Min(progressEnd, 0.99f);

            Vector3 raycastStart = spline.EvaluatePosition(progressStart);
            Vector3 raycastEnd = spline.EvaluatePosition(progressEnd);
            //Spline is in local space; raycast uses world space
            raycastStart += transform.position + _handOffset;
            raycastEnd += transform.position + _handOffset;

            Vector3 raycastRay = raycastEnd - raycastStart;

            RaycastHit hitInfo;
            if (Physics.Raycast(raycastStart, raycastRay, out hitInfo, Vector3.Magnitude(raycastRay), LayerMask.GetMask(LayerMask.LayerToName(0))))
            {
                isBlocked = true;
                blockedAtProgress = progressStart;
                blockedAtPoint = hitInfo.point;
                break;
            }
        }

        //Draw a line along the spline using the LineRenderer
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = 0;
        for (int i = 0; i < _splineDisplaySegments + 1; i++)
        {
            _lineRenderer.positionCount++;
            float progress = i / (float)_splineDisplaySegments;
            if (isBlocked && progress > blockedAtProgress)
            {
                _lineRenderer.SetPosition(i, blockedAtPoint - transform.position - _handOffset);
                break;
            }
            else
            {
                _lineRenderer.SetPosition(i, spline.EvaluatePosition(progress));
            }
        }

        return !isBlocked;
    }

    private void ClearThrowGraphics()
    {
        if (_activeMarker)
        {
            Destroy(_activeMarker);
        }
        _lineRenderer.enabled = false;
    }
}
