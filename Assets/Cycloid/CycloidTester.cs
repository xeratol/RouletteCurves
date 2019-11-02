using UnityEngine;
using UnityEngine.UI;

public class CycloidTester : MonoBehaviour
{
    [SerializeField]
    private Cycloid _curveGenerator = null;
    [SerializeField]
    private LineRenderer _lineRenderer = null;

    [SerializeField]
    private Text _radius1Text = null;
    [SerializeField]
    private Text _radius2Text = null;
    [SerializeField]
    private Text _radius3Text = null;

    private float _radius1 = 400;
    private float _radius2 = -110;
    private float _radius3 = 30;

    public float Radius1
    {
        get => _radius1;
        set
        {
            _radius1Text.text = value.ToString("0");
            _radius1 = value;
            if (Realtime)
            {
                Test();
            }
        }
    }
    public float Radius2
    {
        get => _radius2;
        set
        {
            _radius2Text.text = value.ToString("0");
            _radius2 = value;
            if (Realtime)
            {
                Test();
            }
        }
    }
    public float Radius3
    {
        get => _radius3;
        set
        {
            _radius3Text.text = value.ToString("0");
            _radius3 = value;
            if (Realtime)
            {
                Test();
            }
        }
    }
    public bool Realtime { get; set; } = false;

    private void Awake()
    {
        Debug.Assert(_curveGenerator != null);
        Debug.Assert(_lineRenderer != null);
        Debug.Assert(_radius1Text != null);
        Debug.Assert(_radius2Text != null);
        Debug.Assert(_radius3Text != null);
    }

    private void Start()
    {
        _radius1Text.text = _radius1.ToString("0");
        _radius2Text.text = _radius2.ToString("0");
        _radius3Text.text = _radius3.ToString("0");
    }

    public void Test()
    {
        _curveGenerator.Radius1 = Radius1;
        _curveGenerator.Radius2 = Radius2;
        _curveGenerator.Radius3 = Radius3;

        var points = _curveGenerator.GeneratePoints();
        _lineRenderer.positionCount = points.Length;
        _lineRenderer.SetPositions(points);
    }
}
