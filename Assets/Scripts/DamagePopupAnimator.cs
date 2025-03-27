using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopupAnimator : MonoBehaviour
{
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private AnimationCurve heightCurve;

    private TextMeshProUGUI tmp;
    private float time = 0;
    private Vector2 origin;
    // Start is called before the first frame update
    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        origin = tmp.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        tmp.color = new Color(1, 1, 1, opacityCurve.Evaluate(time));
        transform.position = origin + new Vector2(0, heightCurve.Evaluate(time));
        time += Time.deltaTime;
    }
}
