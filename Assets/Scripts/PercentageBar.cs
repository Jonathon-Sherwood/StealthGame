using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PercentageBar : MonoBehaviour
{
    public float currentValue;
    public float maxValue;
    public float criticalLevel = 0.25f;
    private Image imageComponent;
    public Color barColor = Color.green;
    public Color barCriticalColor = Color.magenta;

    // Start is called before the first frame update
    void Start()
    {
        imageComponent = GetComponent<Image>();
        if(imageComponent != null)
        {
            imageComponent.type = Image.Type.Filled;
            imageComponent.fillMethod = Image.FillMethod.Horizontal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float percentOfMax = currentValue / maxValue;

        imageComponent.fillAmount = percentOfMax;

        if(percentOfMax > criticalLevel)
        {
            imageComponent.color = barColor;
        }
        else
        {
            imageComponent.color = barCriticalColor;
        }
    }
}
