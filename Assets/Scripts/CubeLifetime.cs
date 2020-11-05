using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CubeLifetime : MonoBehaviour
{

    public float life;
    void Start()
    {
        life = Random.Range(10, 20);
    }

    
    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0)
            Destroy(gameObject);

    }
}
