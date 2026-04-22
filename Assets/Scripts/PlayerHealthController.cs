using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    private void Awake()
    {
        instance = this;
    }

    public float currentHealth, maxHealth;

    public Slider healthSlider;

    public GameObject deathEffect;

    // ... existing code ...

    // Start is called before the first frame update
    void Start()
    {
        // Ensure purchased stats are applied (handles load order)
        if (PlayerStatController.instance != null)
        {
            PlayerStatController.instance.ApplyPurchasedStats();
        }
        else
        {
            // Use fallback if no stat controller
            if (PlayerStatController.instance != null && PlayerStatController.instance.health.Count > 0)
            {
                int hLevel = Mathf.Clamp(PlayerStatController.instance.healthLevel, 0, PlayerStatController.instance.health.Count - 1);
                maxHealth = PlayerStatController.instance.health[hLevel].value;
            }
        }

        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // Heal by a flat amount
    public void Heal(float amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }

    // Heal by a percentage of max health (e.g. 0.25f for 25%)
    public void HealPercent(float percent)
    {
        Heal(maxHealth * percent);
    }
    // Update is called once per frame
    void Update()
    {
        /* if(Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        } */
    }

    public void TakeDamage(float damageToTake)
    {
        currentHealth -= damageToTake;

        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);

            LevelManager.instance.EndLevel();

            Instantiate(deathEffect, transform.position, transform.rotation);

            SFXManager.instance.PlaySFX(3);
        }

        healthSlider.value = currentHealth;
    }
}
