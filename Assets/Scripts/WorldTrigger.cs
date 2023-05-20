using System;
using UnityEngine;

public class WorldTrigger : MonoBehaviour
{
    public event Action trigger;
    [SerializeField] private LayerMask _layerToTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (_layerToTrigger.ExistLayerByLayerMask(other.gameObject.layer))
        {
            Debug.Log("Trigger");
            trigger.Invoke();
        }
    }
}
