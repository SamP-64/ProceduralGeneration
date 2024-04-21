using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public void DeleteAllEnemies()
    {
        AiMovement[] enemies = FindObjectsOfType<AiMovement>(); // Find all spawned enemies
        
        foreach (AiMovement enemy in enemies)
        {
            if(enemy.name != "Enemy") // Always keep origional enemy
            {
                Destroy(enemy.gameObject);
                Debug.Log("destroyed");
            }
           
        }
    }
}
