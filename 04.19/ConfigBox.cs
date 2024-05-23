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

        //체크 상태가 변경되었을 때 호출되는 함수 대기시키는 코드
        if (m_SoundToggle != null)
            m_SoundToggle.onValueChanged.AddListener(SoundOnOff);

        //슬라이더가 조정되었을 때 호출되는 함수 대기시키는 코드
        if (m_VolumeSlider != null)
            m_VolumeSlider.onValueChanged.AddListener(VolumeChanged);

        //Hierarchy에서 HeroCtrl컴포넌트가 붙어있는 게임오브젝트를 찾아서 객체로 가져옴
        m_RefHero = FindObjectOfType<HeroCtrl>();


        //체크상태, 슬라이드 상태, 닉네임 로딩 후 UI컨트롤에 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (m_SoundToggle != null)
        {
            //if (a_SoundOnOff == 1)
            //    m_SoundToggle.isOn = true;
            //else
            //    m_SoundToggle.isOn = false;

            //삼항조건연산식
            m_SoundToggle.isOn = (a_SoundOnOff == 1) ? true : false;
        }

        if (m_VolumeSlider != null)
            m_VolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);


        Text a_Placeholder = null;
        if (NickInputField != null)
        {
            //인풋필드 플레이스 홀더에 적용
            Transform a_PLHTr = NickInputField.transform.Find("Placeholder");
            a_Placeholder = a_PLHTr.GetComponent<Text>();
            if (a_Placeholder != null)
                a_Placeholder.text = PlayerPrefs.GetString("UserNick", "나 강림");
            //인풋필드 텍스트상자에 바로 적용
            //NickInputField.text = PlayerPrefs.GetString("UserNick", "나 강림");

        }
    }


    void Update()
    {
        
    }
    private void SaveBtnClick()
    {
        //닉네임 주인공 머리 위에 적용
        if (NickInputField != null && NickInputField.text.Trim() != "")
        {
            string NickStr = NickInputField.text.Trim();
            if (m_RefHero != null)
                m_RefHero.ChangeNickName(NickStr);

            GlobalUserData.g_NickName = NickStr;
            PlayerPrefs.SetString("UserNick", NickStr);
        }


        Time.timeScale = 1.0f;      //일시정지 해제
        Destroy(gameObject);
    }
    private void CloseBtnClick()
    {
        Time.timeScale = 1.0f;
        Destroy(gameObject);
    }

    private void SoundOnOff(bool value)
    {
        //체크 상태 저장
        //if (value == true)
        //    PlayerPrefs.SetInt("SoundOnOff", 1);
        //else
        //    PlayerPrefs.SetInt("SoundOnOff", 0);

        int a_Value = (value == true) ? 1 : 0;
        PlayerPrefs.SetInt("SoundOnOff", a_Value);  
        //적용시점은 값을 조정하자마자 저장됨
        //확인을 눌러야만 저장되게끔 하고 싶으면 이 코드를 확인버튼 함수로 옮기자
        
        Sound_Mgr.Inst.SoundOnOff(value);
               
    }

    private void VolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SoundVolume", value);
        Sound_Mgr.Inst.SoundVolume(value);
    }
}
