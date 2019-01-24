using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToDestroy : MonoBehaviour {

    public float waitBeforeDestroy = 3f;
    public GameObject objectToDestroy;

    private void Start()
    {
        if (objectToDestroy == null)
        {
            objectToDestroy = this.gameObject;
        }
        Destroy(objectToDestroy, waitBeforeDestroy);
    }
}
