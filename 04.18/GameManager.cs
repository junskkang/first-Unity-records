using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Turn
{
    player,
    Computer,
    Gameover
}

public class MyBingo
{
    public int myNum;
    public MyBingo(int a_Num)
    {
        myNum = a_Num;
    }
    public string PrintNum()
    {
        return myNum.ToString();
    }
}

public class ComBingo
{
    public int comNum;
    public ComBingo(int a_Num)
    {
        comNum = a_Num;
    }
    public string PrintNum()
    {
        return comNum.ToString();
    }
}

public class GameManager : MonoBehaviour
{
    [Header("User Bingo")]
    public Button[] MyBingoArr;
    public Text[] MyBingoText;
    public GameObject[] myLine;
    int MyBingoCount = 0;
    List<MyBingo> myBingo = new List<MyBingo>();


    [Header("Com Bingo")]
    public Button[] ComBingoArr;
    public Text[] ComBingoText;
    public GameObject[] ComLine;
    int ComBingoCount = 0;
    List<ComBingo> comBingo = new List<ComBingo>();


    public InputField NumInput;
    public Button NumInputBtn;
    public Text Timer;

    Turn state;

    float m_Timer = 0.0f;

    void Start()
    {
        Time.timeScale = 1.0f;

        BingoNumCreate();

        //�� ���� ���ϱ�
        int a_turn = Random.Range(0, 2);
        if (a_turn == 0)
        {
            state = Turn.player;
            m_Timer = 2.0f;
            Debug.Log("���� ��");
        }
        else
        {
            state = Turn.Computer;
            m_Timer = 3.0f;
            Debug.Log("��ǻ�� ��");
            NumInputBtn.interactable = false;
        }

        if (NumInputBtn != null)
            NumInputBtn.onClick.AddListener(()=>
            {
                if (NumInput.text.Trim() != "")
                    ButtonInput();
            });
    }


    void Update()
    {
        m_Timer -= Time.deltaTime;
        if (m_Timer <= 0.0f)
            m_Timer = 0.0f;

        if (Timer != null)
        {
            Timer.text = m_Timer.ToString("N2");
        }

        if (state == Turn.player && m_Timer > 0.0f) //�� �Ͽ� �Է� ����
        {
            NumInputBtn.interactable = true;
        }
        else if (state == Turn.player && m_Timer <= 0.0f)   //15�� �ʰ��� ���� �ڵ�����
        {
            AutoDuplicationCheck();
        }

        //��ǻ�� �Ͽ� ���� �ڵ� ����
        if (state == Turn.Computer && m_Timer <= 0.0f)
        {
            ComDuplicationCheck();
        }

        if (Input.GetKeyDown(KeyCode.Return) && state == Turn.player && m_Timer > 0.0f)
        {
            if(NumInput.text.Trim() == "")
                return;

            ButtonInput();
        }
    }

    void ButtonInput()
    {
        NumCheck(NumInput.text);
        m_Timer = 3.0f;
        state = Turn.Computer;
        NumInput.text = "";
        NumInput.ActivateInputField();
        NumInputBtn.interactable = false;
    }

    void AutoDuplicationCheck()
    {
        int a_AutoChoiceNum = Random.Range(0, myBingo.Count);
        for (int i = 0; i < myBingo.Count; i++)
        {

            int.TryParse(ComBingoText[a_AutoChoiceNum].text, out int checkNum);
            if (myBingo[i].myNum == checkNum)
            {
                string a_Comchoice = MyBingoText[a_AutoChoiceNum].text.ToString();
                NumCheck(a_Comchoice);
                m_Timer = 3.0f;
                state = Turn.Computer;
                NumInputBtn.interactable = false;
                return;
            }
            else
            {
                continue;
            }
        }
        AutoDuplicationCheck();
    }
    void ComDuplicationCheck()
    {
        int a_ComChoiceNum = Random.Range(0, comBingo.Count);
        for (int i = 0; i < comBingo.Count; i++)
        {
            int.TryParse(ComBingoText[a_ComChoiceNum].text, out int checkNum);
            if (comBingo[i].comNum == checkNum)
            {
                string a_Comchoice = ComBingoText[a_ComChoiceNum].text.ToString();
                NumCheck(a_Comchoice);
                m_Timer = 2.0f;
                state = Turn.player;
                NumInputBtn.interactable = true;
                return;
            }
            else
            {
                continue;
            }
        }
        ComDuplicationCheck();
    }

    void BingoNumCreate()
    {
        //�� ������ ��ȣ ����
        for (int i = 0; i < MyBingoArr.Length; i++)     //����ĭ 25����ŭ �ݺ�
        {
            int shuffle = Random.Range(1, 51);
            for (int ii = 0; ii < myBingo.Count; ii++)  //
            {
                if (myBingo[ii].myNum == shuffle)
                {
                    //Debug.Log("�ߺ����� : " + shuffle);
                    shuffle = Random.Range(1, 51);
                    ii = -1;
                }
                else
                {
                    continue;
                }
            }
            MyBingo a_Num = new MyBingo(shuffle);
            myBingo.Add(a_Num);  
        }
        //Debug.Log(myBingo.Count);

        for (int i = 0; i < myBingo.Count; i++)
        {
            if (MyBingoText != null)
            {
                //Debug.Log(myBingo[i].myNum);
                MyBingoText[i].text = myBingo[i].myNum.ToString();
                MyBingoText[i].fontSize = 20;
            }
        }

        //��ǻ�� ������ ��ȣ ����
        for (int i = 0; i < ComBingoArr.Length; i++)
        {
            int shuffle = Random.Range(1, 51);
            for (int ii = 0; ii < comBingo.Count; ii++)
            {
                if (comBingo[ii].comNum == shuffle)
                {
                    //Debug.Log("�ߺ����� : " + shuffle);
                    shuffle = Random.Range(1, 51);
                    ii = -1;
                }
                else
                {
                    continue;
                }
            }
            ComBingo a_Num = new ComBingo(shuffle);
            comBingo.Add(a_Num);
        }
        //Debug.Log(comBingo.Count);

        for (int i = 0; i < comBingo.Count; i++)
        {
            if (ComBingoText != null)
            {
                //Debug.Log(comBingo[i].comNum);
                ComBingoText[i].text = comBingo[i].comNum.ToString();
                ComBingoText[i].fontSize = 20;
            }
        }
    }

    void NumCheck(string choiceNumSt)
    {
        choiceNumSt.Trim();
        int.TryParse(choiceNumSt, out int choiceNum);

        if (choiceNum > 50)
        {
            Debug.Log("1���� 50���� ���ڸ� �Է����ּ���.");
            return;
        }

        //�� �����ǿ��� ��ȣüũ
        for(int i  = 0; i < myBingo.Count; i++)
        {
            if (myBingo[i].myNum == choiceNum)
            {
                //myBingo.RemoveAt(i);
                MyBingoArr[i].interactable = false;
                myBingo[i].myNum = 0;
                //Debug.Log($"{choiceNum}�� �Է�, {i}�� ��ư�� �������ϴ�.");
                break;
            }
        }

        //��ǻ�� �����ǿ��� ��ȣüũ
        for (int i = 0; i < comBingo.Count; i++)
        {
            if (comBingo[i].comNum == choiceNum)
            {
                //myBingo.RemoveAt(i);
                ComBingoArr[i].interactable = false;
                comBingo[i].comNum = 0;
                //Debug.Log($"{choiceNum}�� �Է�, {i}�� ��ư�� �������ϴ�.");
                break;
            }
        }

        MyBingoCheck();
        ComBingoCheck();
        Win();
    }
    void ComBingoCheck()
    {
        //������
        if (ComBingoArr[0].interactable == false && ComBingoArr[1].interactable == false
            && ComBingoArr[2].interactable == false && ComBingoArr[3].interactable == false
            && ComBingoArr[4].interactable == false && ComLine[0].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            ComLine[0].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 1���� ����!");
        }

        if (ComBingoArr[5].interactable == false && ComBingoArr[6].interactable == false
            && ComBingoArr[7].interactable == false && ComBingoArr[8].interactable == false
            && ComBingoArr[9].interactable == false && ComLine[1].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            ComLine[1].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }

        if (ComBingoArr[10].interactable == false && ComBingoArr[11].interactable == false
            && ComBingoArr[12].interactable == false && ComBingoArr[13].interactable == false
            && ComBingoArr[14].interactable == false && ComLine[2].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            ComLine[2].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }

        if (ComBingoArr[15].interactable == false && ComBingoArr[16].interactable == false
            && ComBingoArr[17].interactable == false && ComBingoArr[18].interactable == false
            && ComBingoArr[19].interactable == false && ComLine[3].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            ComLine[3].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }

        if (ComBingoArr[20].interactable == false && ComBingoArr[21].interactable == false
            && ComBingoArr[22].interactable == false && ComBingoArr[23].interactable == false
            && ComBingoArr[24].interactable == false && ComLine[4].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            ComLine[4].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //������
        if (ComBingoArr[0].interactable == false && ComBingoArr[5].interactable == false
            && ComBingoArr[10].interactable == false && ComBingoArr[15].interactable == false
            && ComBingoArr[20].interactable == false && ComLine[5].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            ComLine[5].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 1���� ����!");
        }

        if (ComBingoArr[1].interactable == false && ComBingoArr[6].interactable == false
            && ComBingoArr[11].interactable == false && ComBingoArr[16].interactable == false
            && ComBingoArr[21].interactable == false && ComLine[6].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            ComLine[6].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }

        if (ComBingoArr[2].interactable == false && ComBingoArr[7].interactable == false
            && ComBingoArr[12].interactable == false && ComBingoArr[17].interactable == false
            && ComBingoArr[22].interactable == false && ComLine[7].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            ComLine[7].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }
        if (ComBingoArr[3].interactable == false && ComBingoArr[8].interactable == false
            && ComBingoArr[13].interactable == false && ComBingoArr[18].interactable == false
            && ComBingoArr[23].interactable == false && ComLine[8].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            ComLine[8].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }
        if (ComBingoArr[4].interactable == false && ComBingoArr[9].interactable == false
            && ComBingoArr[14].interactable == false && ComBingoArr[19].interactable == false
            && ComBingoArr[24].interactable == false && ComLine[9].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            ComLine[9].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //�밢����
        if (ComBingoArr[0].interactable == false && ComBingoArr[6].interactable == false
           && ComBingoArr[12].interactable == false && ComBingoArr[18].interactable == false
           && ComBingoArr[24].interactable == false && ComLine[10].activeSelf == false)
        {
            //�밢��1���� ���� SetActive(true);
            ComLine[10].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("�밢�� 1���� ����!");
        }
        if (ComBingoArr[4].interactable == false && ComBingoArr[8].interactable == false
            && ComBingoArr[12].interactable == false && ComBingoArr[16].interactable == false
            && ComBingoArr[20].interactable == false && ComLine[11].activeSelf == false)
        {
            //�밢��2���� ���� SetActive(true);
            ComLine[11].gameObject.SetActive(true);
            ComBingoCount += 1;
            Debug.Log("�밢�� 2���� ����!");
        }
    }
    void MyBingoCheck()
    {
        //������
        if (MyBingoArr[0].interactable == false && MyBingoArr[1].interactable == false
            && MyBingoArr[2].interactable == false && MyBingoArr[3].interactable == false
            && MyBingoArr[4].interactable == false && myLine[0].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            myLine[0].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 1���� ����!");
        }

        if (MyBingoArr[5].interactable == false && MyBingoArr[6].interactable == false
            && MyBingoArr[7].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[9].interactable == false && myLine[1].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            myLine[1].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }
        
        if (MyBingoArr[10].interactable == false && MyBingoArr[11].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[13].interactable == false
            && MyBingoArr[14].interactable == false && myLine[2].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            myLine[2].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }
        
        if (MyBingoArr[15].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[17].interactable == false && MyBingoArr[18].interactable == false
            && MyBingoArr[19].interactable == false && myLine[3].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            myLine[3].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }
        
        if (MyBingoArr[20].interactable == false && MyBingoArr[21].interactable == false
            && MyBingoArr[22].interactable == false &&     MyBingoArr[23].interactable == false
            && MyBingoArr[24].interactable == false && myLine[4].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            myLine[4].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //������
        if (MyBingoArr[0].interactable == false && MyBingoArr[5].interactable == false
            && MyBingoArr[10].interactable == false && MyBingoArr[15].interactable == false
            && MyBingoArr[20].interactable == false && myLine[5].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            myLine[5].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 1���� ����!");
        }
       
        if (MyBingoArr[1].interactable == false && MyBingoArr[6].interactable == false
            && MyBingoArr[11].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[21].interactable == false && myLine[6].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            myLine[6].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }
        
        if (MyBingoArr[2].interactable == false && MyBingoArr[7].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[17].interactable == false
            && MyBingoArr[22].interactable == false && myLine[7].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            myLine[7].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }
        if (MyBingoArr[3].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[13].interactable == false && MyBingoArr[18].interactable == false
            && MyBingoArr[23].interactable == false && myLine[8].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            myLine[8].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }
        if (MyBingoArr[4].interactable == false && MyBingoArr[9].interactable == false
            && MyBingoArr[14].interactable == false && MyBingoArr[19].interactable == false
            && MyBingoArr[24].interactable == false && myLine[9].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            myLine[9].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //�밢����
        if (MyBingoArr[0].interactable == false && MyBingoArr[6].interactable == false
           && MyBingoArr[12].interactable == false && MyBingoArr[18].interactable == false
           && MyBingoArr[24].interactable == false && myLine[10].activeSelf == false)
        {
            //�밢��1���� ���� SetActive(true);
            myLine[10].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("�밢�� 1���� ����!");
        }
        if (MyBingoArr[4].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[20].interactable == false && myLine[11].activeSelf == false)
        {
            //�밢��2���� ���� SetActive(true);
            myLine[11].gameObject.SetActive(true);
            MyBingoCount += 1;
            Debug.Log("�밢�� 2���� ����!");
        }
    }

    void Win()
    {
        if (MyBingoCount == 3 || ComBingoCount == 3)
        {
            if (MyBingoCount == 3)
            {
                //�÷��̾� �¸� UI
                Debug.Log("�÷��̾� �¸�");
            }
            else
            {
                //��ǻ�� �¸� UI
                Debug.Log("��ǻ�� �¸�");
            }

            state = Turn.Gameover;
            Time.timeScale = 0.0f;
        }
    }
}
