using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumber3D : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float fadeSpeed = 2f;
    public float lifetime = 1.5f;

    private TextMeshProUGUI text;
    private Color startColor;
    private Transform cam;

    void Awake() 
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        startColor = text.color;
        cam = Camera.main.transform;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        
        transform.LookAt(transform.position + cam.forward);

        
        Color currentColor = text.color;
        currentColor.a -= fadeSpeed * Time.deltaTime;
        text.color = currentColor;
    }

    public void SetDamage(float dmg)
    {
        if (text != null)
        {
            text.text = dmg.ToString("F0");
        }
        else
        {
            Debug.LogWarning("Text reference is null in DamageNumber3D!");
        }
    }
}