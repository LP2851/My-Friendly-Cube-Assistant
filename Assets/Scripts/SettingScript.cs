using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{
    string settingValue;
    const string noSetting = "NO_SETTING";

    // Start is called before the first frame update
    void Awake() {
        settingValue = @PlayerPrefs.GetString(gameObject.tag, noSetting);

        if(settingValue != noSetting) {
            gameObject.GetComponent<InputField>().text = settingValue;
        }

    }

    public void UpdatedSettingValue(string val) {
        settingValue = @val;
        PlayerPrefs.SetString(gameObject.tag, @val);
        PlayerPrefs.Save();
    }
}
