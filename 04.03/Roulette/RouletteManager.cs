using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteManager : MonoBehaviour
{
    float m_rotationSpeed = 0;
    public float m_Power = 0;

    public Image m_PwBarImg ;

    public int ChoiceNum;
    public Text[] ChoiceNumUI;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && m_rotationSpeed == 0 && m_Power >= 0)
        {
            m_Power += Time.deltaTime*20;

            if (m_Power >= 100)
                m_Power = 100;  
        }
        m_PwBarImg.fillAmount = m_Power / 100.0f;

        if (Input.GetMouseButtonUp(0) && m_Power!= 0)
        {
            m_rotationSpeed = m_Power;
            m_Power = 0;
        }

        transform.Rotate(0, 0, m_rotationSpeed);

        m_rotationSpeed *= 0.98f;

        if (m_rotationSpeed <= 0.1)
        {
            m_rotationSpeed = 0;
        }

        m_PwBarImg.fillAmount = m_rotationSpeed / 100.0f;

        if (89.9 < transform.transform.eulerAngles.z && transform.transform.eulerAngles.z <= 125.5)
        {
            ChoiceNum = 0;
            ChoiceNumUI[0].text = ChoiceNum.ToString();
        }

    }
}
