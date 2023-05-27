using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldSegment", menuName = "ScriptableObjects/WorldSegmentDataScriptableObject", order = 1)]
public class WorldSegmentData : ScriptableObject
{
    [SerializeField] private Vector3 _worldSegmentScale;
    private float _stepSize;
    [SerializeField] private List<float> _xMoveCoordinates;
    private float _maxStepsCount;
    private float _xSpawnBorderValue;
    public float XSpawnBorderValue => _xSpawnBorderValue;
    [SerializeField] private float _xSpawnOffset;
    public float MaxStepsCount => _maxStepsCount;
    public List<float> XMoveCoordinates => _xMoveCoordinates;
    public Vector3 WorldSegmentScale => _worldSegmentScale;
    public float StepSize => _stepSize;

    private void OnValidate()
    {
        _stepSize = _worldSegmentScale.z * 10;
        _maxStepsCount = (_worldSegmentScale.x / _worldSegmentScale.z - 1) / 2;
        CalculateCoordinates();
        _xSpawnBorderValue = WorldSegmentScale.x * 5 + _xSpawnOffset;
    }

    public int GetCenterOfSegmentCount()
    {
        if (_xMoveCoordinates.Count % 2 == 0)
        {
            return _xMoveCoordinates.Count / 2;
        }
        
        return _xMoveCoordinates.Count / 2 + 1;
    }

    private void CalculateCoordinates()
    {
        if (_stepSize == 0)
        {
            return;
        }

        _xMoveCoordinates.Clear();
        for (float i = -(_stepSize * _maxStepsCount); i <= _stepSize * _maxStepsCount; i += _stepSize)
        {
            _xMoveCoordinates.Add(i);
        }
    }
    
    public int GetNearestCoordinate(float target)
    { 
        var nearestCoordinateCount = 0;
        var bestResult = Mathf.Abs(target - _xMoveCoordinates[0]);
        
        for (int i = 1; i < _xMoveCoordinates.Count; i++)
        {
            var currentResult = Mathf.Abs(target - _xMoveCoordinates[i]);

            if (bestResult > currentResult)
            {
                bestResult = currentResult;
                nearestCoordinateCount = i;
            }
        }

        return nearestCoordinateCount;
    }
}

