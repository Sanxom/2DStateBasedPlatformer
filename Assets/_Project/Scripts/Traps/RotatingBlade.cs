using UnityEngine;

public class RotatingBlade : MonoBehaviour
{
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float bladeDamage;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float knockbackDuration;

    private void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockbackAbility knockbackAbility = collision.GetComponentInParent<KnockbackAbility>();
        knockbackAbility.StartKnockback(knockbackDuration, knockbackForce, transform);

        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        playerStats.DamagePlayer(bladeDamage);
    }
}