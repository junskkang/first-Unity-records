using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MyRoomManager : MonoBehaviour
{
    public Button InvenBtn;
    public Image InvenPanel;
    public Button ShopBtn;
    public Image ShopPanel;
    public Button ToLobbyBtn;
    public Button ClearBtn;
    public Text GameGoldText;

    //내 인벤토리
    [Header("My Inventory")]
    public Button CloseInvenBtn;
    public Button CharacterBtn;
    public Sprite[] ChaBtnSprite;
    bool CharacterBtnOnOff = true;
    public Button JunkBtn;
    public ScrollRect CharacterSV;
    public ScrollRect JunkSV;

    //상점 
    [Header("Shop")]
    public Button CloseShopBtn;
    public Button CharacterBuyBtn;
    public Sprite[] CBBtnSprite;
    bool CBBtnOnOff = true;
    public Button ByeoriBuyBtn;
    public ScrollRect CBSV;
    public ScrollRect ByeoriBuySV;
    
    void Start()
    {
        GlobalData.LoadGold();

        if (InvenBtn != null)
            InvenBtn.onClick.AddListener(InvenBtnClick);

        if (ShopBtn != null)
            ShopBtn.onClick.AddListener(ShopBtnClick);

        if (ToLobbyBtn != null)
            ToLobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
            });

        if (ClearBtn != null)
            ClearBtn.onClick.AddListener(() =>
            {
                PlayerPrefs.DeleteAll();
                GlobalData.LoadGold();

                if (GameGoldText != null)
                    GameGoldText.text = GlobalData.gameGold.ToString("N0");
            });

        if (CloseInvenBtn != null)
            CloseInvenBtn.onClick.AddListener(() =>
            {
                InvenPanel.gameObject.SetActive(false);
                InvenBtn.interactable = true;
            });

        if (CloseShopBtn != null)
            CloseShopBtn.onClick.AddListener(() =>
            {
                ShopPanel.gameObject.SetActive(false);
                ShopBtn.interactable = true;
            });

        if (CharacterBtn != null)
            CharacterBtn.onClick.AddListener(() =>
            {
                CharacterBtnOnOff = !CharacterBtnOnOff;                
            });

        if (JunkBtn != null)
            JunkBtn.onClick.AddListener(() =>
            {
                CharacterBtnOnOff = !CharacterBtnOnOff;
            });

        if (CharacterBuyBtn != null)
            CharacterBuyBtn.onClick.AddListener(() =>
            {
                CBBtnOnOff = !CBBtnOnOff;
            });

        if (ByeoriBuyBtn != null)
            ByeoriBuyBtn.onClick.AddListener(() =>
            {
                CBBtnOnOff = !CBBtnOnOff;
            });

        if (GameGoldText != null)
            GameGoldText.text = GlobalData.gameGold.ToString("N0");
    }

    
    void Update()
    {
        CharacterBtnClick();
        CharBuyBtnClick();
    }

    void InvenBtnClick()
    {
        if (InvenPanel != null)
            InvenPanel.gameObject.SetActive(true);

        if (ShopPanel != null)
            ShopPanel.gameObject.SetActive(false);

        InvenBtn.interactable = false;
        ShopBtn.interactable = true;
    }

    void ShopBtnClick()
    {
        if (ShopPanel != null)
            ShopPanel.gameObject.SetActive(true);

        if (InvenPanel != null)
            InvenPanel.gameObject.SetActive(false);

        ShopBtn.interactable = false;
        InvenBtn.interactable = true;
    }

    void CharacterBtnClick()
    {
        if (CharacterBtnOnOff == false)
        {
            CharacterBtn.GetComponent<Image>().sprite = ChaBtnSprite[0];
            CharacterBtn.interactable = true;
            CharacterSV.gameObject.SetActive(CharacterBtnOnOff);

            JunkBtn.GetComponent<Image>().sprite = ChaBtnSprite[1];
            JunkBtn.interactable = false;
            JunkSV.gameObject.SetActive(!CharacterBtnOnOff);
        }
        else
        {
          
            CharacterBtn.GetComponent<Image>().sprite = ChaBtnSprite[1];
            CharacterBtn.interactable = false;
            CharacterSV.gameObject.SetActive(CharacterBtnOnOff);

            JunkBtn.GetComponent<Image>().sprite = ChaBtnSprite[0];
            JunkBtn.interactable = true;
            JunkSV.gameObject.SetActive(!CharacterBtnOnOff);
        }
    }

    void CharBuyBtnClick()
    {
        if (CBBtnOnOff == false)
        {
            CharacterBuyBtn.GetComponent<Image>().sprite = CBBtnSprite[0];
            CharacterBuyBtn.interactable = true;
            CBSV.gameObject.SetActive(CBBtnOnOff);

            ByeoriBuyBtn.GetComponent<Image>().sprite = CBBtnSprite[1];
            ByeoriBuyBtn.interactable = false;
            ByeoriBuySV.gameObject.SetActive(!CBBtnOnOff);
        }
        else
        {

            CharacterBuyBtn.GetComponent<Image>().sprite = CBBtnSprite[1];
            CharacterBuyBtn.interactable = false;
            CBSV.gameObject.SetActive(CBBtnOnOff);

            ByeoriBuyBtn.GetComponent<Image>().sprite = CBBtnSprite[0];
            ByeoriBuyBtn.interactable = true;
            ByeoriBuySV.gameObject.SetActive(!CBBtnOnOff);
        }
    }
}
