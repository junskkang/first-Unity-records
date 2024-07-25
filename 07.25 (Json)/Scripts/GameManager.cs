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
}
