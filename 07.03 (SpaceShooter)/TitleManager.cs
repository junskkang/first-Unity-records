using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject loginPanel;
    public Button loginBtn;
    public Button createAccountBtn;
    public Button exitBtn;
    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.LoadGameDate();
        if (loginBtn != null)
            loginBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

        if (createAccountBtn != null)
            createAccountBtn.onClick.AddListener(() =>
            {
                
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
