using UnityEngine;

public class RoseCurveTester : MonoBehaviour
{
    [SerializeField]
    private RoseCurve _curveGenerator = null;
    [SerializeField]
    private LineRenderer _lineRenderer = null;

    public float kNumerator { get; set; } = 3;
    public float kDenominator { get; set; } = 1;

    private void Awake()
    {
        Debug.Assert(_curveGenerator != null);
        Debug.Assert(_lineRenderer != null);
    }

    public void Test()
    {
        _curveGenerator.kNumerator = (int)kNumerator;
        _curveGenerator.KDenominator = (int)kDenominator;

        Debug.Log(string.Format("Generating Rose Curve with k = {0}/{1}", kNumerator, kDenominator));

        var points = _curveGenerator.GeneratePoints();
        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }
}
