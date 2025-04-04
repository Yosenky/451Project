using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f); // spin collectible

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
