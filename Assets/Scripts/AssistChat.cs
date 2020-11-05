using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssistChat : MonoBehaviour
{

    public GameObject text;
    public TransparentAndInteractable windowObject;
    public bool textShown = false;

    // Start is called before the first frame update
    void Start()
    {
        //text = GameObject.Find("Words and Shit");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown() {
        textShown = !textShown;
        text.SetActive(textShown);
        //windowObject.SetClickthrough(false);
        
    }
}
