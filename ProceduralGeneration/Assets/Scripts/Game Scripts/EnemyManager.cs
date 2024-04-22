using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void DeleteAllEnemies()
    {
        AiMovement[] enemies = FindObjectsOfType<AiMovement>(); // Find all spawned enemies
        Bullet[] bullets = FindObjectsOfType<Bullet>(); // Find all spawned bullets

        foreach (AiMovement enemy in enemies)
        {
            if(enemy.name != "Enemy") // Always keep origional enemy
            {
                Destroy(enemy.gameObject);
            }
           
        }
        foreach (Bullet bullet in bullets)
        {
            Destroy(bullet.gameObject);
        }
    }
}
