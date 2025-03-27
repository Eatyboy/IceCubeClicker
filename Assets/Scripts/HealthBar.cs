using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void setMaxHp(float hp)
    {
        slider.maxValue = hp;
    }

    public void setHp(float hp)
    {
        slider.value = hp;
    }
}
