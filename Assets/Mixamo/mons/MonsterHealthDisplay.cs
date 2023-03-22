using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthDisplay : MonoBehaviour
{
    public MonsterAI monster;
    private Slider healthBar;
    private float initialHealth;

    private void Start()
    {
        healthBar = GetComponent<Slider>();
        initialHealth = monster.health;
    }

    private void Update()
    {
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = monster.health / initialHealth;
        healthBar.value = healthPercentage;
    }
}