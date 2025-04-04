using UnityEngine;

public class Ball : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 20f;

    private Rigidbody rb;

    public AudioClip shootSound;
    public AudioClip hitSound;
    private AudioSource audioSource;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
        // Destroy after 5 seconds to prevent clutter
        Destroy(gameObject, 5f);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Play the throwing sound
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.CompareTag("Enemy"))
        {

            // Get the EnemyAI script on the enemy
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            RangedLangsat rangedLangsat = collision.gameObject.GetComponent<RangedLangsat>();
            Rollingenemy rollingenemy = collision.gameObject.GetComponent<Rollingenemy>();

            if (hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            if (enemyAI != null)
            {
                // Call the Damaged method with the boomerang’s damage
                // Note: enemyAI.Damaged() expects an int,
                //       so we cast if boomerang damage is a float
                enemyAI.Damaged(Mathf.RoundToInt(damage));
            }
            if (rangedLangsat != null)
            {
                rangedLangsat.Damaged(Mathf.RoundToInt(damage));
            }
            if (rollingenemy != null)
            {
                rollingenemy.Damaged(Mathf.RoundToInt(damage));
            }


        }
        Destroy(gameObject);

    }
}
