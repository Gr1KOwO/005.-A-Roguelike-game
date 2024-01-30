using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected LayerMask floorLayer;
    protected abstract void CustomOnTriggerEnter(Collider other);

    private void Update()
    {
        if (transform.position.y <= -1f)
            FindFloor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hole"))
        {
            FindFloor();
        }
        CustomOnTriggerEnter(other);
    }

    private void FindFloor()
    {
        RaycastHit hit;
        Vector3[] directions = { Quaternion.Euler(-45f, 0f, 0f) * Vector3.up,Quaternion.Euler(0f, 0f, -45f) * Vector3.up,
                                     Quaternion.Euler(0f, 0f, 45f) * Vector3.up, Quaternion.Euler(45f, 0f, 0f) * Vector3.up};

        foreach (Vector3 direction in directions)
        {
            if (Physics.Raycast(transform.position, direction, out hit, Mathf.Infinity, floorLayer))
            {
                transform.position = hit.point;
                break;
            }
        }
    }
}
