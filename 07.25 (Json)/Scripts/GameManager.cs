using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //�ܺο��� Json������ ����� �ҷ����� ���� UI 
    public Text printText = null;
    public Button jsonFileLoadBtn = null;

    //���� Json������ ����� �ҷ����� ���� UI
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

            //Resources ������ JSON ������ �ε�
            TextAsset JsonData = Resources.Load("user_Info") as TextAsset;

            //TextAsset���� string Ÿ���� �ؽ�Ʈ�� ����
            string strJsonData = JsonData.text;
            //Debug.Log(strJsonData);

            //JSON ������ �Ľ�
            var N = JSON.Parse(strJsonData);
            //JSONNode N = JSON.Parse(strJsonData); 
            //���� ������. Parse()�Լ��� ���� JSONNODE ������������ ���ϵ�

            //Json ������ �Ľ��� ������ ���� ������ ������� Ű ������ �ε��� �� �ִ�.
            if (N["����"] != null)
            {
                string user_job = N["����"];
                strOutput += "N[\"����\"] : " + user_job + "\n";
            }
            
            //"�̸�" Ű�� ����� �� ����
            if (N["�̸�"] != null)
            {
                string user_Name = N["�̸�"];
                strOutput += "N[\"�̸�\"] : " + user_Name + "\n";     
                //���ڿ� �ȿ��� �ֵ���ǥ�� ����ϱ� ���ؼ� \"���·� �����
            }

            if (N["����"] != null)
            {
                string user_gender = N["����"];
                strOutput += "N[\"����\"] : " + user_gender + "\n";
            }

            strOutput += "\n";

            //"�ɷ�ġ" ��ü �� ���� �׸��� �����ϱ�
            if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["����"] != null)
            {
                int level = N["�ɷ�ġ"]["����"].AsInt;   
                //json�� �� ���ڿ��̾ int�� ����ȯ �ʿ�
                strOutput += "N[\"�ɷ�ġ\"][\"����\"] : " + level + "\n";
            }

            if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["Ȱ��"] != null)
            {
                int energy = N["�ɷ�ġ"]["Ȱ��"].AsInt;
                //json�� �� ���ڿ��̾ int�� ����ȯ �ʿ�
                strOutput += "N[\"�ɷ�ġ\"][\"Ȱ��\"] : " + energy + "\n";
            }
            
            if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["�����"] != null)
            {
                int maxHp = N["�ɷ�ġ"]["�����"].AsInt;
                //json�� �� ���ڿ��̾ int�� ����ȯ �ʿ�
                strOutput += "N[\"�ɷ�ġ\"][\"�����\"] : " + maxHp + "\n";
            }

            if (N["�ɷ�ġ"] != null && N["�ɷ�ġ"]["����"] != null)
            {
                int mana = N["�ɷ�ġ"]["����"].AsInt;
                //json�� �� ���ڿ��̾ int�� ����ȯ �ʿ�
                strOutput += "N[\"�ɷ�ġ\"][\"����\"] : " + mana + "\n";
            }

            strOutput += "\n";

            //"������ų"�� �迭 �����ϱ�
            if (N["������ų"]!= null )
            {
                for (int i = 0; i < N["������ų"].Count; i++) 
                {
                    strOutput += "N[\"������ų\"][" + i + "] :" + N["������ų"][i] + "\n";
                }
            }

            strOutput += "\n";

            //���� �����ϱ�
            if (N["���ھ�"] != null)
            {
                int user_Score = N["���ھ�"].AsInt;
                strOutput += "N[\"���ھ�\"] : " + user_Score + "\n";
            }


            printText.text = strOutput;

        }
    }

    private void MyJsonMakeClick()
    {
        //����Ƽ���� Json ��ü ����
        JSONObject makeJson = new JSONObject();
        makeJson["StrData"] = "SBS�ڵ�";
        //���� Ű���� �̹� �����ϸ� ������ �ȴ�.
        makeJson["Level"] = 777;
        makeJson["BoolTest"] = true;
        makeJson["X_Position"] = 123123.8143;
        makeJson["FValue"] = 3.14f;

        //Json �迭 ����� RkList = ��ŷ����Ʈ
        makeJson["RkList"][0] = "AAA";      //���̵�
        makeJson["RkList"][1] = "�ʱ���";   //�г���
        makeJson["RkList"][2] = 111;        //����
        makeJson["RkList"][3] = "BBB";
        makeJson["RkList"][4] = "�����";
        makeJson["RkList"][5] = 222;
        makeJson["RkList"][6] = "CCC";
        makeJson["RkList"][7] = "�ȶ�";
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
            strOutput += "���ڿ�Data : " + strDt + "\n";
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

        //�迭 �����ϱ�
        int ranking = 0;
        if (parseJs["RkList"] != null)
            for (int i = 0; i < parseJs["RkList"].Count; i++)
            { 
                if ((i % 3) == 0)   //�� ����� ����ϴ� �迭�� 3���̹Ƿ� 3���� ������ ���
                {
                    ranking = (i / 3) + 1;
                    strOutput += ranking + " �� : ";
                    
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
