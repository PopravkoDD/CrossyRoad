using System;
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
        var nice = _worldSegmentScale.x / _worldSegmentScale.z;
        _maxStepsCount = (nice - 1) / 2;
        Debug.Log(_maxStepsCount);
        CalculateCoordinates();
        _xSpawnBorderValue = WorldSegmentScale.x * 5 + _xSpawnOffset;

    }

    public List<float> GetXMoveCoordinates()
    {
        var copy = new List<float>(_xMoveCoordinates);
        return copy;
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

    public float GetNearestCoordinate(float target)
    { 
        var nearestCoordinate = _xMoveCoordinates[0];
        var bestResult = Mathf.Abs(target - _xMoveCoordinates[0]);
        
        for (int i = 1; i < _xMoveCoordinates.Count; i++)
        {
            var currentResult = Mathf.Abs(target - _xMoveCoordinates[i]);

           // Debug.Log($"{bestResult}      {currentResult}");
            if (bestResult > currentResult)
            {
                bestResult = currentResult;
                nearestCoordinate = _xMoveCoordinates[i];
            }
        }

        return nearestCoordinate;
    }
}

