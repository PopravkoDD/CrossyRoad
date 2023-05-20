using UnityEngine;

public class MovingObstaclesController : MonoBehaviour
{
    [SerializeField] private WorldSegmentData _worldSegmentData;
    [SerializeField] private float _speed;
    private float _xDisableRightBorder;
    private float _xDisableLeftBorder;


    private void Start()
    {
        _xDisableRightBorder = _worldSegmentData.XSpawnBorderValue;
        _xDisableLeftBorder = -_worldSegmentData.XSpawnBorderValue;
    }

    void Update()
    {
        if (transform.position.x > _xDisableRightBorder || transform.position.x < _xDisableLeftBorder)
        {
            gameObject.SetActive(false);
        }

        transform.Translate(Vector3.right * (_speed * Time.deltaTime));
    }
}
