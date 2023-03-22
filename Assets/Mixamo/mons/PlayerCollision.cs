using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public int collisionDamage = 10; // Damage dealt to the monster when colliding with the player

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        MonsterAI monster = hit.gameObject.GetComponent<MonsterAI>();

        if (monster != null)
        {
            monster.TakeDamage(collisionDamage);
        }
    }
}