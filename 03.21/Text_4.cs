using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Unity C# 저장, 로딩 PlayerPrefab
//보안에는 취약함, 저장 경로를 쉽게 파악가능, 해당 정보 복사 가능
//해킹해도 상관 없는 옵션들 : 사운드 on/off, 볼륨을 몇으로 할건지, 등의 옵션설정 값
public class Text_4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.S) == true)
        {
            PlayerPrefs.SetString("NickName", "팔라독");            //저장
            PlayerPrefs.SetInt("UserGold", 120);
            PlayerPrefs.SetInt("UserLevel", 11);
            PlayerPrefs.SetFloat("AttackRate", 0.57f);
        }
        
        if(Input.GetKeyDown(KeyCode.L) == true)
        {
            string a_Nick = PlayerPrefs.GetString("NickName", "");  //불러오기
            int a_UserGold = PlayerPrefs.GetInt("UserGold", 0);
            int a_UserLevel = PlayerPrefs.GetInt("UserLevel", 0);
            float a_AttackRate = PlayerPrefs.GetFloat("AttackRate", 0.0f);

            Debug.Log("별명 : " + a_Nick + ", 골드 : " + a_UserGold + ", 레벨 : " + a_UserLevel
                + ", 공격력 : " + a_AttackRate);
        }

        if(Input.GetKeyDown(KeyCode.C) == true)
        {//전체 삭제
            PlayerPrefs.DeleteAll();     //이 프로젝트에 저장된 모든 정보를 삭제시키는 함수
            PlayerPrefs.DeleteKey("UserName"); //특정 키 값만 초기화 시켜주는 함수
        }
    }
}
