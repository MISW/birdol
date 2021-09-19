using UnityEngine;
using UnityEngine.UI;

public class IgnoreAlpha : MonoBehaviour
{
    Image image;
    void Start()
    {
        image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 1f; //検出したいピクセルの透明度の閾値を0から1の間
    }
}
