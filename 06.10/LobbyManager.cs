using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("UI")]
    public Button startBtn;
    Vector3 enterScale = new Vector3(1.1f, 1.1f, 1.0f);
    public Button optionBtn;
    public Button creditBtn;
    
    void Start()
    {
        Time.timeScale = 1.0f;

        if (startBtn != null)
            startBtn.onClick.AddListener(StartBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("scPlay");
    }

    public void BtnEnter(Button button)
    {
        button.GetComponentInChildren<Outline>().enabled = true;
        button.GetComponent<RectTransform>().localScale = enterScale;
        button.GetComponentInChildren<Text>().fontSize += 3;
    }

    public void BtnExit(Button button)
    {
        button.GetComponentInChildren<Outline>().enabled = false;
        button.GetComponent<RectTransform>().localScale = Vector3.one;
        button.GetComponentInChildren<Text>().fontSize -= 3;
    }
}
