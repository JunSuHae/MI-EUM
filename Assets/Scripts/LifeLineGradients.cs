using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeLineGradients : MonoBehaviour
{
    Color c;
    float period = 2f;

    // Start is called before the first frame update
    void Start()
    {
        // c = this.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Time.time;
        SetScale(Mathf.Abs(Mathf.Sin(t * (Mathf.PI / period))));
    }

    void SetScale(float f)
    {
        this.transform.localScale = new Vector3(1, 0.2f + 0.1f * f, 1);
    }
}
