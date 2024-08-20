using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    public Button confirmBtn;
    public Button closeBtn;
    public InputField nickInput;

    float showMsTimer = 0;
    public Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        if (confirmBtn != null)
            confirmBtn.onClick.AddListener(ConfirmClick);

        if (closeBtn != null)
            closeBtn.onClick.AddListener(() =>
            {
                Destroy(gameObject);
                Time.timeScale = 1.0f;
            });

        if (nickInput != null)
        {
            nickInput.text = GlobalValue.g_NickName;
        }

        //���� �Է��� ���� NickInputField�� ������ ���� ������ ��� �����
        //�ٽ� �Է¹��� �� �ֵ��� �غ��Ű�� �ڵ�
        if (nickInput != null)
        {
            EventTrigger trigger = nickInput.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((data) =>
            {
                nickInput.text = "";
            });
            trigger.triggers.Add(entry);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (showMsTimer > 0)
        {
            showMsTimer -= Time.unscaledDeltaTime;
            if(showMsTimer <= 0) 
            {
                MessageOnOff("", false);       
            }
        }
    }


    void ConfirmClick()
    {
        string strNick = nickInput.text.Trim();
        if (string.IsNullOrEmpty(strNick))
        {
            MessageOnOff("������ �� ĭ ���� �Է��� �ּ���.");
            return;
        }

        if (!(3 <= strNick.Length && strNick.Length <= 20))
        {
            MessageOnOff("������ 3���� �̻� 20���� ���Ϸ� �Է��� �ּ���.");
            return;
        }

        LobbyNetworkMgr.Inst.m_NickStrBuff = strNick;
        LobbyNetworkMgr.Inst.m_RefCfgBox = this;
        LobbyNetworkMgr.Inst.PushPacket(PacketType.NickUpdate);
    }

    public void ResultOkBtn(bool isWait, string str)
    {
        if (isWait) //�г��� ���� ����
        {
            MessageOnOff(str);
        }
        else   //�г��� ���� ����
        {
            Time.timeScale = 1.0f;
            Destroy(this);
        }

    }

    void MessageOnOff(string msg = "", bool isOn = true)
    {
        if (isOn)
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(true);
            showMsTimer = 3.0f;
        }
        else
        {
            messageText.text = msg;
            messageText.gameObject.SetActive(false);
        }
    }
}
