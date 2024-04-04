using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class RouletteManager : MonoBehaviour
{
    float m_rotationSpeed = 0;
    public float m_Power;

    public Image m_PwBarImg ;

    public Text Power_Text;

    public int ChoiceNum;

    public float Choice1st;

    bool IsRotate = false;

    GameManager m_GameMgr = null;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;


        //오브젝트 찾는 방법1
        GameObject a_GObj = GameObject.Find("GameManager");
        if (a_GObj != null)
            m_GameMgr = a_GObj.GetComponent<GameManager>();

        //오브젝트 찾는 방법2
        m_GameMgr = GameObject.FindObjectOfType<GameManager>();


    }

    // Update is called once per frame
    void Update()
    {
        if (m_GameMgr.ChoiceNumUI.Length <= m_GameMgr.ClickCount)
            return;  //5개의 슬롯에 숫자값이 모두 채워졌다면 더이상 룰렛이 돌아가지 않도록
          
        if (IsRotate == false)
        {
            if (GameManager.IsPointerOverUIObject() == true)//마우스가UI 위에 있다면
                return;

            if (Input.GetMouseButton(0))
            {
                m_rotationSpeed += Time.deltaTime*20;
                m_PwBarImg.fillAmount = m_rotationSpeed / 100.0f;
                Power_Text.text = (int)(m_PwBarImg.fillAmount*100) + " / 100";

                if (m_rotationSpeed >= 100)
                    m_rotationSpeed = 100;    
            }

            if (Input.GetMouseButtonUp(0))
            {
                IsRotate = true;
            }
        }

        transform.Rotate(0, 0, m_rotationSpeed);
       
        m_rotationSpeed *= 0.98f;
        if (IsRotate == true)
        { 
            if (m_rotationSpeed <= 0.1)
            {
                m_rotationSpeed = 0;

                Roulette();
                IsRotate = false;
            }
        }
        m_PwBarImg.fillAmount = m_rotationSpeed / 100.0f;
        Power_Text.text = (int)(m_PwBarImg.fillAmount * 100) + " / 100"; 
    }

    void Roulette()
    {
        float a_Angle = transform.eulerAngles.z;

        if (89.9 < a_Angle && a_Angle <= 125.5)
        {
            ChoiceNum = 0;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if(125.5 < a_Angle && a_Angle <= 162)
        {
            ChoiceNum = 1;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (162 < a_Angle && a_Angle <= 197.6)
        {
            ChoiceNum = 2;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (197.6 < a_Angle && a_Angle <= 234.2)
        {
            ChoiceNum = 3;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (234.2 < a_Angle && a_Angle <= 270)
        {
            ChoiceNum = 4;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (270 < a_Angle && a_Angle <= 306)
        {
            ChoiceNum = 5;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (306 < a_Angle && a_Angle <= 342.3)
        {
            ChoiceNum = 6;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if ((342.3 < a_Angle && a_Angle < 360) || (0 <= a_Angle && a_Angle < 18.2))
        {
            ChoiceNum = 7;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (18.2 < a_Angle && a_Angle <= 54.3)
        {
            ChoiceNum = 8;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }
        else if (54.3 < a_Angle && a_Angle <= 89.9)
        {
            ChoiceNum = 9;
            //ChoiceNumUI[a_Try].text = ChoiceNum.ToString();
        }

        if (m_GameMgr != null)
            m_GameMgr.SetNumber(ChoiceNum);
    }

    
}
