using System.Collections.Generic;
using UnityEngine;

public class SegmentsPool : Singleton<SegmentsPool>
{
    [SerializeField] private SegmentTypeSegmentGenerationStrategyDictionary _segmentsPrefabs;
    [SerializeField] private ObstacleTypeGameObjectDictionary _obstaclePrefabs;
    private Dictionary<SegmentType, List<WorldSegmentGenerationStrategy>> _segmentsPool;
    private Dictionary<ObstacleType, List<GameObject>> _obstaclesPool;
    [SerializeField] private List<WorldSegmentGenerationStrategy> _activeSegments;

    private void Awake()
    {
        _activeSegments = new List<WorldSegmentGenerationStrategy>();
        _segmentsPool = new Dictionary<SegmentType, List<WorldSegmentGenerationStrategy>>();
        _obstaclesPool = new Dictionary<ObstacleType, List<GameObject>>();

        foreach (var segment in _segmentsPrefabs)
        {
            _segmentsPool.Add(segment.Key, new List<WorldSegmentGenerationStrategy>());
            AddAndReturnNewSegment(segment.Key).DisableSegment();
        }
        
        foreach (var obstacle in _obstaclePrefabs)
        {
            _obstaclesPool.Add(obstacle.Key, new List<GameObject>());
            AddAndReturnNewObstacle(obstacle.Key).SetActive(false);
        }

    }


    public GameObject GetAvailableObstacle(ObstacleType obstacleType)
    {
        foreach (var obstacle in _obstaclesPool[obstacleType])
        {
            if (!obstacle.activeSelf)
            {
                obstacle.SetActive(true);
                return obstacle;
            }
        }

        var nice = AddAndReturnNewObstacle(obstacleType);
        return nice;
    }

    public WorldSegmentGenerationStrategy GetAvailableSegment(SegmentType segmentType)
    {
        foreach (var segment in _segmentsPool[segmentType])
        {
            if (segment.IsAvailable)
            {
                segment.EnableSegment();
                _activeSegments.Add(segment);
                return segment;
            }
        }

        var nice = AddAndReturnNewSegment(segmentType);
        _activeSegments.Add(nice);
        return nice;
    }

    private WorldSegmentGenerationStrategy AddAndReturnNewSegment(SegmentType segmentType)
    {
        var instantiatedSegment = Instantiate(_segmentsPrefabs[segmentType]);
        _segmentsPool[segmentType].Add(instantiatedSegment);
        instantiatedSegment.EnableSegment();
        return instantiatedSegment;
    }
    
    private GameObject AddAndReturnNewObstacle(ObstacleType obstacleType)
    {
        var instantiatedObstacle = Instantiate(_obstaclePrefabs[obstacleType]);
        _obstaclesPool[obstacleType].Add(instantiatedObstacle);
        return instantiatedObstacle;
    }

    public void DisableSegments()
    {
        var UPPER = _activeSegments.Count / 3;
        for (int i = 0; i < UPPER; i++)
        {
            _activeSegments[0].DisableSegment();
            _activeSegments.RemoveAt(0);
        }
    }

    public int GetActiveSegmentsCount()
    {
        return _activeSegments.Count;
    }
}

public enum SegmentType
{
    GrassSegment,
    RoadSegment,
    RiverSegment
}

public enum ObstacleType
{
    TreeObstacle,
    RockObstacle,
    CarObstacle,
    MovingPlatform
}