using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ���
// if : ���ǹ�, �б⹮
// if(���ǽ�)
// {
//     ����� �ڵ�
// }
// else if(���ǽ�)
// {
//     ����� �ڵ�
// }
// else
// {
//     ����� �ڵ�
// }
public class Test_1 : MonoBehaviour
{
    void Start()
    {
        int x = 1;
        if(x == 1)
        {
            int y = 2;
            Debug.Log(x);
            Debug.Log(y);
        }
        // [���������� ����]
        // y = 11; //y�� if�� �ȿ��� ����� �����̹Ƿ� �� if�ȿ����� ��밡���� 
        // ���������̹Ƿ� �ڱ� �Ҽ� �߰�ȣ�� ��� y�� ����� ������ ����.

        x = 8;
        if (x < 5)
        {
            Debug.Log("x�� 5���� �۽��ϴ�.");
        }
        // if���� ����� �ڵ尡 �� ���̸� { }�߰�ȣ�� ������ �� �ִ�.
        else if (x < 10)
        {
            Debug.Log("x�� 5���� ũ�ų� ����");
            Debug.Log("x�� 10���� �۽��ϴ�.");
        }
        else if(x < 15)
        {
            Debug.Log("x�� 10���� ũ�ų� ����");
            Debug.Log("x�� 15���� �۽��ϴ�.");
        }
        else
        {
            Debug.Log("x�� 15���� ũ�ų� �����ϴ�");
        }


        //if���� 3���� ����
        Debug.Log("---- if���� ù��° ����");
        int xyz = 15;
        if (xyz == 4) 
        {
            Debug.Log("xyz�� 4�Դϴ�.");
        }
        if (xyz == 5) 
        {
            Debug.Log("xyz�� 5�Դϴ�.");
        }
        if (xyz == 6)
        {
            Debug.Log("xyz�� 5�Դϴ�.");
        }
        if (xyz == 5)
        {
            Debug.Log("xyz�� 5�� Ȯ���մϴ�.");
        }

        Debug.Log("---- if���� �ι�° ����");
        if (xyz == 4)
            Debug.Log("xyz�� 4�Դϴ�.");
        else if (xyz == 5)
            Debug.Log("xyz�� 5�Դϴ�.");
        else if (xyz == 6)
            Debug.Log("xyz�� 6�Դϴ�.");
        else if (xyz == 5)
            Debug.Log("xyz�� 5�� Ȯ���մϴ�.");

        Debug.Log("---- if���� ����° ����");
        if (xyz == 4)
            Debug.Log("xyz�� 4�Դϴ�.");
        else if (xyz == 5)
            Debug.Log("xyz�� 5�Դϴ�.");
        else if (xyz == 6)
            Debug.Log("xyz�� 6�Դϴ�.");
        else
            Debug.Log("xyz�� 4, 5, 6�� �ƴմϴ�.");

        //switch ~ case ��
        //switch (���ǽ�)           // ���ǽ� ����� ���� �� �ִ� �� : ������, ����, ���ڿ�
        //{
        //     case ��� :
        //          ����� �ڵ�;
        //     break;
        //
        //     case ��� :
        //          ����� �ڵ�;
        //     break;
        //
        //     default:             // if������ else�� ������ ������ �ش���.
        //          ����� �ڵ�;    // if���� elseó�� �־ ��� �������.
        //     break;
        //}

        int a_ii = 7;
        switch(a_ii % 2)
        {
            case 0:
                Debug.Log("¦���Դϴ�.");
                break;
            case 1:
                Debug.Log("Ȧ���Դϴ�.");
                break;
        }
        //switch case���� ���������� ���������� ������� ó����.

        if ((a_ii % 2) == 0)
            Debug.Log("¦���Դϴ�.");
        else if ((a_ii % 2) == 1)
            Debug.Log("Ȧ���Դϴ�.");
        //���� switch case�� ������ ����� ������ if��


        // switch ~ case ���� if ~else if������ ������ ������.

        // a_Day = '��'; --> ������ �������Դϴ�.
        // a_Day = 'ȭ'; --> ������ ȭ�����Դϴ�.
        //....
        // a_Day = '��'; --> �ش��ϴ� ������ ��Ȯ�� �Է��� �ּ���.

        char a_Day = 'ȭ';    // c#���� char���� 2byte(�ѱ� �ѱ��ڵ� ������ �� �ִ�.)

        if (a_Day == '��')
        {
            Debug.Log("������ ������ �Դϴ�.");
        }
        else if (a_Day == 'ȭ')
            Debug.Log("������ ȭ���� �Դϴ�.");
        else if (a_Day == '��')
            Debug.Log("������ ������ �Դϴ�.");
        else if (a_Day == '��')
            Debug.Log("������ ����� �Դϴ�.");
        else if (a_Day == '��')
            Debug.Log("������ �ݿ��� �Դϴ�.");
        else if (a_Day == '��')
            Debug.Log("������ ����� �Դϴ�.");
        else if (a_Day == '��')
            Debug.Log("������ �Ͽ��� �Դϴ�.");
        else
            Debug.Log("�ش��ϴ� ������ ��Ȯ�� �Է��� �ּ���.");

        switch(a_Day)
        {
            case '��':
                Debug.Log("������ ������ �Դϴ�");
                break;
            case 'ȭ':
                Debug.Log("������ ȭ���� �Դϴ�");
                break;
            case '��':
                Debug.Log("������ ������ �Դϴ�");
                break;
            case '��':
                Debug.Log("������ ����� �Դϴ�");
                break;
            case '��':
                Debug.Log("������ �ݿ��� �Դϴ�");
                break;
            case '��':
                Debug.Log("������ ����� �Դϴ�");
                break;
            case '��':
                Debug.Log("������ �Ͽ��� �Դϴ�");
                break;
            default:
                Debug.Log("�ش��ϴ� ������ ��Ȯ�� �Է��� �ּ���.");
                break;
        }//switch(a_Day)

        switch(a_Day)
        {
            case '��':
            case 'ȭ':
                Debug.Log("������ ������ �ϼ���, ȭ���� �ϼ���...");
                break;
        }

        if (a_Day == '��' || a_Day == 'ȭ')
            Debug.Log("������ ������ �ϼ���, ȭ���� �ϼ���...");

    }//void Start()

    void Update()
    {
        //����Ƽ c#���� �������� ������ ���
        if(Input.GetKeyDown(KeyCode.R) == true) 
        {
            //int a_Rand = Random.Range(1, 101);   
            // Random.Range(�ּڰ�, �ִ�);
            //1���� 100���� ������ ���ڸ� �߻�������
            // �ּڰ����� ���ų� ũ�� �ִ񰪺��ٴ� ���� ����
            //Debug.Log(a_Rand);

            float a_FRd = Random.Range(0.0f, 10.0f);
            //0.0f ~ 10.0f ���� ������ ���ڸ� �߻����� ��
            Debug.Log(a_FRd);
            //int�� �ٸ��� float������ �ִ񰪵� ������ ���Եȴ�.
            //��! 10������ ��µǰ� 10.xxxx �Ҽ����� ������ �ʴ´�.
        }
        
    }
}
