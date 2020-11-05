using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public int maxCooldown = 25;
    public int cooldown = 0;
    public GameObject cube;


    // Update is called once per frame
    void Update()
    {
        if (cube == null)
        {
            GlobalScript gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
            cube = gs.cubes[gs.cubeNo];
        }
        if (cooldown <= 0)
        {
            SpawnCube();
            cooldown = Random.Range(0, maxCooldown);
        }
        cooldown--;
    }

    private void SpawnCube()
    {
        //float mass = Random.Range(1, 3);
        //int size = Random.Range(1, 3);
        Instantiate(cube, transform);

    }
}
