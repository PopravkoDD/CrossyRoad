using System;
using UnityEngine;

public abstract class WorldSegmentGenerationStrategy : MonoBehaviour
{
    [SerializeField] protected bool _isAvailable;
    protected SegmentsPool _pool;

    public bool IsAvailable => _isAvailable;

    [SerializeField] protected WorldSegmentData _worldSegmentData;

    private void Awake()
    {
        _pool = SegmentsPool.Instance;
    }

    void Start()
    {
        transform.localScale = _worldSegmentData.WorldSegmentScale;
    }

    public abstract void GenerateSegment();

    public virtual void DisableSegment()
    {
        _isAvailable = true;
        gameObject.SetActive(false);
    }

    public void EnableSegment()
    {
        gameObject.SetActive(true);
        _isAvailable = false;
    }
}
