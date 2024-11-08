using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitNode : MonoBehaviour
{
    public Image image;
    public Sprite sprite;

    private void Start()
    {
        image = this.GetComponent<Image>();
        sprite = image.sprite;
    }
    
}
