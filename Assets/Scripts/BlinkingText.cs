using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkingText : MonoBehaviour
{
    Color c;
    float period = 2f;

    // Start is called before the first frame update
    void Start()
    {
        c = this.GetComponent<Text>().color;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time;
        SetOpacity(Mathf.Abs(Mathf.Sin(t * (Mathf.PI / period))));
    }

    void SetOpacity(float f)
    {
        this.GetComponent<Text>().color = new Color(c.r, c.g, c.b, c.a * f);
    }
}
