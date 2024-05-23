using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public Button m_SaveBtn;
    public Button m_CloseBtn;

    public InputField NickInputField;

    public Toggle m_SoundToggle;
    public Slider m_VolumeSlider;

    HeroCtrl m_RefHero = null;

    void Start()
    {
        if (m_SaveBtn != null)
            m_SaveBtn.onClick.AddListener(SaveBtnClick);

        if (m_CloseBtn != null)
            m_CloseBtn.onClick.AddListener(CloseBtnClick);

        //üũ ���°� ����Ǿ��� �� ȣ��Ǵ� �Լ� ����Ű�� �ڵ�
        if (m_SoundToggle != null)
            m_SoundToggle.onValueChanged.AddListener(SoundOnOff);

        //�����̴��� �����Ǿ��� �� ȣ��Ǵ� �Լ� ����Ű�� �ڵ�
        if (m_VolumeSlider != null)
            m_VolumeSlider.onValueChanged.AddListener(VolumeChanged);

        //Hierarchy���� HeroCtrl������Ʈ�� �پ��ִ� ���ӿ�����Ʈ�� ã�Ƽ� ��ü�� ������
        m_RefHero = FindObjectOfType<HeroCtrl>();


        //üũ����, �����̵� ����, �г��� �ε� �� UI��Ʈ�ѿ� ����
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (m_SoundToggle != null)
        {
            //if (a_SoundOnOff == 1)
            //    m_SoundToggle.isOn = true;
            //else
            //    m_SoundToggle.isOn = false;

            //�������ǿ����
            m_SoundToggle.isOn = (a_SoundOnOff == 1) ? true : false;
        }

        if (m_VolumeSlider != null)
            m_VolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);


        Text a_Placeholder = null;
        if (NickInputField != null)
        {
            //��ǲ�ʵ� �÷��̽� Ȧ���� ����
            Transform a_PLHTr = NickInputField.transform.Find("Placeholder");
            a_Placeholder = a_PLHTr.GetComponent<Text>();
            if (a_Placeholder != null)
                a_Placeholder.text = PlayerPrefs.GetString("UserNick", "�� ����");
            //��ǲ�ʵ� �ؽ�Ʈ���ڿ� �ٷ� ����
            //NickInputField.text = PlayerPrefs.GetString("UserNick", "�� ����");

        }
    }


    void Update()
    {
        
    }
    private void SaveBtnClick()
    {
        //�г��� ���ΰ� �Ӹ� ���� ����
        if (NickInputField != null && NickInputField.text.Trim() != "")
        {
            string NickStr = NickInputField.text.Trim();
            if (m_RefHero != null)
                m_RefHero.ChangeNickName(NickStr);

            GlobalUserData.g_NickName = NickStr;
            PlayerPrefs.SetString("UserNick", NickStr);
        }


        Time.timeScale = 1.0f;      //�Ͻ����� ����
        Destroy(gameObject);
    }
    private void CloseBtnClick()
    {
        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }

    private void SoundOnOff(bool value)
    {
        //üũ ���� ����
        //if (value == true)
        //    PlayerPrefs.SetInt("SoundOnOff", 1);
        //else
        //    PlayerPrefs.SetInt("SoundOnOff", 0);

        int a_Value = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt("SoundOnOff", a_Value);  
        //��������� ���� �������ڸ��� �����
        //Ȯ���� �����߸� ����ǰԲ� �ϰ� ������ �� �ڵ带 Ȯ�ι�ư �Լ��� �ű���
        
        Sound_Mgr.Inst.SoundOnOff(value);
               
    }

    private void VolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Inst.SoundVolume(value);
    }
}
