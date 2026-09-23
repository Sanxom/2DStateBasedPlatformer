using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float spikeDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockbackAbility knockbackAbility = collision.GetComponentInParent<KnockbackAbility>();
        knockbackAbility.StartKnockback(knockbackDuration, knockbackForce, transform);

        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        playerStats.DamagePlayer(spikeDamage);
    }
}