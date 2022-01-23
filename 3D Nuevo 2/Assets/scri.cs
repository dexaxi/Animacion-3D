using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scri : MonoBehaviour
{
    Transform thisTransform;

    [SerializeField] Transform boneTransfor;
    void Awake()
    {
        thisTransform =GetComponent<Transform>();
    }

    void Update()
    {
        thisTransform.position = new Vector3(thisTransform.position.x, boneTransfor.position.y,thisTransform.position.z);
    }
}
