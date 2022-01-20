using UnityEngine;

public class Simulation : MonoBehaviour {
    public Color color {
        set {
            renderer.material.color = value;
        }

        get {
            return renderer.material.color;
        }
    }

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
}