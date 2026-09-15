using UnityEngine;
using UnityEngine.Splines;

public class DrawSpline : MonoBehaviour
{
    [SerializeField] private int renderPoints = 50;

    private Spline _spline;
    private LineRenderer _lineRenderer;

    private void Start()
    {
        _spline = GetComponent<SplineContainer>()[0];
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = renderPoints;
    }

    private void Update()
    {
        for (int i = 0; i < renderPoints; i++)
        {
            float progress = i / (float)(renderPoints - 1);
            _lineRenderer.SetPosition(i, _spline.EvaluatePosition(progress));
        }
    }
}
