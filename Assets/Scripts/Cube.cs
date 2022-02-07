using UnityEngine;

public class Cube : MonoBehaviour {

    private float start = 2.0f;
    private float period = 2.0f;
    public Color color {
        set {
            renderer.material.color = value;
        }

        get {
            return renderer.material.color;
        }
    }
    public Color emission;

    public int sortingOrder {
        set {
            renderer.sortingOrder = value;
        }

        get {
            return renderer.sortingOrder;
        }
    }

    Renderer renderer;

    private void Awake() {
        renderer = GetComponent<Renderer>();

        if (renderer == null) {
            Debug.LogError("You need to meshRenderer for Block");
        }
    }

    // private void Update() {
    //     float t = Mathf.Abs(Mathf.Sin( (Time.time + start) * (Mathf.PI / period)));
    //     renderer.material.SetColor("_EmissionColor", emission * (0.5f + 8*t));
    //     // renderer.material.SetColor("_EmissionColor", emission * Mathf.Pow(2.0f, 0f + 3*t));

    // }
    
    public void SetBlink(float s, float p) {
        start = s;
        period = p;
    }
}