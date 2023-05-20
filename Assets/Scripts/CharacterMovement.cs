using System;
using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
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
    private int _maxHorizontalJumpCounter;
    private bool _isOnMovingPlatform;

    private void Start()
    {
        _previousSegmentLayer = _groundSegmentLayer;
        _stepDistance = _worldSegmentData.StepSize;
        _maxHorizontalJumpCounter = Convert.ToInt32(_worldSegmentData.MaxStepsCount);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && !_isInJump)
        {
            StartCoroutine(Jump(transform.position + Vector3.forward * _stepDistance));
        }
        
        if (Input.GetKeyDown(KeyCode.S) && !_isInJump)
        {
            StartCoroutine(Jump(transform.position + Vector3.back * _stepDistance));
        }
        
        if (Input.GetKeyDown(KeyCode.D) && !_isInJump && _horizontalJumpCounter < _maxHorizontalJumpCounter)
        {
            _horizontalJumpCounter++;
            StartCoroutine(Jump(transform.position + Vector3.right * _stepDistance));
        }
        
        if (Input.GetKeyDown(KeyCode.A) && !_isInJump && _horizontalJumpCounter > -_maxHorizontalJumpCounter)
        {
            _horizontalJumpCounter--;
            StartCoroutine(Jump(transform.position + Vector3.left * _stepDistance));
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

                transform.position = new Vector3(_worldSegmentData.GetNearestCoordinate(endPosition.x), startPosition.y, endPosition.z);
                yield return null;
            }
             
            yield return null;
        }
        
        if (_deathLayer.ExistLayerByLayerMask(layer))
        {
            Debug.Log("Death");
            _isInJump = false;
            //Destroy(gameObject);
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
        /*Debug.Log(gameObject.name);
        Debug.Log(other.gameObject.name);
        Debug.Log(_deathLayer.value);
        Debug.Log(other.gameObject.layer);*/
        Debug.LogWarning("character");
        if (_deathLayer.ExistLayerByLayerMask(other.gameObject.layer));
        {
            Debug.Log("Nice");
        }
    }
}

public static class LayerMaskExtension
{
    public static bool ExistLayerByLayerMask(this LayerMask layerMask, int layer)
    {
        /*Debug.LogError(layerMask.value);
        Debug.LogError(layer);*/
        Debug.LogWarning((layerMask.value & (1 << layer)));
        
        if ((layerMask.value & (1 << layer)) != 0)
        {
            return true;
        }

        return false;
    }
}
