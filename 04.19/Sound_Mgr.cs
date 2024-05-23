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

    //ȿ���� ����ȭ�� ���� ���� ����
    int m_EffSdCount = 5;   //������ 5���� ���̾�� �÷���
    int m_SoundCount = 0;   //�ִ� 5������ ����ǰ� �����ų ����
    GameObject[] m_SdObjList = new GameObject[10];
    AudioSource[] m_SdSrcList = new AudioSource[10];
    float[] m_EffVolume = new float[10];

    protected override void Init()      //Awake() �Լ� ��ſ� ���
    {
        base.Init();    //�θ��ʿ� �ִ� Init()�Լ� ȣ��

        LoadChildGameObj();
    }


    void Start()
    {
        //���� ���ҽ� �̸� �ε�
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

        //���� ȿ���� �÷��̸� ���� 5���� ���̾� �����ڵ�
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

        //������ ���۵Ǹ� ���� OnOff, ���� ���� ���� �ε� �� ����
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
        if (m_AdClipList.ContainsKey(a_FileName) == true)   //������ �ҽ� ����Ʈ �߿� �����ϸ� �ٷ� ������
        {
            a_GAudioClip = m_AdClipList[a_FileName];
        }
        else    //���ٸ� �ٽ� �ѹ� �ε�
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_AdClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null) return;

        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName) return;

        //������ҽ� �ɼǼ���
        m_AudioSrc.clip = a_GAudioClip;     //����� ������Ʈ�� �޾ƿ� ���ϼҽ� ����
        m_AudioSrc.volume = fVolume * m_SoundVolume;    //���� �ɼ� ����
        m_bgmVolume = fVolume;
        m_AudioSrc.loop  = true;    //�ݺ� o
        m_AudioSrc.Play();          //����� �ҽ� ������Ʈ�� AudioClip�� ����Ǿ� �ִ� �ָ� ���
    }

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {
        //GUIȿ���� �÷��̸� ���� �Լ�

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
        //�ߺ� ����� ����
        //���ο� ����õ��� ���͵� ���� �����Ŭ���� ������ �� ������� ��
        //�׷��ٰ� ���Ѵ�� �ߺ� �÷��̸� ��Ű���� ����. �������� ���۸��� ������
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
            m_AudioSrc.mute = a_MuteOnOff;  //��Ʈ�ÿ� ���� �����ġ �Ͻ�����

            //if (a_MuteOnOff == false)       //��Ʈ�ÿ� ���� �����ġ ó������
            //    m_AudioSrc.time = 0.0f;
        }

        for (int i = 0; i < m_EffSdCount; i++) 
        { 
            if (m_SdSrcList[i] != null)
            {
                m_SdSrcList[i].mute = a_MuteOnOff;
                if (a_MuteOnOff == false)   //ȿ������ ó������ �ٽ� �÷���
                    m_SdSrcList[i].time = 0;
            }
        }

        m_SoundOnOff = a_OnOff;
    }

    public void SoundVolume(float fVolume)
    {
        if (m_AudioSrc != null)
            m_AudioSrc.volume = m_bgmVolume * fVolume;  //�������� * ��ü����

        //for (int i = 0; i < m_EffSdCount; i++)
        //{
        //    if (m_SdSrcList[i] != null)
        //        m_SdSrcList[i].volume = 1.0f;
        //}

        m_SoundVolume = fVolume;    //������ �Ҹ��� �÷��̵� �� �����Ŀ� �ִ� �� ����
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
