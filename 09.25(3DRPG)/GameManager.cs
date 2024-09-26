using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum JoyStickType
{
    Fixed = 0,
    Flexible = 1,
    FlexibleOnOff = 2

}
public class GameManager : MonoBehaviour
{
    GameObject refHero;

    //조이스틱 UI관련 변수
    JoyStickType typeJS = JoyStickType.Fixed;
    //Fixed 조이스틱
    public GameObject joystickBackObj;
    public Image joystickImg;
    Color basicAlpha = Color.white;
    float radius = 0.0f;
    Vector3 originPos = Vector3.zero;
    Vector3 axis = Vector3.zero;
    Vector3 joystickCacVect = Vector3.zero;
    float joystickCacDist = 0.0f;

    //Flexible 조이스틱
    public GameObject m_JoystickPickPanel = null;
    Vector3 posJoyBack;
    Vector3 dirStick;

    //공격 관련 UI
    public Button attackBtn;

    public static GameManager inst;

    public void InitManager(GameObject a_Player)     //따라다닐 캐릭터 받아오는 함수
    {
        refHero = a_Player;
    }
    void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //고정된 조이스틱 관련
        if (joystickBackObj != null && joystickImg != null &&
            joystickBackObj.activeSelf && !m_JoystickPickPanel.activeSelf)
        {
            typeJS = JoyStickType.Fixed;
            Vector3[] v = new Vector3[4];   
            joystickBackObj.GetComponent<RectTransform>().GetWorldCorners(v);//오브젝트의 네 모서리 좌표값 구하기
            radius = v[2].y - v[0].y;
            radius = radius / 3.0f;

            originPos = joystickImg.transform.position;
            basicAlpha = joystickImg.color;
        }

        if (attackBtn != null)
            attackBtn.onClick.AddListener(() =>
            {
                if (!refHero.gameObject.GetComponent<HeroCtrl>().isAttack)
                    refHero.gameObject.GetComponent<HeroCtrl>().isAttack = true;
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDrag()
    {       

        if (joystickImg == null) return;

        joystickCacVect = Input.mousePosition - originPos;
        joystickCacVect.z = 0.0f;
        joystickCacDist = joystickCacVect.magnitude;
        axis = joystickCacVect.normalized;

        joystickImg.color = new Color(1, 1, 1, 1);

        if (radius < joystickCacDist)
        {
            joystickImg.transform.position = originPos + axis * radius;
        }
        else
        {
            joystickImg.transform.position = originPos + axis * joystickCacDist;
        }

        if (1.0f < joystickCacDist) joystickCacDist = 1.0f;

        

        //캐릭터 이동 처리
        if (refHero != null)
        {
            refHero.GetComponent<HeroCtrl>().SetJoyStickMv(joystickCacDist, axis);
        }
    }
    
    public void OnDragEnd()
    {
        if (joystickImg == null) return;

        axis = Vector3.zero;
        joystickCacDist = 0.0f;
        joystickImg.transform.position = originPos;
        joystickImg.color = basicAlpha;

        //캐릭터 정지 처리
        if (refHero != null)
        {
            refHero.GetComponent<HeroCtrl>().SetJoyStickMv(joystickCacDist, axis);
        }
    }

    public static bool IsPointerOverUIObject() //UGUI의 UI들이 먼저 피킹되는지 확인하는 함수
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
        EventSystem.current.RaycastAll(a_EDCurPos, results);
        return (0 < results.Count);
#endif
    }//public bool IsPointerOverUIObject() 
}
