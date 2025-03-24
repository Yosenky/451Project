using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrowaveDoor : MonoBehaviour
{

    // define vars
    public float openAngle;
    public float openSpeed;

    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    // Start is called before the first frame update
    void Start()
    {
        // set closed rotation
        closedRotation = transform.localRotation;
        //openRotation = Quaternion.Euler(0, openAngle, 0) * closedRotation; // with this line the door swings inwards
        openRotation = closedRotation * Quaternion.Euler(0, -openAngle, 0);
    }

    // toggle the door
    public void Toggle()
    {
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(RotateDoor());
    }

    // actual rotate door function 
    private System.Collections.IEnumerator RotateDoor()
    {
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * openSpeed);
            yield return null;
        }
        transform.localRotation = targetRotation;
    }
}
