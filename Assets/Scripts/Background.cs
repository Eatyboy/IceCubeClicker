using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private float x, y;
    // Start is called before the first frame update
    void Start()
    {
        image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);


    }

    // Update is called once per frame
    void Update()
    {
        //if (image.uvRect.position == new Vector2(-image.uvRect.width, -image.uvRect.height)) image.uvRect = new(new(image.uvRect.width, image.uvRect.height), image.uvRect.size);
        //image.uvRect = new Rect(image.uvRect.position + new Vector2(x, y) * Time.deltaTime, image.uvRect.size);   
    }
}
