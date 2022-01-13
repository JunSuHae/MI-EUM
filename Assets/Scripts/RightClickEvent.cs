using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightClickEvent : MonoBehaviour
{
    public void RightClick() {
        int rc = GetComponent<Animator>().GetInteger("RightClick");
        GetComponent<Animator>().SetInteger("RightClick", rc + 1);
    }
}
