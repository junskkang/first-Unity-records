using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [HideInInspector] public int ClickCount;
    public Text[] ChoiceNumUI;

    public Button Reset_Btn;


// Start is called before the first frame update
void Start()
    {
        if (Reset_Btn != null)
            Reset_Btn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("RouletteScene");
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNumber(int a_Num)
    {
        if (ClickCount < ChoiceNumUI.Length)
        {
            ChoiceNumUI[ClickCount].text = a_Num.ToString();
            ClickCount++;
        }
    }

    public static bool IsPointerOverUIObject() //UGUI�� UI���� ���� ��ŷ�Ǵ��� Ȯ���ϴ� �Լ�
    {
        PointerEventData a_EDCurPos = new PointerEventData(EventSystem.current);

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)

			List<RaycastResult> results = new List<RaycastResult>();
			for (int i = 0; i < Input.touchCount; ++i)
			{
				a_EDCurPos.position = Input.GetTouch(i).position;  
				results.Clear();
				EventSystem.current.RaycastAll(a_EDCurPos, results);
                if (0 < results.Count)
                    return true;
			}

			return false;
#else
        a_EDCurPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(a_EDCurPos, results);   //������ UI�� �ǵ帮���� �����ϴ� �ְ� EventSystem
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 
}
