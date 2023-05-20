using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RiverSegmentGenerator : WorldSegmentGenerationStrategy
{
    private static bool _rotationSwitcher;
    private bool _isPlatformMovingRight;
    [SerializeField] private float _minDelayTime;
    [SerializeField] private float _maxDalayTime;
    private float _xSpawnPosition;

    public override void GenerateSegment()
    {
        _rotationSwitcher = !_rotationSwitcher;
        _isPlatformMovingRight = _rotationSwitcher;
        _xSpawnPosition = _isPlatformMovingRight
            ? -_worldSegmentData.XSpawnBorderValue
            : _worldSegmentData.XSpawnBorderValue;
        SetPlatform(new Vector3(Random.Range(-_xSpawnPosition / 2, _xSpawnPosition / 2), 
            transform.position.y, transform.position.z));
        StartCoroutine(SpawnPlatforms());
    }

    private IEnumerator SpawnPlatforms()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(_minDelayTime, _maxDalayTime));
            SetPlatform(new Vector3(_xSpawnPosition, transform.position.y, transform.position.z));
        }
    }

    private void SetPlatform(Vector3 spawnPosition)
    {
        var currentPlatform = _pool.GetAvailableObstacle(ObstacleType.MovingPlatform);
        currentPlatform.transform.position = spawnPosition;
        currentPlatform.transform.rotation = Quaternion.Euler(0, _isPlatformMovingRight ? 0 : 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(_xSpawnPosition, transform.position.y, transform.position.z), 1);
    }
}
