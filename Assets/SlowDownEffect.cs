using UnityEngine;

public class SlowDownEffect : MonoBehaviour
{
    [SerializeField] private float slowDownFactor = 0.5f; // Adjust this value to set the slowdown amount
    [SerializeField] private float effectDuration = 3f; // Duration of the effect in seconds
    [SerializeField] private float damageAmount = 10f; // Adjust this value to set the damage amount

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("slow!");
        MonsterAI monster = other.GetComponent<MonsterAI>();
        if (monster != null)
        {
            monster.ApplySlowDown(slowDownFactor, effectDuration);
            DealDamageToMonster(monster);
        }
    }

    private void DealDamageToMonster(MonsterAI monster)
    {
        Health monsterHealth = monster.GetComponent<Health>();
        if (monsterHealth != null)
        {
            monsterHealth.TakeDamage((int)damageAmount);
        }
    }
}
