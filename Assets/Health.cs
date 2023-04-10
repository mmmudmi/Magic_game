using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private string objectTag;
    [SerializeField] private GameObject warp;
    [SerializeField] private float runAwayHealthLimit;

    [SerializeField] TextMeshProUGUI livesText;

    private int currentHealth;

    private GameSession gameSession;

    private void Start()
    {
        gameSession = GameSession.Instance;

        if (gameObject.CompareTag("Player"))
        {
            if (gameSession.playerHealth > 0)
            {
                currentHealth = gameSession.playerHealth;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }
        else if (gameObject.CompareTag("Boss"))
        {
            if (gameSession.bossHealth > 0)
            {
                currentHealth = gameSession.bossHealth;
            }
            else
            {
                currentHealth = maxHealth;
            }
        }

        UpdateHealthSlider();
    }

    private void OnDestroy()
    {
        if (gameObject.CompareTag("Player"))
        {
            gameSession.playerHealth = currentHealth;
        }
        else if (gameObject.CompareTag("Boss"))
        {
            gameSession.bossHealth = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthSlider();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
            livesText.text = ((int) currentHealth).ToString();

        }
    }

    private void Die()
    {
        if (objectTag == "Player")
        {
            // Handle player death
            SceneManager.LoadScene("Lost");
        }
        else if (objectTag == "Boss")
        {
            // Handle boss death
            SceneManager.LoadScene("Win");
        }
    }

    public bool RunAwayHealthLimitReached()
    {
        return currentHealth <= runAwayHealthLimit;
    }
    
    public void Heal(int healingAmount)
    {
        currentHealth += healingAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthSlider();
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
