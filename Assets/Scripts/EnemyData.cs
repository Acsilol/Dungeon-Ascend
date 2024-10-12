using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemy/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public float maxHealth;
    public float damage;
    public float moveSpeed;
}