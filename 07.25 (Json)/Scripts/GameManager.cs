using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //외부에서 Json파일을 만들고 불러오기 위한 UI 
    public Text printText = null;
    public Button jsonFileLoadBtn = null;

    //직접 Json파일을 만들고 불러오기 위한 UI
    public Button myJsonMakeBtn;
    public Button myJsonLoadBtn;
    string strJson = "";

    // Start is called before the first frame update
    void Start()
    {
        if (jsonFileLoadBtn != null)
            jsonFileLoadBtn.onClick.AddListener(JsonFileLoadClick);

        if (myJsonMakeBtn != null)
            myJsonMakeBtn.onClick.AddListener(MyJsonMakeClick);

        if (myJsonLoadBtn != null)
            myJsonLoadBtn.onClick.AddListener(MyJsonLoadClick);
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void JsonFileLoadClick()
    {
        if (printText != null)
        {
            string strOutput = "";

            //Resources 폴더의 JSON 파일을 로드
            TextAsset JsonData = Resources.Load("user_Info") as TextAsset;

            //TextAsset에서 string 타입의 텍스트를 추출
            string strJsonData = JsonData.text;
            //Debug.Log(strJsonData);

            //JSON 파일을 파싱
            var N = JSON.Parse(strJsonData);
            //JSONNode N = JSON.Parse(strJsonData); 
            //위와 동일함. Parse()함수를 통해 JSONNODE 데이터형으로 리턴됨

            //Json 형식은 파싱이 끝나고 나면 순서에 상관없이 키 값으로 로딩할 수 있다.
            if (N["직업"] != null)
            {
                string user_job = N["직업"];
                strOutput += "N[\"직업\"] : " + user_job + "\n";
            }
            
            //"이름" 키에 저장된 값 추출
            if (N["이름"] != null)
            {
                string user_Name = N["이름"];
                strOutput += "N[\"이름\"] : " + user_Name + "\n";     
                //문자열 안에서 쌍따옴표를 출력하기 위해서 \"형태로 사용함
            }

            if (N["성별"] != null)
            {
                string user_gender = N["성별"];
                strOutput += "N[\"성별\"] : " + user_gender + "\n";
            }

            strOutput += "\n";

            //"능력치" 객체 중 하위 항목을 추출하기
            if (N["능력치"] != null && N["능력치"]["레벨"] != null)
            {
                int level = N["능력치"]["레벨"].AsInt;   
                //json은 다 문자열이어서 int로 형변환 필요
                strOutput += "N[\"능력치\"][\"레벨\"] : " + level + "\n";
            }

            if (N["능력치"] != null && N["능력치"]["활력"] != null)
            {
                int energy = N["능력치"]["활력"].AsInt;
                //json은 다 문자열이어서 int로 형변환 필요
                strOutput += "N[\"능력치\"][\"활력\"] : " + energy + "\n";
            }
            
            if (N["능력치"] != null && N["능력치"]["생명력"] != null)
            {
                int maxHp = N["능력치"]["생명력"].AsInt;
                //json은 다 문자열이어서 int로 형변환 필요
                strOutput += "N[\"능력치\"][\"생명력\"] : " + maxHp + "\n";
            }

            if (N["능력치"] != null && N["능력치"]["마나"] != null)
            {
                int mana = N["능력치"]["마나"].AsInt;
                //json은 다 문자열이어서 int로 형변환 필요
                strOutput += "N[\"능력치\"][\"마나\"] : " + mana + "\n";
            }

            strOutput += "\n";

            //"보유스킬"의 배열 추출하기
            if (N["보유스킬"]!= null )
            {
                for (int i = 0; i < N["보유스킬"].Count; i++) 
                {
                    strOutput += "N[\"보유스킬\"][" + i + "] :" + N["보유스킬"][i] + "\n";
                }
            }

            strOutput += "\n";

            //정수 추출하기
            if (N["스코어"] != null)
            {
                int user_Score = N["스코어"].AsInt;
                strOutput += "N[\"스코어\"] : " + user_Score + "\n";
            }


            printText.text = strOutput;

        }
    }

    private void MyJsonMakeClick()
    {
        //유니티에서 Json 개체 생성
        JSONObject makeJson = new JSONObject();
        makeJson["StrData"] = "SBS코딩";
        //같은 키값이 이미 존재하면 덮어씌우기 된다.
        makeJson["Level"] = 777;
        makeJson["BoolTest"] = true;
        makeJson["X_Position"] = 123123.8143;
        makeJson["FValue"] = 3.14f;

        //Json 배열 만들기 RkList = 랭킹리스트
        makeJson["RkList"][0] = "AAA";      //아이디
        makeJson["RkList"][1] = "너구리";   //닉네임
        makeJson["RkList"][2] = 111;        //점수
        makeJson["RkList"][3] = "BBB";
        makeJson["RkList"][4] = "고양이";
        makeJson["RkList"][5] = 222;
        makeJson["RkList"][6] = "CCC";
        makeJson["RkList"][7] = "팔라독";
        makeJson["RkList"][8] = 333;

        strJson = makeJson.ToString();
        Debug.Log(strJson);
    }

    private void MyJsonLoadClick()
    {
        string strOutput = "";

        //Json Parsing
        if (string.IsNullOrEmpty(strJson)) return;

        JSONNode parseJs = JSON.Parse(strJson);

        if (parseJs["StrData"] != null)
        {
            string strDt = parseJs["StrData"];
            strOutput += "문자열Data : " + strDt + "\n";
        }

        if (parseJs["BoolTest"] != null)
        {
            bool value = parseJs["BoolTest"].AsBool;
            strOutput += "BoolTest : " + value + "\n";
        }

        if (parseJs["X_Position"] != null)
        {
            double xPos = parseJs["X_Position"].AsDouble;
            strOutput += "X_Position : " + xPos + "\n";
        }

        if (parseJs["FValue"] != null)
        {
            float fValue = parseJs["FValue"].AsFloat;
            strOutput += "FValue : " + fValue + "\n";
        }

        if (parseJs["Level"] != null)
        {
            int value = parseJs["Level"].AsInt;
            strOutput += "Level : " + value + "\n";
        }

        strOutput += "\n";

        //배열 추출하기
        int ranking = 0;
        if (parseJs["RkList"] != null)
            for (int i = 0; i < parseJs["RkList"].Count; i++)
            { 
                if ((i % 3) == 0)   //한 사람당 사용하는 배열이 3개이므로 3으로 나눠서 사용
                {
                    ranking = (i / 3) + 1;
                    strOutput += ranking + " 등 : ";
                    
                    int add = i;
                    string userId = parseJs["RkList"][add];
                    strOutput += "UserId (" + userId + "), ";
                    add++;

                    string userName = parseJs["RkList"][add];
                    strOutput += "UserName (" + userName + "), ";
                    add++;

                    int bestScore = parseJs["RkList"][add].AsInt;
                    strOutput += "BestScore (" + bestScore + ")\n";
                }
            }


        printText.text = strOutput;

    }
}
