using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment
{
    string[] m_Item2 = { "드래곤", "엘프", "전사", "사자", "팔라독", "고양이", "강아지", "상어", "마법사" };
    string[] m_Item3 = { "검", "활", "단검", "지팡이", "발톱", "반지", "갑옷" };
    public string m_Name;
    public int m_Level;
    public int m_Star;
    public float m_Price;
    public float m_SellingPrice;
    public int m_LvUpCount;
    public int m_StarUpCount;

    public Equipment()         //디폴트 생성자    이름입력 안하면 자동으로 아이템생성
    {
        int a_idx2 = Random.Range(0, m_Item2.Length);
        int a_idx3 = Random.Range(0, m_Item3.Length);
        m_Name = m_Item2[a_idx2] + "의" + m_Item3[a_idx3];
        m_Level = Random.Range(1, 9);
        m_Star = Random.Range(6, 9);
        m_LvUpCount = 0;
        m_StarUpCount = 0;
        m_Price = Random.Range(100, 1001);
        m_SellingPrice = m_Price;
    }

    public Equipment(string name)   //이름입력 하면 그 이름으로 아이템생성
    {
        m_Name = name;
        m_Level = Random.Range(1, 9);
        m_Star = Random.Range(6, 9);
        m_LvUpCount = 0;
        m_StarUpCount = 0;
        m_Price = Random.Range(100, 1001);
        m_SellingPrice = m_Price;
    }

    public float Success()
    {
        float a_Up = (m_Price * (1.0f + (m_LvUpCount * 0.05f) + (m_StarUpCount * 0.1f)));
        a_Up = (float)System.Math.Round(a_Up, 0);


        return a_Up;
    }
    public string Print(Equipment AAA)
    {
        return $"[{AAA.m_Name}] : 레벨({AAA.m_Level}) + 등급({AAA.m_Star}) + 가격({AAA.m_SellingPrice})";
    }

}
public class GameManager : MonoBehaviour
{
    [Header("Start Scene")]
    public GameObject StartScene;
    public InputField Nick_IF;
    public Button Start_Btn;
    string m_Nick = "";

    [Header("Iven Scene")]
    public GameObject InvenScene;
    [Header("--- User Inven ---")]
    public Text Inven_Text;
    public Text ItemList_Text;
    public Button AddSort_Btn;
    public Button LevelSort_Btn;
    public Button StarSort_Btn;
    public Button RemoveAll_Btn;
    public Button Restart_Btn;

    [Header("--- Item Add ---")]
    public Text UserGold_Text;
    public InputField ItemAdd_IF;
    public Button ItemAdd_Btn;
    public Button ItemChoice_Btn;


    [Header("--- Item Up ---")]
    public GameObject ItemUp;
    public Text ItemInfo_Text;
    public Text LvUp_Text;
    public Text StarUp_Text;
    public Text UpResult_Text;
    public Button LvUp_Btn;
    public Button StarUp_Btn;
    public Button Sell_Btn;

    List<Equipment> m_ItemList = new List<Equipment>();

    public const int m_MaxItemCount = 9;
    public static int m_UserGold = 10000;
    Equipment m_FindItem = null;

    public int LevelSort(Equipment a, Equipment b)
    {
        return a.m_Level.CompareTo(b.m_Level);
    }

    public int StarSort(Equipment a, Equipment b)
    {
        return b.m_Star.CompareTo(a.m_Star);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Start_Btn != null)
            Start_Btn.onClick.AddListener(StartBtnClick);

        if (Restart_Btn != null)
            Restart_Btn.onClick.AddListener(RestartClick);

        if (ItemAdd_Btn != null)
            ItemAdd_Btn.onClick.AddListener(ItemAddClick);

        if (AddSort_Btn != null)
            AddSort_Btn.onClick.AddListener(AddSortClick);

        if (LevelSort_Btn != null)
            LevelSort_Btn.onClick.AddListener(LevelSortClick);

        if (StarSort_Btn != null)
            StarSort_Btn.onClick.AddListener(StarSortClick);

        if (RemoveAll_Btn != null)
            RemoveAll_Btn.onClick.AddListener(() =>
            {
                m_ItemList.Clear();
                ItemList_Text.text = "";
                m_FindItem = null;
            });

        if (ItemChoice_Btn != null)
            ItemChoice_Btn.onClick.AddListener(ItemChoiceClick);

        if (LvUp_Btn != null)
            LvUp_Btn.onClick.AddListener(LvUpClick);

        if (StarUp_Btn != null)
            StarUp_Btn.onClick.AddListener(StarUpClick);

        if (Sell_Btn != null)
            Sell_Btn.onClick.AddListener(SellClick);


        //플레이어 정보 로드
        if (m_Nick != "")
        {
            m_Nick = PlayerPrefs.GetString("UserName");
            m_UserGold = PlayerPrefs.GetInt("UserGold");
            StartScene.gameObject.SetActive(false);
            InvenScene.gameObject.SetActive(true);

            Inven_Text.text = $"{m_Nick}의 인벤토리";
        }
    }



    // Update is called once per frame
    void Update()
    {
    }

    private void StartBtnClick()
    {
        if  (Nick_IF.text.Trim() == "")
            return;
        else
        {
            if (Nick_IF.text.Trim() == PlayerPrefs.GetString("UserName"))
            {
                m_UserGold = PlayerPrefs.GetInt("UserGold");
                m_Nick = PlayerPrefs.GetString("UserName");
                UserGold_Text.text = $"보유 골드 : {PlayerPrefs.GetInt("UserGold")}";
                ItemAdd_IF.text = "";
            }
            else if (Nick_IF.text.Trim() != PlayerPrefs.GetString("UserName"))
            {
                m_UserGold = 10000;
                PlayerPrefs.SetInt("UserGold", m_UserGold);
                PlayerPrefs.SetString("UserName", Nick_IF.text.Trim());
                m_Nick = PlayerPrefs.GetString("UserName", "");
                UserGold_Text.text = $"보유 골드 : {PlayerPrefs.GetInt("UserGold")}";
                ItemAdd_IF.text = "";
                ItemList_Text.text = "";
            }
        }
        //인벤토리 시작 
        StartScene.gameObject.SetActive(false);
        InvenScene.gameObject.SetActive(true);

        Inven_Text.text = $"{m_Nick}의 인벤토리";
    }

    private void ItemAddClick()
    {
        //if (ItemAdd_IF.text.Trim() == "")
        //    return;
        //else 
        if (m_ItemList.Count >= m_MaxItemCount)
            return;
        else if (m_UserGold <= 0)
        {
            m_UserGold = 0;
            return;
        }

        else
        {
            if(ItemAdd_IF.text.Trim() != "")
            { 
                Equipment MyItem = new Equipment(ItemAdd_IF.text.Trim());

                for (int ii = 0; ii < m_ItemList.Count; ii++)
                {
                    if (MyItem.m_Name == m_ItemList[ii].m_Name)
                        return;
                }

                m_ItemList.Add(MyItem);
                ItemList_Text.text += MyItem.Print(MyItem) + "\n";
                //$"[{MyItem.m_Name}] : 레벨({MyItem.m_Level}) 등급({MyItem.m_Star}) 가격({MyItem.m_Price})\n";
                m_UserGold -= 1000;
                PlayerPrefs.SetInt("UserGold", m_UserGold);
                UserGold_Text.text = $"보유 골드 : {PlayerPrefs.GetInt("UserGold")}";
            }
            else if (ItemAdd_IF.text.Trim() == "")
            {
                Equipment MyItem = new Equipment();

                for (int ii = 0; ii < m_ItemList.Count; ii++)
                {
                    if (MyItem.m_Name == m_ItemList[ii].m_Name)
                        return;
                }
                
                m_ItemList.Add(MyItem);
                ItemList_Text.text += MyItem.Print(MyItem) + "\n";
                //$"[{MyItem.m_Name}] : 레벨({MyItem.m_Level}) 등급({MyItem.m_Star}) 가격({MyItem.m_Price})\n";
                m_UserGold -= 1000;
                PlayerPrefs.SetInt("UserGold", m_UserGold);
                UserGold_Text.text = $"보유 골드 : {PlayerPrefs.GetInt("UserGold")}";

            }
        }

    }
    private void AddSortClick()
    {
        ItemList_Text.text = "";
        for (int ii = 0; ii < m_ItemList.Count; ii++)
        {
            ItemList_Text.text += $"[{m_ItemList[ii].m_Name}] : 레벨({m_ItemList[ii].m_Level}) 등급({m_ItemList[ii].m_Star}) 가격({m_ItemList[ii].m_SellingPrice})\n";
        }
    }

    private void LevelSortClick()
    {
        ItemList_Text.text = "";
        List<Equipment> a_CloneList = m_ItemList.ToList();
        a_CloneList.Sort(LevelSort);

        for (int ii = 0; ii < a_CloneList.Count; ii++)
        {
            ItemList_Text.text += $"[{a_CloneList[ii].m_Name}] : 레벨({a_CloneList[ii].m_Level}) 등급({a_CloneList[ii].m_Star}) 가격({a_CloneList[ii].m_SellingPrice})\n";
        }
    }

    private void StarSortClick()
    {
        ItemList_Text.text = "";
        List<Equipment> a_CloneList = m_ItemList.ToList();
        a_CloneList.Sort(StarSort);

        for (int ii = 0; ii < a_CloneList.Count; ii++)
        {
            ItemList_Text.text += $"[{a_CloneList[ii].m_Name}] : 레벨({a_CloneList[ii].m_Level}) 등급({a_CloneList[ii].m_Star}) 가격({a_CloneList[ii].m_SellingPrice})\n";
        }
    }

    private void ItemChoiceClick()
    {
        for (int ii = 0; ii < m_ItemList.Count; ii++)
        {
            if (m_ItemList[ii].m_Name == ItemAdd_IF.text)
            {
                m_FindItem = m_ItemList[ii];
                break;
            }
            else
            { 
                ItemUp.gameObject.SetActive(false);
                m_FindItem = null;
            }
        }

        if (m_FindItem != null)
        {
            ItemUp.gameObject.SetActive(true);
            ItemInfo_Text.text = m_FindItem.Print(m_FindItem);
            UpResult_Text.text = "강화를 시도해보세요!";
        }

        if (m_FindItem.m_Level == 30)
        {
            LvUp_Text.gameObject.SetActive(false);
        }
        else if (m_FindItem.m_Level <= 5)
        {
            LvUp_Text.text = "확률 100%";
        }
        else if (m_FindItem.m_Level <= 10)
        {
            LvUp_Text.text = "확률 95%";
        }
        else if (m_FindItem.m_Level <= 15)
        {
            LvUp_Text.text = "확률 90%";
        }
        else if (m_FindItem.m_Level <= 20)
        {
            LvUp_Text.text = "확률 80%";
        }
        else if (m_FindItem.m_Level <= 25)
        {
            LvUp_Text.text = "확률 70%";
        }
        else if (m_FindItem.m_Level < 30)
        {
            LvUp_Text.text = "확률 60%";
        }


        if (m_FindItem.m_Star >= 6)
            StarUp_Text.text = "확률 100%";
        else if (m_FindItem.m_Star == 5)
        {
            StarUp_Text.text = "확률 90%";
        }
        else if (m_FindItem.m_Star == 4)
        {
            StarUp_Text.text = "확률 80%";
        }
        else if (m_FindItem.m_Star == 3)
        {
            StarUp_Text.text = "확률 60%";
        }
        else if (m_FindItem.m_Star == 2)
        {
            StarUp_Text.text = "확률 30%";
        }

        Sell_Btn.gameObject.SetActive(true);
        StarUp_Btn.gameObject.SetActive(true);
        StarUp_Text.gameObject.SetActive(true);
        LvUp_Btn.gameObject.SetActive(true);
        LvUp_Text.gameObject.SetActive(true);
    }


    private void LvUpClick()
    {
        if (m_FindItem != null)
        {
            if (m_FindItem.m_Level == 30)
            {
                UpResult_Text.text = "최고등급입니다. 더이상 강화할 수 없습니다.";
                return;
            }
            else if (m_FindItem.m_Level <= 5)
            {
                LevelUp();
            }
            else if (m_FindItem.m_Level <= 10)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 95)
                    LevelUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Level <= 15)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 90)
                    LevelUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Level <= 20)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 80)
                    LevelUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Level <= 25)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 70)
                    StarUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Level < 30)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 60)
                    LevelUp();
                else
                    Fail();

            }
        }
    }

    private void LevelUp()
    {
        m_FindItem.m_Level++;
        m_FindItem.m_LvUpCount++;
        m_FindItem.m_SellingPrice = m_FindItem.Success();
        UpResult_Text.text = "레벨 강화에 성공하였습니다!";
        ItemInfo_Text.text = m_FindItem.Print(m_FindItem);
        if (m_FindItem.m_Level == 30)
        {
            LvUp_Text.gameObject.SetActive(false);
        }
        else if (m_FindItem.m_Level <= 5)
        {
            LvUp_Text.text = "확률 100%";
        }
        else if (m_FindItem.m_Level <= 10)
        {
            LvUp_Text.text = "확률 95%";
        }
        else if (m_FindItem.m_Level <= 15)
        {
            LvUp_Text.text = "확률 90%";
        }
        else if (m_FindItem.m_Level <= 20)
        {
            LvUp_Text.text = "확률 80%";
        }
        else if (m_FindItem.m_Level <= 25)
        {
            LvUp_Text.text = "확률 70%";
        }
        else if (m_FindItem.m_Level < 30)
        {
            LvUp_Text.text = "확률 60%";
        }

    }
    private void StarUpClick()
    {
        if (m_FindItem != null)
        {
            if (m_FindItem.m_Star == 1)
            {
                UpResult_Text.text = "최고등급입니다. 더이상 강화할 수 없습니다.";
                return;
            }
            else if (m_FindItem.m_Star >= 6)
            {
                StarUp();
            }
            else if (m_FindItem.m_Star == 5)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 90)
                    StarUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Star == 4)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 80)
                    StarUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Star == 3)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 60)
                    StarUp();
                else
                    Fail();
            }
            else if (m_FindItem.m_Star == 2)
            {
                int chance = Random.Range(1, 101);
                if (chance <= 30)
                    StarUp();
                else
                    Fail();
            }
        }
    }

    void StarUp()
    {
        m_FindItem.m_Star--;
        m_FindItem.m_StarUpCount++;
        m_FindItem.m_SellingPrice = m_FindItem.Success();
        UpResult_Text.text = "등급 강화에 성공하였습니다!";
        ItemInfo_Text.text = m_FindItem.Print(m_FindItem);
        Debug.Log(m_FindItem.m_LvUpCount + " , " + m_FindItem.m_StarUpCount + " , " + m_FindItem.m_SellingPrice);

        if (m_FindItem.m_Star == 5)
        {
            StarUp_Text.text = "확률 90%";
        }
        else if (m_FindItem.m_Star == 4)
        {
            StarUp_Text.text = "확률 80%";
        }
        else if (m_FindItem.m_Star == 3)
        {
            StarUp_Text.text = "확률 60%";
        }
        else if (m_FindItem.m_Star == 2)
        {
            StarUp_Text.text = "확률 30%";
        }
        else if (m_FindItem.m_Star == 1)
        {
            StarUp_Text.gameObject.SetActive(false);
        }
    }

    void Fail()
    {
        UpResult_Text.text = "아이쿠 실패! 아이템이 파괴되었습니다ㅎ";
        for (int ii = 0; ii < m_ItemList.Count; ii++)
        {
            if (m_ItemList[ii].m_Name == m_FindItem.m_Name)
            {
                m_ItemList.RemoveAt(ii);
                m_FindItem = null;
                AddSortClick();
                Sell_Btn.gameObject.SetActive(false);
                StarUp_Btn.gameObject.SetActive(false);
                StarUp_Text.gameObject.SetActive(false);
                LvUp_Btn.gameObject.SetActive(false);
                LvUp_Text.gameObject.SetActive(false);
            }
        }
    }

    private void SellClick()
    {
        for (int ii = 0; ii < m_ItemList.Count; ii++)
        {
            if (m_ItemList[ii].m_Name == m_FindItem.m_Name)
            {
                m_ItemList.RemoveAt(ii);
                AddSortClick();
                ItemUp.gameObject.SetActive(false);
                m_UserGold += (int)m_FindItem.m_SellingPrice;
                PlayerPrefs.SetInt("UserGold", m_UserGold);
                UserGold_Text.text = $"보유 골드 : {PlayerPrefs.GetInt("UserGold")}";
                m_FindItem = null;
            }
        }
    }

    private void RestartClick()
    {
        m_ItemList.Clear();
        PlayerPrefs.DeleteAll();
        StartScene.gameObject.SetActive(true);
        InvenScene.gameObject.SetActive(false);
        m_Nick = "";
        Nick_IF.text = "";
    }
}
