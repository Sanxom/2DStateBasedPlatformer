using UnityEngine;

public class SwingingBlade : MonoBehaviour
{
    [Header("Swing Settings")]
    [SerializeField] private float maxAngle;
    [SerializeField] private float speed;
    private float timer;

    [Header("Knockback Settings")]
    [SerializeField] private Vector2 knockbackForce;
    [SerializeField] private float bladeDamage;
    [SerializeField] private float knockbackDuration;
    private float previousAngle = 0f;
    private int pushDirection = 1;


    private void Update()
    {
        timer += speed * Time.deltaTime;

        float angle = maxAngle * Mathf.Sin(timer);
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        if (angle > previousAngle)
            pushDirection = 1;
        else if (angle < previousAngle)
            pushDirection = -1;

        previousAngle = angle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockbackAbility knockbackAbility = collision.GetComponentInParent<KnockbackAbility>();
        knockbackAbility.StartSwingKnockback(knockbackDuration, knockbackForce, pushDirection);

        PlayerStats playerStats = collision.GetComponent<PlayerStats>();
        playerStats.DamagePlayer(bladeDamage);
    }
}