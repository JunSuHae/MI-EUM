using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class RotateCameraEvent : MonoBehaviour
{
    public void RotateCamera(int degree) {
        gameObject.transform.Rotate(new Vector3(0, degree, 0));
        Debug.Log("rotate done");
    }
}