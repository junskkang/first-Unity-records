using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Button[] MyBingoArr;
    public Text[] MyBingoText;
    public Button[] ComBingoArr;
    List<MyBingo> myBingo = new List<MyBingo>();
    List<ComBingo> comBingo = new List<ComBingo>();

    public InputField NumInput;
    public Button NumInputBtn;

    public GameObject[] line;

    int BingoCount = 0;

    void Start()
    {
        //MyBingoArr = new Button[25];
        //ComBingoArr = new Button[25];
        //line = new GameObject[12];
        


        BingoNumCreate();

        if (NumInputBtn != null)
            NumInputBtn.onClick.AddListener(() =>
            NumCheck(NumInput.text));

    }


    void Update()
    {
        
    }

    void BingoNumCreate()
    {
        //�� ������ ��ȣ ����
        for (int i = 0; i < MyBingoArr.Length; i++)
        {
            int shuffle = Random.Range(1, 51);
            for (int ii = 0; ii < myBingo.Count; ii++)
            {
                if (myBingo[ii].myNum == shuffle)
                {
                    Debug.Log("�ߺ����� : " + shuffle);
                    shuffle = Random.Range(1, 51);
                    ii = 0;
                }
                else
                {
                    break;
                }
            }
            MyBingo a_Num = new MyBingo(shuffle);
            myBingo.Add(a_Num);
        }
        Debug.Log(myBingo.Count);

        //��ǻ�� ������ ��ȣ ����
        for (int i = 0; i < ComBingoArr.Length; i++)
        {
            int shuffle = Random.Range(1, 51);
            for (int ii = 0; ii < comBingo.Count; ii++)
            {
                if (comBingo[ii].comNum == shuffle)
                {
                    Debug.Log("�ߺ����� : " + shuffle);
                    shuffle = Random.Range(1, 51);
                    ii = 0;
                }
                else
                {
                    break;
                }
            }
            ComBingo a_Num = new ComBingo(shuffle);
            comBingo.Add(a_Num);
        }
        Debug.Log(comBingo.Count);
    }

    void NumCheck(string choiceNumSt)
    {
        choiceNumSt.Trim();
        int.TryParse(choiceNumSt, out int choiceNum);

        //�� �����ǿ��� ��ȣüũ
        for(int i  = 0; i < myBingo.Count; i++)
        {
            if (myBingo[i].myNum == choiceNum)
            {
                //myBingo.RemoveAt(i);
                MyBingoArr[i].interactable = false;
                Debug.Log($"{choiceNum}�� �Է�, {i}�� ��ư�� �������ϴ�.");
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
                Debug.Log($"{choiceNum}�� �Է�, {i}�� ��ư�� �������ϴ�.");
                break;
            }
        }

        BingoCheck();
    }

    void BingoCheck()
    {
        //������
        if (MyBingoArr[0].interactable == false && MyBingoArr[1].interactable == false
            && MyBingoArr[2].interactable == false && MyBingoArr[3].interactable == false
            && MyBingoArr[4].interactable == false && line[0].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            line[0].gameObject.SetActive(true);
            Debug.Log("���� 1���� ����!");
        }

        if (MyBingoArr[5].interactable == false && MyBingoArr[6].interactable == false
            && MyBingoArr[7].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[9].interactable == false && line[1].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            line[1].gameObject.SetActive(true);
            Debug.Log("���� 2���� ����!");
        }
        
        if (MyBingoArr[10].interactable == false && MyBingoArr[11].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[13].interactable == false
            && MyBingoArr[14].interactable == false && line[2].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            line[2].gameObject.SetActive(true);
            Debug.Log("���� 3���� ����!");
        }
        
        if (MyBingoArr[15].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[17].interactable == false && MyBingoArr[18].interactable == false
            && MyBingoArr[19].interactable == false && line[3].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            line[3].gameObject.SetActive(true);
            Debug.Log("���� 4���� ����!");
        }
        
        if (MyBingoArr[20].interactable == false && MyBingoArr[21].interactable == false
            && MyBingoArr[22].interactable == false && MyBingoArr[23].interactable == false
            && MyBingoArr[24].interactable == false && line[4].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            line[4].gameObject.SetActive(true);
            Debug.Log("���� 5���� ����!");
        }

        //������
        if (MyBingoArr[0].interactable == false && MyBingoArr[5].interactable == false
            && MyBingoArr[10].interactable == false && MyBingoArr[15].interactable == false
            && MyBingoArr[20].interactable == false && line[5].activeSelf == false)
        {
            //����1���� ���� SetActive(true);
            line[5].gameObject.SetActive(true);
            Debug.Log("���� 1���� ����!");
        }
       
        if (MyBingoArr[1].interactable == false && MyBingoArr[6].interactable == false
            && MyBingoArr[11].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[21].interactable == false && line[6].activeSelf == false)
        {
            //����2���� ���� SetActive(true);
            line[6].gameObject.SetActive(true);
            Debug.Log("���� 2���� ����!");
        }
        
        if (MyBingoArr[2].interactable == false && MyBingoArr[7].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[17].interactable == false
            && MyBingoArr[22].interactable == false && line[7].activeSelf == false)
        {
            //����3���� ���� SetActive(true);
            line[7].gameObject.SetActive(true);
            Debug.Log("���� 3���� ����!");
        }
        if (MyBingoArr[3].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[13].interactable == false && MyBingoArr[18].interactable == false
            && MyBingoArr[23].interactable == false && line[8].activeSelf == false)
        {
            //����4���� ���� SetActive(true);
            line[8].gameObject.SetActive(true);
            Debug.Log("���� 4���� ����!");
        }
        if (MyBingoArr[4].interactable == false && MyBingoArr[9].interactable == false
            && MyBingoArr[14].interactable == false && MyBingoArr[19].interactable == false
            && MyBingoArr[24].interactable == false && line[9].activeSelf == false)
        {
            //����5���� ���� SetActive(true);
            line[9].gameObject.SetActive(true);
            Debug.Log("���� 5���� ����!");
        }

        //�밢����
        if (MyBingoArr[0].interactable == false && MyBingoArr[6].interactable == false
           && MyBingoArr[12].interactable == false && MyBingoArr[18].interactable == false
           && MyBingoArr[24].interactable == false && line[10].activeSelf == false)
        {
            //�밢��1���� ���� SetActive(true);
            line[10].gameObject.SetActive(true);
            Debug.Log("�밢�� 1���� ����!");
        }
        if (MyBingoArr[4].interactable == false && MyBingoArr[8].interactable == false
            && MyBingoArr[12].interactable == false && MyBingoArr[16].interactable == false
            && MyBingoArr[20].interactable == false && line[11].activeSelf == false)
        { 
            //�밢��2���� ���� SetActive(true);
            line[11].gameObject.SetActive(true);
            Debug.Log("�밢�� 2���� ����!");
        }

        Win();
    }

    void Win()
    {
        for(int i = 0;i< line.Length;i++)
        {
            if (line[i].activeSelf == true && line[i] != null)
            {  
                BingoCount++;
                line[i] = null;
            }
        }

        Debug.Log(BingoCount);
    }
}
