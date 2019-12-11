using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    [SerializeField] private Text _toolTipText;
    [SerializeField] private Image _toolTipImage;
    public Sprite _leftClick;
    public Sprite _rightClick;

    public void showPopup(bool isVisible, string toolTipText = null, Sprite toolTipImage = null)
    {
        if (isVisible)
        {
            _toolTipText.text = toolTipText;
            _toolTipImage.sprite = toolTipImage;
            _toolTipImage.color = new Color(255, 255, 255, 50);
        }
        else
        {
            _toolTipText.text = null;
            _toolTipImage.sprite = null;
            _toolTipImage.color = new Color(255, 255, 255, 0);
        }
    }

}
