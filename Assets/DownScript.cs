using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DownScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private float downTime;
    private bool active = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        Stage.stage.DownButtonPressed();
        downTime = Time.time;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Time.time - downTime < 0.2f) {
            if (active) {
                Stage.stage.DownButtonClicked();
            } else {
                active = true;
                this.GetComponent<Image>().color = Color.yellow;
                Stage.stage.DownButtonReleased();
            }
        } else {
            Stage.stage.DownButtonReleased();
        }
    }

    public void Update() {
        if (Time.time - downTime > 0.3f) {
            active = false;
            this.GetComponent<Image>().color = Color.white;
        }
    }
}