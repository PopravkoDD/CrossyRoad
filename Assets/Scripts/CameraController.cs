using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private WorldSegmentData _worldSegmentData;
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxZDistance;
    private float _nextXPosition;
    private float _nextZPosition;
    private float _xMinBorder;
    private float _xMaxBorder;


    private void Start()
    {
        _xMaxBorder = _worldSegmentData.WorldSegmentScale.x * 5 - Camera.main.aspect * Camera.main.orthographicSize;
        _xMinBorder = -_xMaxBorder;
        _target.gameObject.GetComponent<CharacterMovement>().deathEvent += DisableThis;
    }

    private void Update()
    {
        if (_target.position.z - transform.position.z > _maxZDistance)
        {
            transform.position = new Vector3( _nextXPosition, transform.position.y, _target.position.z - _maxZDistance);
        }
        
        var nextX = Mathf.Lerp(transform.position.x, _target.position.x, _speed * Time.deltaTime);
        _nextZPosition  = transform.position.z + _speed * Time.deltaTime;
        
        if (nextX < _xMaxBorder && nextX > _xMinBorder)
        {
            _nextXPosition = nextX;
        }
        
        transform.position = new Vector3( _nextXPosition, transform.position.y, _nextZPosition);
    }

    private void DisableThis()
    {
        this.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(new Vector3(_xMaxBorder, transform.position.y, transform.position.z), Vector3.forward);
        Gizmos.DrawRay(new Vector3(_xMinBorder, transform.position.y, transform.position.z), Vector3.forward);
    }
}
