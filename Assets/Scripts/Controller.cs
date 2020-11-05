using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public RingMenuMB mainMenuPrefab;
    public RingMenuMB mainMenuInstance;
    //[HideInInspector]
    public ControllerMode mode = ControllerMode.MIN;
    public bool spawned = false;

    public GameObject textOverlay;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mode == ControllerMode.MAX && !spawned)
        {
            textOverlay.SetActive(true);
            mainMenuInstance = Instantiate(mainMenuPrefab, FindObjectOfType<Canvas>().transform);
            spawned = true;
            mainMenuInstance.callback = MenuClick;
        } 
        if (mode == ControllerMode.MIN && spawned) {
            textOverlay.SetActive(false);
            spawned = false;
            Destroy(mainMenuInstance.gameObject);
        }
        // if(Input.GetKeyDown(KeyCode.L))
        // {
        //     mode = (mode == ControllerMode.MAX) ? ControllerMode.MIN : ControllerMode.MAX;
        // }

    }

    private void MenuClick(string path)
    {
        var paths = path.Split('/');
        //GetComponent<Place>

    }

    public void SetMode(ControllerMode mode) 
    {
        this.mode = mode;
    }

    public enum ControllerMode
    {
        MIN,
        MAX
    }
}
