using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [Header("Start Block")]
    [SerializeField] private int _startBlockSize;
    [SerializeField] private Transform _startPoint;
    
    [Header("Generation")]
    [SerializeField] private WorldSegmentData _worldSegmentData;
    [SerializeField] private int _maxNumberOfActiveSegments;
    [SerializeField] private int _numberOfActiveSegments;
    [SerializeField] private Transform _triggerTransform;
    [SerializeField] private WorldTrigger _worldTrigger;

    [Header("Grass")] 
    [SerializeField] private int _maxGrassLength;
    [Header("Road")]
    [SerializeField] private int _maxRoadLength;
    [Header("River")]
    [SerializeField] private int _maxRiverLength;

    private SegmentsPool _segmentsPool;
    private float _stepSize;
    private WorldSegmentGenerationStrategy _previousSegment;
    private bool _isTriggerSet = false;

    private void Start()
    {
        _worldTrigger.trigger += StartGeneration;
        _stepSize = _worldSegmentData.StepSize;
        _segmentsPool = SegmentsPool.Instance;
        CreateStartBlock();
        _triggerTransform.localScale = new Vector3(_worldSegmentData.WorldSegmentScale.x * 10f, 2f, 2f);
    }

    private void StartGeneration()
    {
        _triggerTransform.position = _previousSegment.transform.position + Vector3.forward * _stepSize;
        _segmentsPool.DisableSegments();
        PlaceSegments();
    }

    private void PlaceSegments()
    {
        if (_segmentsPool.GetActiveSegmentsCount() >= _maxNumberOfActiveSegments)
        {
            _isTriggerSet = false;
            _numberOfActiveSegments = 0;
            return;
        }

        switch (Random.Range(0, 3))
        {
            case 0:
                SetBlock(SegmentType.GrassSegment, Random.Range(1, _maxGrassLength));
                break;
            case 1:
                SetBlock(SegmentType.RoadSegment, Random.Range(3, _maxRoadLength));
                break;
            case 2:
                SetBlock(SegmentType.RiverSegment, Random.Range(3, _maxRiverLength));
                break;
        }
    }

    private void CreateStartBlock()
    {
        var segment = _segmentsPool.GetAvailableSegment(SegmentType.GrassSegment);
        SetSegmentAtPoint(segment, _startPoint.position);
        _previousSegment = segment;
        _numberOfActiveSegments++;

        SetBlock(SegmentType.GrassSegment, _startBlockSize);
    }

    private void SetBlock(SegmentType segmentType, int numberOfSegments)
    {
        var segment = _previousSegment;
        var previousSegmentPosition = _previousSegment.transform.position;

        for (int i = 0; i < numberOfSegments; i++)
        {
            _numberOfActiveSegments++;
            segment = _segmentsPool.GetAvailableSegment(segmentType);
            SetSegmentAtPoint(segment, previousSegmentPosition + Vector3.forward * _stepSize);
            segment.GenerateSegment();

            previousSegmentPosition = segment.transform.position;
        }
        
        _previousSegment = segment;
        PlaceSegments();
    }

    private void SetSegmentAtPoint(Component segment, Vector3 newPosition)
    {
        segment.transform.position = newPosition;
    }
}