using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalScript : MonoBehaviour
{
    public List<GameObject> cubes;
    public int cubeNo;
    

    // Start is called before the first frame update
    void Start()
    {
        cubeNo = Random.Range(0, cubes.Count);
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit();
    }
}
