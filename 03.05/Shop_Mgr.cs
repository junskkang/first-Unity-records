using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shop_Mgr : MonoBehaviour
{
    public Button Shop_Back_Btn;
    void Start()
    {
        Shop_Back_Btn.onClick.AddListener(ShopBackBtn);
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShopBackBtn()
    {
        //Debug.Log("로비 씬으로 돌아가기");
        SceneManager.LoadScene("LobbyScene");
    }

}
