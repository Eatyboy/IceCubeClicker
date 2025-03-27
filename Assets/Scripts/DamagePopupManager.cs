using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

public class DamagePopupManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject textObject;
    private Vector2 positionVariation;

    public void NewPopup(string text, Vector2 pos, Color color)
    {
        positionVariation = new Vector2(Random.Range(-0.75f, 0.75f), Random.Range(-0.5f, 0.5f));
        var popup = Instantiate(textObject, pos + positionVariation, Quaternion.identity, canvas.transform);
        var temp = popup.GetComponent<TextMeshProUGUI>();
        temp.text = text;
        temp.faceColor = color;
        Destroy(popup, 1f);
    }
}
