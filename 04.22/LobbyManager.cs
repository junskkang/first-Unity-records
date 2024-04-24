using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public Text idText;
    public Text pwText;
    public Text nickText;

    public Button logoutBtn;

    // Start is called before the first frame update
    void Start()
    {
        if(logoutBtn != null)
            logoutBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_Unique_Id = "";
                GlobalValue.g_Password = "";
                GlobalValue.g_NickName = "";
                SceneManager.LoadScene("TitleScene");
            });

        if (idText != null)
            idText.text = GlobalValue.g_Unique_Id;

        if (pwText != null)
            pwText.text = GlobalValue.g_Password;

        if (nickText != null)
            nickText.text = GlobalValue.g_NickName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
