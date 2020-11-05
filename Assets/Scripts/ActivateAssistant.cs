using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCore.Menu;

public class ActivateAssistant : MonoBehaviour
{
    public Controller controller;
    public PageController pageController;
    public Canvas canvas;
    
    public Vector3 startPos;
    public Vector3 midPos;

    public bool opening;
    public bool closing;
    public bool animating;
    public static bool menuOpen;
    public bool nonRingMenuOpen;
    public bool nonRingMenuHasBeenClosed;
    public bool exitingApp;

    public static float speed;

    public Vector3 startingSize;
    public Vector3 maxSize;

    protected Vector3 movingFrom;
    protected Vector3 movingTo;
    protected Vector3 scaleFrom;
    protected Vector3 scaleTo;

    public static GameObject assistant;
    


    // Start is called before the first frame update
    void Start()
    {
        menuOpen = false;
        animating = false;
        startPos = transform.position;
        midPos = canvas.transform.position;
        opening = false;
        closing = false;
        startingSize = transform.localScale;
        assistant = gameObject;
        speed = 0.5f;
        nonRingMenuOpen = false;
        nonRingMenuHasBeenClosed = false;
        exitingApp = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(opening)
        {
            AnimateMovingOpen();
        } else if (closing)
        {
            AnimateMovingClose();
        } else if (animating) {
            AnimateMove();
        } else if (!animating && nonRingMenuHasBeenClosed) {
            nonRingMenuHasBeenClosed = false;
            controller.mode = Controller.ControllerMode.MAX;
            
        }

        if (nonRingMenuOpen) {
            controller.mode = Controller.ControllerMode.MIN;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            
            // go back to ring menu if not open, if open then go close
            if(!animating && menuOpen && !nonRingMenuOpen && CheckForRings()) {
                // If:
                //      Not animating
                //      Menu is open
                //      Non Ring Menu object not open
                //      Current Ring open has no parent
                AnimateMovingClose();
            } else if(!animating && menuOpen && nonRingMenuOpen) {
                //  If:
                //      Not animating
                //      Menu is open
                //      Non Ring Menu Object is open
                HandleClosingNonRingMenuAndReseting();
            }
            
        }

        if(exitingApp && !animating) {
            Debug.Log("Application Closed");
            Application.Quit();
        }
    }

    private bool CheckForRings() {
        return GameObject.FindGameObjectWithTag("RingMenuRing").GetComponent<RingMenuMB>().parent == null;
    }

    public void CloseApp() {
        SetAnimateMoveScale(transform.position, startPos, transform.localScale, startingSize);
        SetAnimating(true);
        exitingApp = true;
        controller.mode = Controller.ControllerMode.MIN;
        pageController.TurnPageOff(pageController.onPage);
        AnimateMove();
    }

    

    void OnMouseDown()
    {
        if(!animating && !menuOpen)
        {
            AnimateMovingOpen();
        } else if (!animating && menuOpen && !nonRingMenuOpen && CheckForRings())
        {
            AnimateMovingClose();
        } else if (!animating && menuOpen && nonRingMenuOpen) {
            HandleClosingNonRingMenuAndReseting();
        } 

    }
    
    private void HandleClosingNonRingMenuAndReseting() {
        //CLOSE NON RING MENU
        pageController.TurnPageOff(pageController.onPage);
        nonRingMenuOpen = false;
        nonRingMenuHasBeenClosed = true;
        SetAnimateMoveScale(transform.position, midPos, transform.localScale, maxSize);
        SetAnimating(true);
        AnimateMove();
    }

    public void SetAnimating(bool a) {
        animating = a;
    }

    public bool GetAnimating() {
        return animating;
    }

    public Vector3 GetCurrentPosition() {
        return transform.position;
    }

    public Vector3 GetCurrentLocalScale() {
        return transform.localScale;
    }

    public void SetAnimateMoveScale(Vector3 startFrom, Vector3 end, Vector3 startScale, Vector3 endScale) {
        if(!animating) {
            movingFrom = startFrom;
            //Debug.Log("Set movingFrom = "+ movingFrom);
            movingTo = end;
            //Debug.Log("Set movingTo = "+ movingTo);
            scaleFrom = startScale;
            //Debug.Log("Set scaleFrom = "+ scaleFrom);
            scaleTo = endScale;
            //Debug.Log("Set scaleTo = "+ scaleTo);
        }
    }

    void AnimateMove() {
        //Debug.Log("Called AnimateMove()");
        if (scaleFrom == scaleTo) {
            animating = !MoveAssistantTo(movingFrom, movingTo);
        } else {
            animating = !MoveAssistantToAndScaleTo(movingFrom, movingTo, scaleFrom, scaleTo);
        }
    }

    void AnimateMovingOpen()
    {
        opening = true;
        animating = true;
        if (transform.position != midPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, midPos, speed * Time.deltaTime);
            float percentDone = PercentageDistanceComplete(startPos, midPos, transform.position);
            transform.localScale = Vector3.Lerp(startingSize, maxSize, percentDone);
        }
        else
        {
            opening = false; menuOpen = true; animating = false;
            transform.localScale = maxSize;
            controller.mode = Controller.ControllerMode.MAX;
        }
        
    }

    public void AnimateMovingClose()
    {
        
        closing = true;
        controller.mode = Controller.ControllerMode.MIN;
        menuOpen = false;
        animating = true;
        if (transform.position != startPos) { 
            transform.position = Vector3.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
            float percentDone = PercentageDistanceComplete(midPos, startPos, transform.position);
            transform.localScale = Vector3.Lerp(maxSize, startingSize, percentDone);
            controller.mode = Controller.ControllerMode.MIN;
        }
        else
        {
            closing = false; animating = false;
            transform.localScale = startingSize;
        }
    }

    private float PercentageDistanceComplete(Vector3 startingPos, Vector3 endingPosing, Vector3 currentPosition) 
    {
        float startToEndDist = Vector3.Distance(startingPos, endingPosing);
        float currentToEndDist = Vector3.Distance(currentPosition, endingPosing);
        return (1.0f-currentToEndDist) / startToEndDist;
    }


    public bool AssistantIsActive()
    {
        return (animating || menuOpen);
    }

    public bool IsAnimating() {
        return animating;
    }

    public bool IsMenuOpen() {
        return menuOpen;
    }

    private bool MoveAssistantTo(Vector3 start, Vector3 end) {
        //Debug.Log("Called MoveAssistantTo()");
        return MoveAssistantTo(start, end, speed);
    }

    private bool MoveAssistantTo(Vector3 start, Vector3 end, float speed) {
        transform.position = Vector3.MoveTowards(transform.position, end, speed * Time.deltaTime);

        return (transform.position == end);
    }

    private bool MoveAssistantToAndScaleTo(Vector3 start, Vector3 end, Vector3 startScale, Vector3 endScale) {
        return MoveAssistantToAndScaleTo(start, end, speed, startScale, endScale);
    }

    private bool MoveAssistantToAndScaleTo(Vector3 start, Vector3 end, float speed, Vector3 startScale, Vector3 endScale) {
        bool move = MoveAssistantTo(start, end, speed);
        bool scale = ScaleAssistantTo(start, end, startScale, endScale);

        return (move && scale);
    }

    private bool ScaleAssistantTo(Vector3 start, Vector3 end, Vector3 startScale, Vector3 endScale) {
        float percent = PercentageDistanceComplete(start, end, transform.position);
        transform.localScale = Vector3.Lerp(startScale, endScale, percent);

        return (transform.localScale == endScale);
    }
    

    /*
    /// TESTING WITH ENUMERATION
    public static IEnumerator MoveAssistantTo(Vector3 start, Vector3 end) {
        return MoveAssistantTo(start, end, speed);
    }

    public static IEnumerator MoveAssistantTo(Vector3 start, Vector3 end, float speed) {
        if(!animating) {
            animating = true;
            while(assistant.transform.position != end) {
                assistant.transform.position = Vector3.MoveTowards(start, end, speed * Time.deltaTime);
                yield return null;
            }
            animating = false;
        }
    }

    public static IEnumerator MoveAssistantToAndScaleTo(Vector3 start, Vector3 end, Vector3 startScale, Vector3 endScale) {
        return MoveAssistantToAndScaleTo(start, end, speed, startScale, endScale);
    }

    public static IEnumerator MoveAssistantToAndScaleTo(Vector3 start, Vector3 end, float speed, Vector3 startScale, Vector3 endScale) {
        MoveAssistantTo(start, end, speed);
        return ScaleAssistantTo(start, end, startScale, endScale);
    }

    public static IEnumerator ScaleAssistantTo(Vector3 start, Vector3 end, Vector3 startScale, Vector3 endScale) {
        if(!animating) {
            animating = true;
            float percent = PercentageDistanceComplete(start, end, assistant.transform.position);
            while (percent < 1) {
                assistant.transform.localScale = Vector3.Lerp(startScale, endScale, percent);
                yield return null;
            }
            animating = false;
        }
    }
    */


}
