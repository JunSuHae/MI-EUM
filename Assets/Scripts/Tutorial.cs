using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Header("Editor Objects")]
    public GameObject tutorialSubPanel1;
    public GameObject tutorialSubPanel2;
    public GameObject tutorialSubPanel3;
    private GameObject[] tutorialSubPanels;
    private int oldTutorialPage;
    private int tutorialPage;

    private int tutorialSubPanelNo;

    // Start is called before the first frame update
    void Start()
    {
        tutorialSubPanels = new GameObject[tutorialSubPanelNo];
        for (int i = 0; i < tutorialSubPanelNo; i++) {
            tutorialSubPanels[i] = GameObject.Find("tutorialSubPanel" + i.ToString());
        }
        oldTutorialPage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TurnOffAllPages() {
        foreach (GameObject panel in tutorialSubPanels) {
            panel.SetActive(false);
        }
    }

    public void ControlScene(int newTutorialPage) {
        // start == 0, game == 1, pause == 2, end == 3
        if (oldTutorialPage == newTutorialPage) return;
        oldTutorialPage = newTutorialPage;
        tutorialPage = newTutorialPage;

        TurnOffAllPages();
        tutorialSubPanels[newTutorialPage].SetActive(true);

    }
}
