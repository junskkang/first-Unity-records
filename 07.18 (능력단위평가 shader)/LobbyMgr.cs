using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    public Button m_Start_Btn;
    public Button optionBtn;
    public Button creditBtn;
    Vector3 enterScale = new Vector3(1.1f, 1.1f, 1.0f);

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f; //일시정지를 원래 속도로...

        if (m_Start_Btn != null)
            m_Start_Btn.onClick.AddListener(StartBtnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBtnClick()
    {
        SceneManager.LoadScene("scLevel01");
        SceneManager.LoadScene("scPlay", LoadSceneMode.Additive);
    }

    public void BtnEnter(Button btn)
    {
        btn.GetComponentInChildren<Outline>().enabled = true;
        btn.GetComponent<RectTransform>().localScale = enterScale;
        btn.GetComponentInChildren<Text>().fontSize += 3;
    }

    public void BtnExit(Button btn)
    {
        btn.GetComponentInChildren<Outline>().enabled = false;
        btn.GetComponent<RectTransform>().localScale = Vector3.one;
        btn.GetComponentInChildren<Text>().fontSize -= 3;
    }
}
