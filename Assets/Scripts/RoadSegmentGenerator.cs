using System.Collections;
using UnityEngine;

public class RoadSegmentGenerator : WorldSegmentGenerationStrategy
{
    [SerializeField] private float _minDelayTime;
    [SerializeField] private float _maxDalayTime;
    private float _xSpawnPosition;
    private bool _isCarMovingRight;

    public override void GenerateSegment()
    {
        _isCarMovingRight = (Random.value > 0.5f);
        _xSpawnPosition = _isCarMovingRight
            ? -_worldSegmentData.XSpawnBorderValue
            : _worldSegmentData.XSpawnBorderValue;
        SetCar(new Vector3(Random.Range(-_xSpawnPosition / 2, _xSpawnPosition / 2), 
            transform.position.y, transform.position.z));
        StartCoroutine(SpawnCars());
    }

    private IEnumerator SpawnCars()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(Random.Range(_minDelayTime, _maxDalayTime));
            SetCar(new Vector3(_xSpawnPosition, transform.position.y, transform.position.z));
        }
    }
    private void SetCar(Vector3 spawnPosition)
    {
        var currentPlatform = _pool.GetAvailableObstacle(ObstacleType.CarObstacle);
        currentPlatform.transform.position = spawnPosition;
        currentPlatform.transform.rotation = Quaternion.Euler(0, _isCarMovingRight ? 0 : 180, 0);
    }
}
