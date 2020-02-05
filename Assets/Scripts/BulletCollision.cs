using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCollision : MonoBehaviour
{
    public Bullet bulletType;
    bool isPlayer;
    bool isEnemy;
    public int bulletDamage;
    public enum Bullet { player, enemy }
    public void SetBulletType(Bullet type)
    {
        switch (type)
        {
            case Bullet.player:
                {
                    isPlayer = true;
                    isEnemy = false;
                    break;
                }
            case Bullet.enemy:
                {
                    isPlayer = false;
                    isEnemy = true;
                    break;
                }
        }
    }
    private void Start()
    {
        SetBulletType(bulletType);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && isEnemy)
        {
            ShipSystem ship = collision.gameObject.GetComponent<ShipSystem>();
            ship.ShipDamage(bulletDamage);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy") && isPlayer)
        {
            EnemyAI enemyShip = collision.gameObject.GetComponent<EnemyAI>();
            enemyShip.DamageEnemy(bulletDamage);
            Destroy(gameObject);
        }
    }
}
