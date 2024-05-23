using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound_Mgr : G_Singleton<Sound_Mgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_AdClipList = new Dictionary<string, AudioClip>();

    float m_bgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    //효과음 최적화를 위한 버퍼 변수
    int m_EffSdCount = 5;   //지금은 5개의 레이어로 플레이
    int m_SoundCount = 0;   //최대 5개까지 재생되게 제어시킬 변수
    GameObject[] m_SdObjList = new GameObject[10];
    AudioSource[] m_SdSrcList = new AudioSource[10];
    float[] m_EffVolume = new float[10];

    protected override void Init()      //Awake() 함수 대신에 사용
    {
        base.Init();    //부모쪽에 있는 Init()함수 호출

        LoadChildGameObj();
    }


    void Start()
    {
        //사운드 리소스 미리 로딩
        AudioClip a_GAudioClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for (int i = 0; i < temp.Length; i++)
        {
            a_GAudioClip = temp[i] as AudioClip;

            if (m_AdClipList.ContainsKey(a_GAudioClip.name) == true)
                continue;

            m_AdClipList.Add(a_GAudioClip.name, a_GAudioClip);
        }
    }

    bool m_TestSOnOff = true;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) == true)
        {
            m_TestSOnOff = !m_TestSOnOff;
            SoundOnOff(m_TestSOnOff);
        }
        //VolumeControlTest();
    }

    private void LoadChildGameObj()
    {
        m_AudioSrc = gameObject.AddComponent<AudioSource>();

        //게임 효과음 플레이를 위한 5개의 레이어 생성코드
        for (int i = 0; i < m_EffSdCount; i++)
        {
            GameObject newSndObj = new GameObject();
            newSndObj.transform.SetParent(transform);
            newSndObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSndObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            newSndObj.name = "SoundEffObj";

            m_SdSrcList[i] = a_AudioSrc;
            m_SdObjList[i] = newSndObj;
        }

        //게임이 시작되면 사운드 OnOff, 사운드 볼륨 로컬 로딩 후 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (a_SoundOnOff == 1)
            SoundOnOff(true);
        else
            SoundOnOff(false);

        float a_Value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        SoundVolume(a_Value);
    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;
        if (m_AdClipList.ContainsKey(a_FileName) == true)   //가져온 소스 리스트 중에 존재하면 바로 가져옴
        {
            a_GAudioClip = m_AdClipList[a_FileName];
        }
        else    //없다면 다시 한번 로딩
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_AdClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null) return;

        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName) return;

        //오디오소스 옵션설정
        m_AudioSrc.clip = a_GAudioClip;     //오디오 컴포넌트에 받아온 파일소스 연결
        m_AudioSrc.volume = fVolume * m_SoundVolume;    //볼륨 옵션 조절
        m_bgmVolume = fVolume;
        m_AudioSrc.loop  = true;    //반복 o
        m_AudioSrc.Play();          //오디오 소스 컴포넌트에 AudioClip에 연결되어 있는 애를 재생
    }

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {
        //GUI효과음 플레이를 위한 함수

        if (m_SoundOnOff == false) return;

        AudioClip a_GAudioClip = null;

        if (m_AdClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_AdClipList[a_FileName];
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_AdClipList.Add(a_FileName, a_GAudioClip);
        }

        if(m_AudioSrc == null) return;

        m_AudioSrc.PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);  
        //중복 재생이 가능
        //새로운 재생시도가 들어와도 앞의 오디오클립이 온전히 다 재생시켜 줌
        //그렇다고 무한대로 중복 플레이를 시키지는 않음. 내부적인 버퍼링이 존재함
    }

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false) return;

        AudioClip a_GAudioClip = null;
        if (m_AdClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_AdClipList[a_FileName];
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_AdClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip == null) return;

        if (m_SdSrcList[m_SoundCount] != null)
        {
            m_SdSrcList[m_SoundCount].volume = 1.0f;
            m_SdSrcList[m_SoundCount].PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);
            m_EffVolume[m_SoundCount] = fVolume;

            m_SoundCount++;
            if (m_EffSdCount <= m_SoundCount)
                m_SoundCount = 0;
        }

    }

    public void SoundOnOff(bool a_OnOff = true)
    {
        bool a_MuteOnOff = !a_OnOff;

        if (m_AudioSrc != null)
        {
            m_AudioSrc.mute = a_MuteOnOff;  //뮤트시에 사운드 재생위치 일시정지

            //if (a_MuteOnOff == false)       //뮤트시에 사운드 재생위치 처음으로
            //    m_AudioSrc.time = 0.0f;
        }

        for (int i = 0; i < m_EffSdCount; i++) 
        { 
            if (m_SdSrcList[i] != null)
            {
                m_SdSrcList[i].mute = a_MuteOnOff;
                if (a_MuteOnOff == false)   //효과음은 처음부터 다시 플레이
                    m_SdSrcList[i].time = 0;
            }
        }

        m_SoundOnOff = a_OnOff;
    }

    public void SoundVolume(float fVolume)
    {
        if (m_AudioSrc != null)
            m_AudioSrc.volume = m_bgmVolume * fVolume;  //개별볼륨 * 전체볼륨

        //for (int i = 0; i < m_EffSdCount; i++)
        //{
        //    if (m_SdSrcList[i] != null)
        //        m_SdSrcList[i].volume = 1.0f;
        //}

        m_SoundVolume = fVolume;    //나머지 소리들 플레이될 때 계산공식에 있는 값 조정
    }


    void VolumeControlTest()
    {
        float m_TestVolume = 1.0f;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_TestVolume -= 0.1f;
            if (m_TestVolume < 0.0f)
                m_TestVolume = 0.0f;
            SoundVolume(m_TestVolume);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_TestVolume += 0.1f;
            if (m_TestVolume > 1.0f)
                m_TestVolume = 1.0f;
            SoundVolume(m_TestVolume);
        }
    }
}
