using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Ui_PlayerHeart : MonoBehaviour
{
    [Header("Images/Sprites")] public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;
    
    public void OnUpdateHeart(int _life)
    {
        var spriteToChange = emptyHeart;
        if (_life == 2)
            spriteToChange = fullHeart;
        else if (_life == 1)
            spriteToChange = halfHeart;

        transform.GetComponent<Image>().sprite = spriteToChange;
    }
}