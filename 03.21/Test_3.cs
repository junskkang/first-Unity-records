using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 조건부 컴파일 (전처리문)
// 전처리문 : #이 붙어있는 명령어들로 빌드(컴파일)시점에 동작하는 명령어
public class Test_3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if KOREA
        Debug.Log("이것은 한글 버전입니다.");
#elif ENGLISH
        Debug.Log("이것은 영어 버전입니다.");
#elif CHINA
        Debug.Log("이것은 중국어 버전입니다.");
#endif

    }



    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
#if UNITY_STANDALONE_WIN
        Debug.Log("윈도우에서 실행");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "윈도우에서 실행" + "</size>");
#elif UNITY_ANDROID
        Debug.Log("안드로이드에서 실행");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "안드로이드에서 실행" + "</size>");
#elif UNITY_IOS
        Debug.Log("아이폰에서 실행");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "아이폰에서 실행" + "</size>");
#elif UNITY_WEBGL
        Debug.Log("웹에서 실행");
        GUI.Label(new Rect(55, 10, 500, 60), "<size=25>" + "웹에서 실행" + "</size>");
#endif

    }
}
