using UnityEngine;

public class RoseCurve : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;
    [SerializeField]
    private Vector3 _center = Vector3.zero;
    [SerializeField]
    private float _radius = 1024;
    [SerializeField]
    private int _kNum = 3;
    [SerializeField]
    private int _kDen = 1;

    private bool _recomputeK = true;
    private int _kDenReduced; // also used as number of thread groups
    private float _angleRadInc;
    private int _numSubdivisions;

    private Vector3[] _points = null;
    private ComputeBuffer _pointsBuffer = null;
    private int _generatePointsKernel;
    private const int NUM_THREADS_PER_GROUP = 360; // must match compute shader

    public int kNumerator
    {
        get => _kNum;
        set
        {
            if (_kNum != value)
            {
                _kNum = value;
                _recomputeK = true;
            }
        }
    }
    public int KDenominator
    {
        get => _kDen;
        set
        {
            if (_kDen != value)
            {
                _kDen = value;
                _recomputeK = true;
            }
        }
    }

    void Awake()
    {
        Debug.Assert(_shader != null, "Compute Shader not set");
        Debug.Assert(_kNum > 0, "Invalid kNum");
        Debug.Assert(_kDen > 0, "Invalid kDen");

        _generatePointsKernel = _shader.FindKernel("GeneratePoints");
    }

    void OnDestroy()
    {
        if (_pointsBuffer != null)
        {
            _pointsBuffer.Release();
            _pointsBuffer = null;
        }
    }

    public Vector3[] GeneratePoints()
    {
        if (_recomputeK)
        {
            RecomputeParameters();
            RecreateBuffers();

            _recomputeK = false;
        }

        _shader.SetFloat("radius", _radius);
        _shader.SetFloat("k", _kNum / _kDen);
        _shader.SetFloat("angleRadInc", _angleRadInc);
        _shader.SetFloats("center", new float[]{ _center.x, _center.y, _center.z });
        _shader.SetBuffer(_generatePointsKernel, "points", _pointsBuffer);
        _shader.Dispatch(_generatePointsKernel, _kDenReduced, 1, 1);

        _pointsBuffer.GetData(_points);
        return _points;
    }

    private void RecomputeParameters()
    {
        _kDenReduced = _kDen / Utility.GCD(_kNum, _kDen);
        var maxAngleRad = Mathf.PI * _kDenReduced;
        _numSubdivisions = NUM_THREADS_PER_GROUP * _kDenReduced;
        _angleRadInc = maxAngleRad / _numSubdivisions;
    }

    private void RecreateBuffers()
    {
        var hasChanged = false;

        if (_pointsBuffer == null)
        {
            _pointsBuffer = new ComputeBuffer(_numSubdivisions, sizeof(float) * 3, ComputeBufferType.Default);
            hasChanged = true;
        }
        else if (_pointsBuffer.count != _numSubdivisions)
        {
            _pointsBuffer.Release();
            _pointsBuffer = new ComputeBuffer(_numSubdivisions, sizeof(float) * 3, ComputeBufferType.Default);
            hasChanged = true;
        }

        if (_points == null || _points.Length != _numSubdivisions)
        {
            _points = new Vector3[_numSubdivisions];
            hasChanged = true;
        }

        if (hasChanged)
        {
            _pointsBuffer.SetData(_points);
        }
    }
}
