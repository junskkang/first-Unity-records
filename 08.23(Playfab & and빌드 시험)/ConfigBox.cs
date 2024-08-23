using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;
using PlayFab;

public class ConfigBox : MonoBehaviour
{
    public Button m_Ok_Btn = null;
    public Button m_Close_Btn = null;

    public InputField NickInputField = null;

    public Toggle m_Sound_Toggle = null;
    public Slider m_Sound_Slider = null;

    HeroCtrl m_RefHero = null;

    public Text messageText = null;
    float showMsTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (m_Ok_Btn != null)
            m_Ok_Btn.onClick.AddListener(OkBtnClick);

        if (m_Close_Btn != null)
            m_Close_Btn.onClick.AddListener(CloseBtnClick);

        if (m_Sound_Toggle != null) 
            m_Sound_Toggle.onValueChanged.AddListener(SoundOnOff);
        //üũ ���°� ����Ǿ��� �� ȣ��Ǵ� �Լ��� ����ϴ� �ڵ�

        if (m_Sound_Slider != null)
            m_Sound_Slider.onValueChanged.AddListener(SliderChanged);
        //�����̵� ���°� ���� �Ǿ��� �� ȣ��Ǵ� �Լ� ����ϴ� �ڵ�

        m_RefHero = FindObjectOfType<HeroCtrl>();
        //Hierarchy�ʿ��� HeroCtrl ������Ʈ�� �پ��ִ� ���ӿ�����Ʈ�� ã�Ƽ� ��ü�� ã�ƿ��� ���

        //--- üũ����, �����̵����, �г��� �ε� �� UI ��Ʈ�ѿ� ����
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if(m_Sound_Toggle != null)
        {
            //if (a_SoundOnOff == 1)
            //    m_Sound_Toggle.isOn = true;
            //else
            //    m_Sound_Toggle.isOn = false;

            m_Sound_Toggle.isOn = (a_SoundOnOff == 1) ? true : false;   
        }

        if (m_Sound_Slider != null)
            m_Sound_Slider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        //Text a_Placehoder = null;
        if (NickInputField != null)
        {
            //Transform a_PLHTr = NickInputField.transform.Find("Placeholder");
            //a_Placehoder = a_PLHTr.GetComponent<Text>();
            //if (a_Placehoder != null)
            //    a_Placehoder.text = PlayerPrefs.GetString("UserNick", "��ɲ�");
            NickInputField.text = GlobalUserData.g_NickName;
        }
        //--- üũ����, �����̵����, �г��� �ε� �� UI ��Ʈ�ѿ� ����

    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < showMsTimer)
        {
            showMsTimer -= Time.unscaledDeltaTime;
            if (showMsTimer <= 0.0f)
            {
                MessageOnOff("", false);
            }
        }
    }

    private void OkBtnClick()
    {
        if(NickInputField != null && NickInputField.text.Trim() != "")
        {
            string NickStr = NickInputField.text.Trim();

            if (string.IsNullOrEmpty(NickStr) == true)
            {
                //MessageOnOff("������ ��ĭ ���� �Է��� �ּ���.");
                return;
            }

            if ((3 <= NickStr.Length && NickStr.Length <= 20) == false)
            {
                //MessageOnOff("������ 3���� �̻� 20���� ���Ϸ� �ۼ��� �ּ���.");
                return;
            }


            PlayerPrefs.SetString("NewNick", NickStr);

            Network_Mgr.Inst.PushPacket(PacketType.NickUpdate);
            Network_Mgr.Inst.TakeConfigBox(this);
        }//if(NickInputField != null && NickInputField.text.Trim() != "")
    }

    private void CloseBtnClick()
    {
        Time.timeScale = 1.0f;  //�Ͻ����� Ǯ���ֱ�
        Destroy(gameObject);
    }

    private void SoundOnOff(bool value) //üũ ���°� ����Ǿ��� �� ȣ��Ǵ� �Լ�
    {
        //--- üũ ���� ����
        //if (value == true)
        //    PlayerPrefs.SetInt("SoundOnOff", 1);
        //else
        //    PlayerPrefs.SetInt("SoundOnOff", 0);

        int a_IntV = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt("SoundOnOff", a_IntV);

        SoundMgr.Inst.SoundOnOff(value);    //���� �� / ��
        //--- üũ ���� ����
    }

    private void SliderChanged(float value) 
    { //value 0.0f ~ 1.0f �����̵� ���°� ���� �Ǿ��� �� ȣ��Ǵ� �Լ�
        PlayerPrefs.SetFloat("SoundVolume", value);
        SoundMgr.Inst.SoundVolume(value);
    }

    public void MessageOnOff(string Mess = "", bool isOn = true)
    {
        if (isOn == true)
        {
            messageText.text = Mess;
            messageText.gameObject.SetActive(true);
            showMsTimer = 7.0f;
        }
        else
        {
            messageText.text = "";
            messageText.gameObject.SetActive(false);
        }
    }
}
