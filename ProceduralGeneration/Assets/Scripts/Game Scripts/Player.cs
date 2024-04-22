using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private GridManager gridManager; 

    void Start()
    {
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>(); 
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            gridManager.Generate();
            gridManager.PlayerDies();
        }
    }
}
