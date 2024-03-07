using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame

    float count;
    void Update()
    {
        count = count + 0.01f;
     
        Diagonal();
       // Vertical();
       // Horizontal();
       // Circle(); 

    }
    void Horizontal()
    {

        gameObject.transform.position = new Vector3(Mathf.Sin(count), 0, 0);
    }
    void Vertical()
    {
        gameObject.transform.position = new Vector3( 0, Mathf.Sin(count), 0);
    }
    void Diagonal()
    {
        gameObject.transform.position = new Vector3(Mathf.Sin(count) , Mathf.Sin(count), 0);
    }
    void Circle()
    {
        
        gameObject.transform.position = new Vector3(Mathf.Sin(count), Mathf.Cos(count), 0);
    }
}
