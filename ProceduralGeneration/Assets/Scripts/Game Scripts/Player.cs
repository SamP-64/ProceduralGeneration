using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private GridManager gridManager;
    public PlayerShoot playerShoot;

    void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        playerShoot = GameObject.Find("Player").GetComponent<PlayerShoot>();
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy") // player dies if collided with enemy
        {
            gridManager.Generate();
            gridManager.PlayerDies();
        }
    }
}
