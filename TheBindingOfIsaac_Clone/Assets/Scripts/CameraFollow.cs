using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject Target;
    private float yOffset = 7.67f;
    public static CameraFollow instance;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Target != null)
            transform.position = new Vector3(Target.transform.position.x, Target.transform.position.y + yOffset, Target.transform.position.z);
    }

    public void SetTarget(GameObject target)
    {
        Target = target;
    }

    public void DiePlayer()
    {
        Target = null;
    }
}
