using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotifyCtrl : MonoBehaviour
{
    public Text titleText;
    public Text messageText;
    public Button confirmButton;
    string savetext = "@@";
    public static bool isNotify = false;
    // Start is called before the first frame update

    void Start()
    {

        if (confirmButton != null)
            confirmButton.onClick.AddListener(() =>
            {
                isNotify = false;
                Time.timeScale = GameManager.Inst.isFast;
                gameObject.SetActive(false);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyOn(string msg)
    {
        string change = messageText.text.Replace(savetext, msg);        
        messageText.text = change;
        savetext = msg;
        isNotify = true;
        Time.timeScale = 0;
    }
}
