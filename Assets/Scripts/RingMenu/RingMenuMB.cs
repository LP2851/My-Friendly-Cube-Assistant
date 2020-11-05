using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RingMenuMB : MonoBehaviour
{
    public Ring data;
    public RingCakePiece ringCakePiecePrefab;
    public float gapWidthDegree = 1f;
    public Action<string> callback;
    public RingCakePiece[] pieces;
    public RingMenuMB parent;
    [HideInInspector]
    public string path;

    public Controller controller;
    public float mouseAngle;
    public Vector3 up = new Vector3(0, 100000, 0);
    public Vector3 mouse;   
    public int activeElement;

    public ActivateAssistant assistant;

    public bool movedBoxAwayFromCenter;

    //private string[] itemNames = {"Search", "Apps", "Settings", "Close"};

    public GameObject textOverlay;


    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("Controller").GetComponent<Controller>();;
        assistant = GameObject.FindGameObjectWithTag("Assistant").GetComponent<ActivateAssistant>();
        movedBoxAwayFromCenter = false;
        var stepLength = 360f/ data.elements.Length;
        var iconDist = Vector3.Distance(ringCakePiecePrefab.icon.transform.position, ringCakePiecePrefab.CakePiece.transform.position) /3;
        
        textOverlay = GameObject.FindGameObjectWithTag("TextOverlayMainMenu");
        

        pieces = new RingCakePiece[data.elements.Length];

        for (int i = 0; i < data.elements.Length; i++) {
            pieces[i] = Instantiate(ringCakePiecePrefab, transform);
            //set root element
            pieces[i].transform.localPosition = Vector3.zero;
            pieces[i].transform.localRotation = Quaternion.identity;

            //set cake piece
            pieces[i].CakePiece.fillAmount = 1f / data.elements.Length - gapWidthDegree / 360f;
            pieces[i].CakePiece.transform.localPosition = Vector3.zero;
            pieces[i].CakePiece.transform.localRotation = Quaternion.Euler(0, 0, -stepLength / 2f + gapWidthDegree / 2f + i * stepLength);
            pieces[i].CakePiece.color = new Color(51, 51, 51, 0.75f);

            //set icon 
            //pieces[i].icon.transform.localPosition = pieces[i].CakePiece.transform.localPosition + Quaternion.AngleAxis(i * stepLength, Vector3.forward) * Vector3.up * iconDist;
            pieces[i].icon.transform.localPosition = pieces[i].CakePiece.transform.localPosition + Quaternion.Euler(0, 0, -stepLength / 2f +  gapWidthDegree/ 2f + i * stepLength + -stepLength /2f) * Vector3.up * iconDist;
            pieces[i].icon.sprite = data.elements[i].icon;
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        var stepLength = 360f / data.elements.Length;
        mouse = Input.mousePosition - new Vector3(Screen.width/2, Screen.height/2);

        mouseAngle = NormalizeAngle(Vector3.SignedAngle(Vector3.up, mouse, Vector3.forward)+ stepLength / 2f);



        activeElement = (int)(mouseAngle / stepLength);
        activeElement = activeElement +1;
        if (activeElement == data.elements.Length)
        {
            activeElement = 0;
        }

        for (int i = 0; i < data.elements.Length; i++)
        {
            if (i == activeElement)
            {
                pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.9f);
            } else
            {
                pieces[i].CakePiece.color = new Color(51, 51, 51, 0.75f);
            }
        }
        //Debug.Log(activeElement);
        //textOverlay.GetComponent<UnityEngine.UI.Text>().text = itemNames[activeElement];
        textOverlay.GetComponent<UnityEngine.UI.Text>().text = data.elements[activeElement].name;
        
        if (Input.GetMouseButtonDown(0))
        {
            var Path = path + "/" + data.elements[activeElement].name;

            if(data.elements[activeElement].nextRing != null)
            {
                var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RingMenuMB>();
                newSubRing.parent = this;

                for (var j = 0; j < newSubRing.transform.childCount; j++)
                {
                    Destroy(newSubRing.transform.GetChild(j).gameObject);
                }
                newSubRing.data = data.elements[activeElement].nextRing;
                newSubRing.path = Path;
                newSubRing.callback = callback;
                gameObject.SetActive(false);
            } else
            {
                //callback?.Invoke(path);
            }

            string activeElementName = data.elements[activeElement].name;

            if (activeElementName.Equals("Settings")) { //settings
                // StopCoroutine("MoveAssistantTo");
                // StartCoroutine(ActivateAssistant.MoveAssistantTo(assistant.gameObject.transform.position, new Vector3(assistant.startPos.x, assistant.midPos.y, 0)));
                assistant.SetAnimateMoveScale(assistant.GetCurrentPosition(), new Vector3(assistant.startPos.x, assistant.midPos.y, assistant.GetCurrentPosition().z), assistant.GetCurrentLocalScale(), assistant.GetCurrentLocalScale());
                assistant.SetAnimating(true);
                assistant.nonRingMenuOpen = true;
                assistant.pageController.TurnPageOn(UnityCore.Menu.PageType.Settings);
                controller.mode = Controller.ControllerMode.MIN;
                //Destroy(gameObject);
            } else if (activeElementName.Equals("Close")) {//close
                assistant.CloseApp();
            } else if (activeElementName.Equals("Search")) {//search
                assistant.SetAnimateMoveScale(assistant.GetCurrentPosition(), new Vector3(assistant.startPos.x, assistant.midPos.y, assistant.GetCurrentPosition().z), assistant.GetCurrentLocalScale(), assistant.GetCurrentLocalScale());
                assistant.SetAnimating(true);
                assistant.nonRingMenuOpen = true;
                assistant.pageController.TurnPageOn(UnityCore.Menu.PageType.Search);
                controller.mode = Controller.ControllerMode.MIN;
            } else if (activeElementName.Equals("Apps")) { // Applications
                // Nothing here, will just open new ring
            } else if(activeElementName.Equals("Back")) {
                GoToParent();
            } 
            
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(parent == null)
            {
                //controller.mode = Controller.ControllerMode.MIN;
                
                assistant.AnimateMovingClose();
                
            } else
            {
                GoToParent();
            }
        }

        void GoToParent() {
            parent.gameObject.SetActive(true);
            Destroy(gameObject);
        }


    }
    private float NormalizeAngle(float a) => (a + 360f) % 360f;
}
