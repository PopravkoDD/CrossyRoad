using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

public class CharacterMovement : MonoBehaviour
{
    public event Action deathEvent;
    public event Action onStepBack;
    public event Action onStepForward;

    [SerializeField] private WorldSegmentData _worldSegmentData;
    [SerializeField] private AnimationCurve _verticalJumpAnimation;
    [SerializeField] private AnimationCurve _horizontalJumpAnimation;
    private LayerMask _previousSegmentLayer;
    [SerializeField] private LayerMask _groundSegmentLayer;
    [SerializeField] private LayerMask _movingPlatformLayer;
    [SerializeField] private LayerMask _deathLayer;
        
    
    private float _stepDistance;
    private bool _isInJump;
    [SerializeField] private int _horizontalJumpCounter;
    private bool _isOnMovingPlatform;

    private Vector2 _startTouchPosition;
    private Vector2 _endTouchPosition;

    private void Start()
    {
        _horizontalJumpCounter = _worldSegmentData.GetCenterOfSegmentCount();
        _previousSegmentLayer = _groundSegmentLayer;
        _stepDistance = _worldSegmentData.StepSize;
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _startTouchPosition = touch.position;
                    break;
                case TouchPhase.Ended:
                {
                    var swipeVector = (touch.position - _startTouchPosition).normalized;

                    if (swipeVector.x >= 0.71 && !_isInJump && _horizontalJumpCounter != _worldSegmentData.XMoveCoordinates.Count)
                    {
                        _horizontalJumpCounter++;
                        StartCoroutine(Jump(transform.position + Vector3.right * _stepDistance));
                        break;
                    }

                    if (swipeVector.y >= 0.71 && !_isInJump)
                    {
                        StartCoroutine(Jump(transform.position + Vector3.forward * _stepDistance));
                        onStepForward?.Invoke();
                        break;
                    }

                    if (swipeVector.x <= -0.71 && !_isInJump && _horizontalJumpCounter != 1)
                    {
                        _horizontalJumpCounter--;
                        StartCoroutine(Jump(transform.position + Vector3.left * _stepDistance));
                        break;
                    }

                    if (swipeVector.y <= -0.71 && !_isInJump)
                    {
                        StartCoroutine(Jump(transform.position + Vector3.back * _stepDistance));
                        onStepBack?.Invoke();
                    }
                    
                    break;
                }
            }
        }
    }

    private IEnumerator Jump(Vector3 endPosition)
    {
        _isInJump = true;
        var animationTime = 0f;
        var startPosition = transform.position;
        do
        {
            animationTime += Time.deltaTime;
            transform.position =
                new Vector3(Mathf.Lerp(startPosition.x, endPosition.x, _horizontalJumpAnimation.Evaluate(animationTime)),
                    startPosition.y + _verticalJumpAnimation.Evaluate(animationTime),
                    Mathf.Lerp(startPosition.z, endPosition.z, _horizontalJumpAnimation.Evaluate(animationTime)));
            yield return null;
        } while (animationTime <= _verticalJumpAnimation.keys[^1].time);

        transform.position = new Vector3(endPosition.x, startPosition.y, endPosition.z);

        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        Physics.Raycast(ray, out hit);
        var layer = hit.transform.gameObject.layer;

        if (_groundSegmentLayer.ExistLayerByLayerMask(layer))
        {
            _isInJump = false;

            if (_previousSegmentLayer.ExistLayerByLayerMask(_movingPlatformLayer))
            {
                transform.parent.DetachChildren();
                _previousSegmentLayer = layer;

                var nearestCoordinateCount = _worldSegmentData.GetNearestCoordinate(endPosition.x);
                Debug.Log(nearestCoordinateCount);
                _horizontalJumpCounter = nearestCoordinateCount + 1;
                transform.position = new Vector3(_worldSegmentData.XMoveCoordinates[nearestCoordinateCount], startPosition.y, endPosition.z);
                yield return null;
            }
             
            yield return null;
        }
        
        if (_deathLayer.ExistLayerByLayerMask(layer))
        {
            _isInJump = false;
            deathEvent?.Invoke();
            Destroy(gameObject);
            yield return null;
        }
        
        if (_movingPlatformLayer.ExistLayerByLayerMask(layer))
        {
            _isInJump = false;
            _isOnMovingPlatform = true;
            transform.parent = hit.transform;
            _previousSegmentLayer = layer;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_deathLayer.ExistLayerByLayerMask(other.gameObject.layer))
        {
            deathEvent?.Invoke();
            Destroy(gameObject);
        }
    }
}

public static class LayerMaskExtension
{
    public static bool ExistLayerByLayerMask(this LayerMask layerMask, int layer)
    {
        if ((layerMask.value & (1 << layer)) != 0)
        {
            return true;
        }

        return false;
    }
}
