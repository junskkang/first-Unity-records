using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Button gameStartBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (gameStartBtn != null)
            gameStartBtn.onClick.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GameStart()
    {
        SceneManager.LoadScene("GameScene");
    }
}
