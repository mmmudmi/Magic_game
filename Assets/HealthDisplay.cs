using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Health bossHealth;

    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private Slider bossHealthSlider;

    private void Start()
    {
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        playerHealthSlider.maxValue = playerHealth.GetMaxHealth();
        playerHealthSlider.value = playerHealth.GetCurrentHealth();

        bossHealthSlider.maxValue = bossHealth.GetMaxHealth();
        bossHealthSlider.value = bossHealth.GetCurrentHealth();
    }

    private void Update()
    {
        UpdateHealthSliders();
    }

    private void UpdateHealthSliders()
    {
        playerHealthSlider.value = playerHealth.GetCurrentHealth();
        bossHealthSlider.value = bossHealth.GetCurrentHealth();
    }
}
