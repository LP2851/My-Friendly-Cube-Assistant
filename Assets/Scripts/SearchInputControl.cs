using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchInputControl : MonoBehaviour
{
    const string webAddress = "www.google.co.uk/search?q=";
    const string locationNotFound = "NONE";

    public static string locationOfChrome;// = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";

    static string[] searchStarters = {"google", "google:", "search", "/google", "/search"};
    
    void Awake() {
        locationOfChrome = @PlayerPrefs.GetString("setting:browserLocation", locationNotFound);
        Debug.Log(locationOfChrome);
    }
    public void HandleUserInput(string txt) {
        Debug.Log("HandleUserInput has received " +txt);
        if(txt.Length > 0 && locationOfChrome != locationNotFound) {
            if(HasSearchStarter(txt)) 
                txt = RemoveSearchStarter(txt);

            DoSearch(txt);
            GetComponent<InputField>().text = "";
        } else if (locationOfChrome == locationNotFound) {
            GetComponent<InputField>().text = "Searching needs a location of browser to be set in the settings.";
        } 
    }

    private string ConstructWebAddress(string txt) {
        Debug.Log("Going to " + webAddress + txt.Replace(" ", "+"));
        return webAddress + txt.Replace(" ", "+");
    }

    void DoSearch(string txt) {
        if (txt.Length > 0 ) {
            System.Diagnostics.Process.Start(locationOfChrome, ConstructWebAddress(txt));
        } else {
            System.Diagnostics.Process.Start(locationOfChrome);
        }
    }



    private bool HasSearchStarter(string txt) {
        foreach(string s in searchStarters) {
            if(txt.StartsWith(s)) {
                Debug.Log("txt has search starter: "+ s);
                return true;
            }
        }
        return false;
    }

    private string RemoveSearchStarter(string txt) {
        string returnValue = txt;
        foreach(string s in searchStarters) {
            if(txt.StartsWith(s)) {
                string testValue = txt.Remove(0, s.Length);
                while(testValue.StartsWith(" ")) {
                    testValue = testValue.Remove(0, 1);
                }
                if (testValue.Length < returnValue.Length) 
                    returnValue = testValue;
            }
        }
        Debug.Log("txt is now being set to " + returnValue);
        return returnValue;
    }
}
