using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeLine : MonoBehaviour
{
    // Start is called before the first frame update
    private int maxLife = 20;
    private int life = 20;

    private Color glowstart = (Color) new Color32(0, 98, 191, 255);
    private Color glowend = (Color) new Color32(255, 0, 0, 255);
    private float h1, h2, s1, s2, v1, v2;

    public GameObject basis;

    
    void Start()
    {
        Color.RGBToHSV(glowstart, out h1, out s1, out v1);
        Color.RGBToHSV(glowend, out h2, out s2, out v2);
    }

    // Update is called once per frame
    void Update()
    {
        float f = 1.0f * (maxLife - life) / maxLife;
        gameObject.transform.position = new Vector3(0, 10.0f * Mathf.Min(1.0f, f), 0);
        
        Color e1 = Color.HSVToRGB( h1 * (1-f) + h2 * f, s1 * (1-f) + s2 * f, v1 * (1-f) + v2 * f );
        Color e2 = e1 / Mathf.Pow((float)(e1.r * 0.3 + e1.g * 0.59 + e1.b * 0.11), 0.5f) * Mathf.Pow(2.0f, 2.8f);

        foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>())
        {
            r.material.SetColor("_EmissionColor", e2);
            // r.material.SetColor("_EmissionColor", e / (float)(e.r * 0.3 + e.g * 0.59 + e.b * 0.11) * Mathf.Pow(2.0f, 2.5f));
            // r.material.SetColor("_EmissionColor", Color.Lerp(glowstart, glowend, f));
        }

        foreach (Renderer r in basis.GetComponentsInChildren<Renderer>())
        {
            r.material.SetColor("_EmissionColor", e2 * 0.1f);
        }

        // foreach (SpriteRenderer sr in GameObject.Find("Gradients").GetComponentsInChildren<SpriteRenderer>())
        // {
        //     sr.color = Color.red * f + Color.white * (1-f);
        // } 
    }

    public void Damage() {
        if (life > 0) {
            life -= 1;
        }
    }

    public void InitLife() {
        life = maxLife;
    }

    public bool Dead() {
        if (life < 1) return true;
        return false;
    }
    
}
