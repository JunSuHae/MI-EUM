using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    private bool fall;
    private int trashingHeight = -5;
    private Stage stage;
    private Score score;
    private GameObject trash;
    // Start is called before the first frame update
    void Start()
    {
        stage = this.transform.parent.GetComponent<Stage>();
        score = GameObject.Find("ScoreSystem").GetComponent<Score>();
        //SceneManager.GetSceneByName("GameScene").get;
        //canvas = this.transform.parent.GetComponent<Canvas>();
        int bh = stage.boardHeight;
        for (int i = 0; i < bh; i++) {
            var col = new GameObject((bh - i - 1).ToString());
            col.transform.position = new Vector3(0, bh - i, 0);
            col.transform.parent = this.transform;
        }
        trash = new GameObject("trash");
        trash.transform.position = new Vector3(0, 0, 0);
        trash.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (stage.getFall()) {
            HandleTrash();
            Debug.Log("handle trash");
        }
    }

    private void HandleTrash() {
        if (trash.transform.childCount == 0) {
            trash.transform.position = new Vector3(0, 0, 0);
        } else {
            trash.transform.position += new Vector3(0, -1, 0);
        }
        int childCount = trash.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--) {
            GameObject child = trash.transform.GetChild(i).gameObject;
            if (child.transform.position.y < trashingHeight) {
                Destroy(child);
                //canvas.GetComponentInChildren<Score>().substractScore();
                score.substractScore();
            }
        }
    }
}
