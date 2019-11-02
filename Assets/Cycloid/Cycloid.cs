using UnityEngine;

public class Cycloid : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader = null;
    [SerializeField]
    private Vector3 _center = Vector3.zero;
    [SerializeField]
    private float _radius1 = 400;
    [SerializeField]
    private float _radius2 = 100;
    [SerializeField]
    private float _radius3 = 30;

    private bool _recomputeParams = true;
    private int _rho; // also used as number of thread groups
    private int _numSubdivisions;
    private float _angleRadInc;

    private Vector3[] _points = null;
    private ComputeBuffer _pointsBuffer = null;
    private int _generatePointsKernel;
    private const int NUM_THREADS_PER_GROUP = 360; // must match compute shader

    public float Radius1
    {
        get => _radius1;
        set
        {
            if (!Mathf.Approximately(_radius1, value))
            {
                _radius1 = value;
                _recomputeParams = true;
            }
        }
    }

    public float Radius2
    {
        get => _radius2;
        set
        {
            if (!Mathf.Approximately(_radius2, value))
            {
                _radius2 = value;
                _recomputeParams = true;
            }
        }
    }

    public float Radius3
    {
        get => _radius3;
        set
        {
            if (!Mathf.Approximately(_radius3, value))
            {
                _radius3 = value;
            }
        }
    }

    void Awake()
    {
        Debug.Assert(_shader != null, "Compute Shader not set");

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
        if (_recomputeParams)
        {
            RecomputeParameters();
            RecreateBuffers();

            _recomputeParams = false;
        }

        _shader.SetFloat("radius1", _radius1);
        _shader.SetFloat("radius2", _radius2);
        _shader.SetFloat("radius3", _radius3);
        _shader.SetFloat("angleRadInc", _angleRadInc);
        _shader.SetFloats("center", new float[] { _center.x, _center.y, _center.z });
        _shader.SetBuffer(_generatePointsKernel, "points", _pointsBuffer);
        _shader.Dispatch(_generatePointsKernel, _rho, 1, 1);

        _pointsBuffer.GetData(_points);
        return _points;
    }

    private void RecomputeParameters()
    {
        // TODO: rho might be wrong
        _rho = Mathf.Abs((int)_radius2) / Utility.GCD(Mathf.Abs((int)_radius1), Mathf.Abs((int)_radius2));
        var maxAngleRad = 2 * Mathf.PI * _rho;
        _numSubdivisions = NUM_THREADS_PER_GROUP * _rho;
        _angleRadInc = maxAngleRad / _numSubdivisions;
        Debug.Log(_numSubdivisions);
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
