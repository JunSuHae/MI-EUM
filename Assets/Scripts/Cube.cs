using UnityEngine;

public class Cube : MonoBehaviour {
    public Color color {
        set {
            GetComponent<Renderer>().material.color = value;
        }

        get {
            return GetComponent<Renderer>().material.color;
        }
    }

    public int sortingOrder {
        set {
            GetComponent<Renderer>().sortingOrder = value;
        }

        get {
            return GetComponent<Renderer>().sortingOrder;
        }
    }

    Renderer r;

    private void Awake() {
        r = GetComponent<Renderer>();

        if (GetComponent<Renderer>() == null) {
            Debug.LogError("You need to meshRenderer for Block");
        }
    }
}