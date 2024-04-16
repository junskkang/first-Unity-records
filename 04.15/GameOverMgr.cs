using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMgr : MonoBehaviour
{
    public Text scoreText;
    public Text helpText;

    // Start is called before the first frame update
    void Start()
    {
        if (scoreText != null)
            scoreText.text = $"Score : {GameManager.Inst.score}";
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}
