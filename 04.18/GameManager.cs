using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Turn    //�� ����
{
    player,
    Computer,
    Gameover
}

//���� ��ȣ Ŭ���� ����Ʈ�� ����
//�ٵ� ������ Ŭ������ ������ ������ �ϳ����� ���� Ŭ������ ���� �ʿ���� �־�����? �ͱ�� ��.
//�׷��� ������� ��Ծ������
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
    public Image[] myLine;
    int MyBingoCount = 0;
    List<MyBingo> myBingo = new List<MyBingo>();


    [Header("Com Bingo")]
    public Button[] ComBingoArr;
    public Text[] ComBingoText;
    public Image[] ComLine;
    int ComBingoCount = 0;
    List<ComBingo> comBingo = new List<ComBingo>();


    public InputField NumInput;
    public Button NumInputBtn;
    public Text Timer;

    Turn state;

    float m_Timer = 0.0f;

    //��ǻ�� ��ȣ���� AI�� ����
    int hor1Check, hor2Check, hor3Check, hor4Check, hor5Check = 0;
    
    int ver1Check, ver2Check, ver3Check, ver4Check, ver5Check = 0;
    



    void Start()
    {
        
        Time.timeScale = 1.0f;

        //������� ����
        BingoNumCreate();

        //�� ���� ���ϱ�
        int a_turn = Random.Range(0, 2);
        if (a_turn == 0)
        {
            state = Turn.player;
            m_Timer = 10.0f;
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
        //Ÿ�̸�
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
        else if (state == Turn.player && m_Timer <= 0.0f)   //10�� �ʰ��� ���� �ڵ�����
        {
            AutoDuplicationCheck();
        }

        //��ǻ�� �Ͽ� ���� �ڵ� ����
        if (state == Turn.Computer && m_Timer <= 0.0f)
        {
            ComNumAI();
        }

        //����Ű�� ���� �Է� �����ϰԲ�
        if (Input.GetKeyDown(KeyCode.Return) && state == Turn.player && m_Timer > 0.0f)
        {
            if(NumInput.text.Trim() == "")
                return;

            ButtonInput();
        }
    }

    //ó�� ������ �� �� ����� ��ǻ�� ���� 1~50���� �� �������� 25�� ����
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

        //������� �� ���� ���ڵ� ȭ�鿡 ǥ��
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
       
        //������� ��ǻ�� ���� ���ڵ� ȭ�鿡 ǥ��
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

    //�� ���ʿ� ���� ���� �Է� �Լ�
    void ButtonInput()
    {
        NumCheck(NumInput.text);
        m_Timer = 3.0f;
        state = Turn.Computer;
        NumInput.text = "";
        NumInput.ActivateInputField();
        NumInputBtn.interactable = false;
    }

    //���� �ð� �� ���� ���� �������� ���� �� �� ������ �� �������� ���� ����
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
        AutoDuplicationCheck(); //�̹� ���õ� ���ڰ� ������ �� �ٽ� �Լ� ����
    }
   

    //�Է��� ���� �����ǿ��� �ð��� ȿ�� ���ֱ� + ���� üũ�� ���� ��ȯ
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
                //�̹� ���Դ� ���ڸ� �ٽ� ������ ��츦 ����Ͽ� ���Դ� ���ڸ� ����Ʈ���� 0���� �����
                //�̿� ���� Ȯ���� DuplicationCheck();����
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
                if (0 <= i && i < 5)
                    hor1Check++;
                else if (5 <= i && i < 10)
                    hor2Check++;
                else if (10 <= i && i < 15)
                    hor3Check++;
                else if (15 <= i && i < 20)
                    hor4Check++;
                else if (20 <= i && i < 25)
                    hor5Check++;

                if(i%5 == 0)
                    ver1Check++;
                else if (i%5 == 1)
                    ver2Check++;
                else if (i%5 == 2)
                    ver3Check++;
                else if (i%5 == 3)
                    ver4Check++;
                else if (i%5 == 4)
                    ver5Check++;

                //Debug.Log($"{choiceNum}�� �Է�, {i}�� ��ư�� �������ϴ�.");
                break;
            }
        }

        BingoCheck(MyBingoArr, myLine, ref MyBingoCount);
        BingoCheck(ComBingoArr, ComLine, ref ComBingoCount);
        //MyBingoCheck();
        //ComBingoCheck();
        Win();
    }

    //������� üũ�ϴ� �Լ�
    void BingoCheck(Button[] a_BingArr, Image[] a_Line, ref int a_BingoCount)
    {
        //������
        if (a_BingArr[0].interactable == false && a_BingArr[1].interactable == false
            && a_BingArr[2].interactable == false && a_BingArr[3].interactable == false
            && a_BingArr[4].interactable == false && a_Line[0].gameObject.activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            a_Line[0].gameObject.SetActive(true);
            a_BingoCount ++;
            Debug.Log("���� 1���� ����!");
        }

        if (a_BingArr[5].interactable == false && a_BingArr[6].interactable == false
            && a_BingArr[7].interactable == false && a_BingArr[8].interactable == false
            && a_BingArr[9].interactable == false && a_Line[1].gameObject.activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            a_Line[1].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }

        if (a_BingArr[10].interactable == false && a_BingArr[11].interactable == false
            && a_BingArr[12].interactable == false && a_BingArr[13].interactable == false
            && a_BingArr[14].interactable == false && a_Line[2].gameObject.activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            a_Line[2].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }

        if (a_BingArr[15].interactable == false && a_BingArr[16].interactable == false
            && a_BingArr[17].interactable == false && a_BingArr[18].interactable == false
            && a_BingArr[19].interactable == false && a_Line[3].gameObject.activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            a_Line[3].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }

        if (a_BingArr[20].interactable == false && a_BingArr[21].interactable == false
            && a_BingArr[22].interactable == false && a_BingArr[23].interactable == false
            && a_BingArr[24].interactable == false && a_Line[4].gameObject.activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            a_Line[4].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //������
        if (a_BingArr[0].interactable == false && a_BingArr[5].interactable == false
            && a_BingArr[10].interactable == false && a_BingArr[15].interactable == false
            && a_BingArr[20].interactable == false && a_Line[5].gameObject.activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            a_Line[5].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 1���� ����!");
        }

        if (a_BingArr[1].interactable == false && a_BingArr[6].interactable == false
            && a_BingArr[11].interactable == false && a_BingArr[16].interactable == false
            && a_BingArr[21].interactable == false && a_Line[6].gameObject.activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            a_Line[6].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 2���� ����!");
        }

        if (a_BingArr[2].interactable == false && a_BingArr[7].interactable == false
            && a_BingArr[12].interactable == false && a_BingArr[17].interactable == false
            && a_BingArr[22].interactable == false && a_Line[7].gameObject.activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            a_Line[7].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 3���� ����!");
        }
        if (a_BingArr[3].interactable == false && a_BingArr[8].interactable == false
            && a_BingArr[13].interactable == false && a_BingArr[18].interactable == false
            && a_BingArr[23].interactable == false && a_Line[8].gameObject.activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            a_Line[8].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 4���� ����!");
        }
        if (a_BingArr[4].interactable == false && a_BingArr[9].interactable == false
            && a_BingArr[14].interactable == false && a_BingArr[19].interactable == false
            && a_BingArr[24].interactable == false && a_Line[9].gameObject.activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            a_Line[9].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("���� 5���� ����!");
        }

        //�밢����
        if (a_BingArr[0].interactable == false && a_BingArr[6].interactable == false
           && a_BingArr[12].interactable == false && a_BingArr[18].interactable == false
           && a_BingArr[24].interactable == false && a_Line[10].gameObject.activeSelf == false)
        {
            //�밢��1���� ���� SetActive(true);
            a_Line[10].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("�밢�� 1���� ����!");
        }
        if (a_BingArr[4].interactable == false && a_BingArr[8].interactable == false
            && a_BingArr[12].interactable == false && a_BingArr[16].interactable == false
            && a_BingArr[20].interactable == false && a_Line[11].gameObject.activeSelf == false)
        {
            //�밢��2���� ���� SetActive(true);
            a_Line[11].gameObject.SetActive(true);
            a_BingoCount += 1;
            Debug.Log("�밢�� 2���� ����!");
        }
    }

    //���� ���� ���� �ϼ��ϴ� ���� �¸�
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
    //��ǻ�� �� �� 3�� �� �ڵ����� ���� ����
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
                m_Timer = 10.0f;
                state = Turn.player;
                NumInputBtn.interactable = true;
                return;
            }
            else
            {
                continue;
            }
        }
        ComDuplicationCheck();  //�̹� ���õ� ���ڰ� ������ �� �ٽ� �Լ� ����
    }
    //��ǻ�� ��ư ���� AI
    void ComNumAI()
    {
        for (int i = 0; i < ComBingoArr.Length; i++)
        {
            if (ComLine[0].gameObject.activeSelf == true && i < 5)
            {
                i = 5;
            }

            if (ComLine[1].gameObject.activeSelf == true && 5 <= i && i < 10)
            {
                i = 10;
            }

            if (ComLine[2].gameObject.activeSelf == true && 10 <= i && i < 15)
            {
                i = 15;
            }

            if (ComLine[3].gameObject.activeSelf == true && 15 <= i && i < 20)
            {
                i = 20;
            }

            if (ComLine[5].gameObject.activeSelf == true && i % 5 == 0)
            {
                i++;
            }

            if (ComBingoArr[i].interactable == false)
            {

                                           
                string a_Str = "";
                int a_Ran = 0;

                if (i % 5 == 0)     //���� ù��°��
                {
                    switch (i)
                    {
                        case 0:
                            {
                                if (hor1Check > ver1Check)
                                    a_Ran = 2;
                                else if (hor1Check < ver1Check)
                                    a_Ran = 1;
                                else
                                    a_Ran = Random.Range (1, 3);
                            }
                            break;
                        case 5:
                            {
                                if (hor2Check > ver1Check)
                                    a_Ran = 2;
                                else if (hor2Check < ver1Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 3);
                            }
                            break;
                        case 10:
                            {
                                if (hor3Check > ver1Check)
                                    a_Ran = 2;
                                else if (hor3Check < ver1Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 3);
                            }
                            break;
                        case 15:
                            {
                                if (hor4Check > ver1Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor4Check < ver1Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 3);
                            }
                            break;
                        case 20:
                            {
                                if (hor5Check > ver1Check)
                                    a_Ran = 2;
                                else if (hor5Check < ver1Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 3);
                            }
                            break;
                    }
                }
                
                if (i % 5 == 1)     //���� �ι�° ��
                {
                    switch (i)
                    {
                        case 1:
                            {
                                if (hor1Check > ver2Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor1Check < ver2Check)
                                    a_Ran = 1;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                        case 6:
                            {
                                if (hor2Check > ver2Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor2Check < ver2Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 11:
                            {
                                if (hor3Check > ver2Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor3Check < ver2Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 16:
                            {
                                if (hor4Check > ver2Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor4Check < ver2Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 21:
                            {
                                if (hor5Check > ver2Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor5Check < ver2Check)
                                    a_Ran = 0;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                    }
                }
                if (i % 5 == 2)     //���� ����° ��
                {
                    switch (i)
                    {
                        case 2:
                            {
                                if (hor1Check > ver3Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor1Check < ver3Check)
                                    a_Ran = 1;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                        case 7:
                            {
                                if (hor2Check > ver3Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor2Check < ver3Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 12:
                            {
                                if (hor3Check > ver3Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor3Check < ver3Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 17:
                            {
                                if (hor4Check > ver3Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor4Check < ver3Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 22:
                            {
                                if (hor5Check > ver3Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor5Check < ver3Check)
                                    a_Ran = 0;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                    }
                }
                if (i % 5 == 3)     //���� �׹�° ��
                {
                    switch (i)
                    {
                        case 3:
                            {
                                if (hor1Check > ver4Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor1Check < ver4Check)
                                    a_Ran = 1;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                        case 8:
                            {
                                if (hor2Check > ver4Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor2Check < ver4Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 13:
                            {
                                if (hor3Check > ver4Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor3Check < ver4Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 18:
                            {
                                if (hor4Check > ver4Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor4Check < ver4Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                    a_Ran = Random.Range(0, 4);
                            }
                            break;
                        case 23:
                            {
                                if (hor5Check > ver4Check)
                                    a_Ran = Random.Range(2, 4);
                                else if (hor5Check < ver4Check)
                                    a_Ran = 0;
                                else
                                    a_Ran = Random.Range(1, 4);
                            }
                            break;
                    }
                }
                if (i % 5 == 4)     //���� �ټ���° ��
                {
                    switch (i)
                    {
                        case 4:
                            {
                                if (hor1Check > ver5Check)
                                    a_Ran = 3;
                                else if (hor1Check < ver2Check)
                                    a_Ran = 1;
                                else
                                {
                                    a_Ran = Random.Range(1, 3);
                                    if (a_Ran == 2)
                                        a_Ran = 3;
                                }                                  

                            }
                            break;
                        case 9:
                            {
                                if (hor2Check > ver5Check)
                                    a_Ran = 3;
                                else if (hor2Check < ver5Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                {
                                    a_Ran = Random.Range(0, 4);
                                    if (a_Ran == 2)
                                        a_Ran = 3;
                                }
                            }
                            break;
                        case 14:
                            {
                                if (hor3Check > ver5Check)
                                    a_Ran = 3;
                                else if (hor3Check < ver5Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                {
                                    a_Ran = Random.Range(0, 4);
                                    if (a_Ran == 2)
                                        a_Ran = 3;
                                }
                            }
                            break;
                        case 19:
                            {
                                if (hor4Check > ver5Check)
                                    a_Ran = 3;
                                else if (hor4Check < ver5Check)
                                    a_Ran = Random.Range(0, 2);
                                else
                                {
                                    a_Ran = Random.Range(0, 4);
                                    if (a_Ran == 2)
                                        a_Ran = 3;
                                }
                            }
                            break;
                        case 24:
                            {
                                if (hor5Check > ver5Check)
                                    a_Ran = 3;
                                else if (hor5Check < ver5Check)
                                    a_Ran = 0;
                                else
                                {
                                    a_Ran = Random.Range(0, 4);
                                    if (a_Ran == 2)
                                        a_Ran = 3;
                                    if (a_Ran == 1)
                                        a_Ran = 0;
                                }
                            }
                            break;
                    }
                }

                switch (a_Ran)
                {
                    case 0://���õ� ĭ�� ��ĭ
                        {
                            if (i - 5 >= 0)
                            {
                                if (ComBingoArr[i - 5].interactable == false)
                                {
                                    if (i - 10 >= 0 && ComBingoArr[i - 10].interactable == false)
                                    {
                                        if (i - 15 >= 0 && ComBingoArr[i - 15].interactable == false)
                                        {
                                            a_Str = ComBingoText[i - 20].text.ToString();
                                            NumCheck(a_Str);
                                            m_Timer = 10.0f;
                                            state = Turn.player;
                                            NumInputBtn.interactable = true;
                                            return;
                                        }
                                        a_Str = ComBingoText[i - 15].text.ToString();
                                        NumCheck(a_Str);
                                        m_Timer = 10.0f;
                                        state = Turn.player;
                                        NumInputBtn.interactable = true;
                                        return;
                                    }
                                    a_Str = ComBingoText[i - 10].text.ToString();
                                    NumCheck(a_Str);
                                    m_Timer = 10.0f;
                                    state = Turn.player;
                                    NumInputBtn.interactable = true;
                                    return;
                                }
                                a_Str = ComBingoText[i - 5].text.ToString();
                                NumCheck(a_Str);
                                m_Timer = 10.0f;
                                state = Turn.player;
                                NumInputBtn.interactable = true;
                                return;
                            }
                            //ComNumAI();
                        }
                        break;
                    case 1: //���õ� ĭ�� �Ʒ�ĭ
                        {
                            if (i + 5 <= 24)
                            {
                                if (ComBingoArr[i + 5].interactable == false)
                                {
                                    if (i + 10 <= 24 && ComBingoArr[i + 10].interactable == false)
                                    {
                                        if (i + 15 <= 24 && ComBingoArr[i + 15].interactable == false)
                                        {
                                            a_Str = ComBingoText[i + 20].text.ToString();
                                            NumCheck(a_Str);
                                            m_Timer = 10.0f;
                                            state = Turn.player;
                                            NumInputBtn.interactable = true;
                                            return;
                                        }
                                        a_Str = ComBingoText[i + 15].text.ToString();
                                        NumCheck(a_Str);
                                        m_Timer = 10.0f;
                                        state = Turn.player;
                                        NumInputBtn.interactable = true;
                                        return;
                                    }
                                    a_Str = ComBingoText[i + 10].text.ToString();
                                    NumCheck(a_Str);
                                    m_Timer = 10.0f;
                                    state = Turn.player;
                                    NumInputBtn.interactable = true;
                                    return;
                                }
                                a_Str = ComBingoText[i + 5].text.ToString();
                                NumCheck(a_Str);
                                m_Timer = 10.0f;
                                state = Turn.player;
                                NumInputBtn.interactable = true;
                                return;
                            }
                            //ComNumAI();
                        }
                        break;
                    case 2: //���õ� ĭ�� ������
                        {
                            if (i + 1 < 25 && (i)%5 != 4)
                            {
                                if ((ComBingoArr[i + 1].interactable) == false)
                                {
                                    if((i + 1)%5 != 4 && ComBingoArr[i + 2].interactable == false)
                                    {
                                        if ((i + 2) % 5 != 4 && ComBingoArr[i + 3].interactable == false)
                                        {
                                            a_Str = ComBingoText[i + 4].text.ToString();
                                            NumCheck(a_Str);
                                            m_Timer = 10.0f;
                                            state = Turn.player;
                                            NumInputBtn.interactable = true;
                                            return;
                                        }
                                        a_Str = ComBingoText[i + 3].text.ToString();
                                        NumCheck(a_Str);
                                        m_Timer = 10.0f;
                                        state = Turn.player;
                                        NumInputBtn.interactable = true;
                                        return;
                                    }
                                    a_Str = ComBingoText[i + 2].text.ToString();
                                    NumCheck(a_Str);
                                    m_Timer = 10.0f;
                                    state = Turn.player;
                                    NumInputBtn.interactable = true;
                                    return;
                                }

                                a_Str = ComBingoText[i+1].text.ToString(); 
                                NumCheck(a_Str);
                                m_Timer = 10.0f;
                                state = Turn.player;
                                NumInputBtn.interactable = true;
                                return;
                            }
                            //ComNumAI();
                        }
                        break;
                    case 3: //���õ� ĭ�� ����
                        {
                            if (i - 1 >= 0 &&(i % 5 != 0))
                            {
                                if ((ComBingoArr[i - 1].interactable) == false)
                                {
                                    if ((i - 1) % 5 != 0 && ComBingoArr[i - 2].interactable == false)
                                    {
                                        if ((i - 2) % 5 != 0 && ComBingoArr[i - 3].interactable == false)
                                        {
                                            a_Str = ComBingoText[i - 4].text.ToString();
                                            NumCheck(a_Str);
                                            m_Timer = 10.0f;
                                            state = Turn.player;
                                            NumInputBtn.interactable = true;
                                            return;
                                        }
                                        a_Str = ComBingoText[i - 3].text.ToString();
                                        NumCheck(a_Str);
                                        m_Timer = 10.0f;
                                        state = Turn.player;
                                        NumInputBtn.interactable = true;
                                        return;
                                    }
                                    a_Str = ComBingoText[i - 2].text.ToString();
                                    NumCheck(a_Str);
                                    m_Timer = 10.0f;
                                    state = Turn.player;
                                    NumInputBtn.interactable = true;
                                    return;
                                }

                                a_Str = ComBingoText[i - 1].text.ToString();
                                NumCheck(a_Str);
                                m_Timer = 10.0f;
                                state = Turn.player;
                                NumInputBtn.interactable = true;
                                return;
                            }
                            //ComNumAI();
                        }
                        break;
                }
            }
        }
        Debug.Log("���� ���ڰ� ����? ���� ���� ����");
        //������ ���� ���� �ϳ��� ������
        ComDuplicationCheck();
    }
}
