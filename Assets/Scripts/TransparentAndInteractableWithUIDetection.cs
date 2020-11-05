using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public class TransparentAndInteractableWithUIDetection : MonoBehaviour
{
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int uIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);


    private struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cxTopHeight;
        public int cxBottomHeight;

    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    const int GWL_EXSTYLE = -20;

    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;

    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    const uint LWA_COLORKEY = 0x00000001;

    private IntPtr hWnd;

    public Camera mainCamera;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public GameObject canvas;

    public bool clickThrough;

    private void Start() {
        hWnd = GetActiveWindow();

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = canvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = canvas.GetComponent<EventSystem>();

#if !UNITY_EDITOR
        MARGINS margins = new MARGINS {cxLeftWidth = -1};
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        //SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
#endif
    }

    private void Update() {
        RaycastHit hit = new RaycastHit();
        bool test1 = !Physics.Raycast (mainCamera.ScreenPointToRay (Input.mousePosition).origin,
                mainCamera.ScreenPointToRay (Input.mousePosition).direction, out hit, 100,
                Physics.DefaultRaycastLayers);
        
         //Check if the left Mouse button is clicked
        
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);
        
        // if(!test1) 
        //     Debug.Log("Raycast has hit something");
        // if(results.Count > 0) {
        //     foreach (RaycastResult result in results)
        //     {
        //         Debug.Log("Hit " + result.gameObject.name);
        //     }
        // }

        //bool test2 = ActivateAssistant.menuOpen;

        //bool clickThrough = !test2 || test1;

        if(results.Count == 0) {
            SetClickthrough(test1);
        } else {
            SetClickthrough(false);
        }

        //SetClickthrough(results.Count != 0 || test1);
        
    }

    // private void Update() {
    //     #if !UNITY_EDITOR
    //     SetClickthrough(Physics2D.OverlapPoint(GetMouseWorldPosition()) == null);

    //     #endif
    // }


    private static Vector3 GetMouseWorldPosition() {
            Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vec.z = 0f;
            return vec;
    }

    public void SetClickthrough(bool clickthrough) {
#if !UNITY_EDITOR
        if (clickthrough) {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
            // CLICKTHROUGH
        } else {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
            // NOT CLICKTHROUGH
        }
        this.clickThrough = clickthrough;
        
#endif
    
    
    }

    private static Vector3 GetMouseWorldPositionWithZ() {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }
    private static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera) {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }
    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera) {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }


}
