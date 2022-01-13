using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftClickEvent : MonoBehaviour
{
    public void LeftClick() {
        int lc = GetComponent<Animator>().GetInteger("LeftClick");
        GetComponent<Animator>().SetInteger("LeftClick", lc + 1);
    }
}
