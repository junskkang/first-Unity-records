using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Title_Mgr : MonoBehaviour
{
    public Button StartBtn;

    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData.LoadGameInfo();

        StartBtn.onClick.AddListener(StartClick);

        Sound_Mgr.Inst.PlayBGM("sound_bgm_title_001", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartClick()
    {
        //Debug.Log("버튼을 클릭 했어요.");
        SceneManager.LoadScene("LobbyScene");

        Sound_Mgr.Inst.PlayGUISound("Pop", 0.4f);
    }
}
