using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitText : MonoBehaviour
{
    public Text hitText;
    Color setColor;
    int size = 0;
    string hitPoint;
    Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        switch (hitPoint)
        {
            case "Miss":
                {
                    setColor = new Color(0.45f, 0.45f, 0.45f);
                    size = 50;
                }
                break;
            case "Good":
                {
                    setColor = new Color(0.12f, 0.89f, 0.91f);
                    size = 52;
                }
                break;
            case "Great":
                {
                    setColor = new Color(0.2f, 0.93f, 0.5f);
                    size = 56;
                }
                break;
            case "Nice":
                {
                    setColor = new Color(0.93f, 0.66f, 0.2f);
                    size = 54;
                }
                break;
            case "Excellent":
                {
                    setColor = new Color(0.93f, 0.24f, 0.15f);
                    size = 60;
                }
                break;
        }

        if (hitText != null)
        {
            hitText.text = hitPoint;
            hitText.color = setColor;
            hitText.fontSize = size;
        }

        transform.position = startPos;

        Destroy(gameObject, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string text)
    {
        hitPoint = text;
    }

}
