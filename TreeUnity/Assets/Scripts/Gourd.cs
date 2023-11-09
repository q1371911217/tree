using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gourd : MonoBehaviour
{
    [SerializeField]
    List<Sprite> spriteList;
    [SerializeField]
    Image img;

    private bool isSelected = false;
    private bool isSystemSelected = false;
    private bool isGiftSelected = false;

    public int select()
    {
        isSelected = !isSelected;
        if (isSelected)
            img.sprite = spriteList[1];
        else
            img.sprite = spriteList[0];
        img.SetNativeSize();

        return isSelected ? 1 : -1;
    }

    public bool isSelect()
    {
        return isSelected;
    }

    public bool isSystemSelect()
    {
        return isSystemSelected;
    }

    public void clear()
    {
        isSelected = false;
        img.sprite = spriteList[0];
        img.SetNativeSize();
    }

    public void systemSelect()
    {
        isSystemSelected = true;
        if(isSelected)
        {
            img.sprite = spriteList[2];
        }
        else
        {
            img.sprite = spriteList[3];
        }

        img.SetNativeSize();
    }

    public void giftSelect()
    {
        isGiftSelected = true;
        if(isSelected)
            img.sprite = spriteList[5];
        else
            img.sprite = spriteList[4];

        img.transform.GetChild(0).gameObject.SetActive(false);
        img.SetNativeSize();
    }

    public void clearSystem()
    {
        isSystemSelected = false;
        isGiftSelected = false;
        img.transform.GetChild(0).gameObject.SetActive(true);
        if (isSelected)
            img.sprite = spriteList[1];
        else
            img.sprite = spriteList[0];
        img.SetNativeSize();
    }

    public bool isReward()
    {
        return isSelected && isSystemSelected;
    }

    public bool isGift()
    {
        return isSelected && isGiftSelected;
    }
}
