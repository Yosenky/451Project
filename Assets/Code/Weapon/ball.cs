using UnityEngine;

public class Ball : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        // Destroy after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Health target = collision.gameObject.GetComponent<Health>();
        //if (target != null)
        //{
        //    target.TakeDamage(damage);
        //}
        Debug.Log("hit");
        Destroy(gameObject);
    }
}
