using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public Text printText = null;
    public Button jsonFileLoadBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (jsonFileLoadBtn != null)
            jsonFileLoadBtn.onClick.AddListener(JsonFileLoadClick);
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
}
