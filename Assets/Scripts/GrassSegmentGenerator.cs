using System.Collections.Generic;
using UnityEngine;

public class GrassSegmentGenerator : WorldSegmentGenerationStrategy
{
    [SerializeField] private int _minNumberOfObstacles;
    [SerializeField] private int _maxNumberOfObstacles;
    private int _currentObstacleNumber;
    private List<GameObject> _obstacles; 
    public override void GenerateSegment()
    {
        _obstacles = new List<GameObject>();
        var coordinates = new List<float>(_worldSegmentData.XMoveCoordinates);
        _maxNumberOfObstacles = (int)_worldSegmentData.MaxStepsCount;
        for (int i = 0; i < Random.Range(_minNumberOfObstacles, _maxNumberOfObstacles); i++)
        {
            var random = Random.Range(0, coordinates.Count);
            var obstacle = _pool.GetAvailableObstacle((Random.value > 0.5f) ? 
                ObstacleType.TreeObstacle : ObstacleType.RockObstacle);
            obstacle.transform.position = new Vector3(coordinates[random], transform.position.y, transform.position.z);
            coordinates.RemoveAt(random);
            _obstacles.Add(obstacle);
        }
    }

    public override void DisableSegment()
    {
        _isAvailable = true;
        gameObject.SetActive(false);
        
        if (_obstacles == null)
        {
            return;
        }

        foreach (var obstacle in _obstacles)
        {
            obstacle.SetActive(false);
        }
        
        _obstacles.Clear();
    }
}
