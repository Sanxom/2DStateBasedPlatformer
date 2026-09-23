using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private HealthBarController healthBarController;
    [SerializeField] private float maxHealth;

    [Header("Flash")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color flashColor;
    [SerializeField, Range(0, 1)] private float flashStrength;
    [SerializeField] private float flashDuration;
    private Material defaultMaterial;
    private WaitForSeconds flashWaitForSeconds;

    [Header("Colliders")]
    [SerializeField] private Collider2D standingStatsCollider;
    [SerializeField] private Collider2D crouchStatsCollider;
    private Collider2D currentStatsCollider;

    public float CurrentHealth { get; private set; }
    public bool CanTakeDamage { get; private set; } = true;

    private void Awake()
    {
        defaultMaterial = sr.material;
        CurrentHealth = maxHealth;
        flashWaitForSeconds = new WaitForSeconds(flashDuration);
    }

    private void Start()
    {
        healthBarController.SetSlider(CurrentHealth, maxHealth);
        currentStatsCollider = standingStatsCollider;
    }

    public void DamagePlayer(float damage)
    {
        if (!CanTakeDamage) return;

        CurrentHealth -= damage;
        healthBarController.SetSlider(CurrentHealth, maxHealth);
        StartCoroutine(FlashCoroutine());

        if (CurrentHealth <= 0f)
        {
            if (player.stateMachine.currentState != PlayerStates.State.Knockback)
            {
                player.stateMachine.ChangeState(PlayerStates.State.Dead);
            }
        }
    }
    
    public void EnableDamage()
    {
        CanTakeDamage = true;
    }

    public void DisableDamage()
    {
        CanTakeDamage = false;
    }

    public void EnableStatsCollider()
    {
        currentStatsCollider.enabled = true;
    }

    public void DisableStatsCollider()
    {
        currentStatsCollider.enabled = false;
    }

    public void EnableStatsStandCollider()
    {
        if (CurrentHealth <= 0f) return;

        crouchStatsCollider.enabled = false;
        standingStatsCollider.enabled = true;
        currentStatsCollider = standingStatsCollider;
    }

    public void EnableStatsCrouchCollider()
    {
        if (CurrentHealth <= 0f) return;

        standingStatsCollider.enabled = false;
        crouchStatsCollider.enabled = true;
        currentStatsCollider = crouchStatsCollider;
    }

    private IEnumerator FlashCoroutine()
    {
        CanTakeDamage = false;
        sr.material = flashMaterial;
        flashMaterial.SetColor("_FlashColor", flashColor);
        flashMaterial.SetFloat("_FlashAmount", flashStrength);
        yield return flashWaitForSeconds;
        sr.material = defaultMaterial;

        if (CurrentHealth > 0f)
            CanTakeDamage = true;
    }
}