using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Button ToLobbyBtn;
    // Start is called before the first frame update
    void Start()
    {
        if (ToLobbyBtn != null)
            ToLobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
