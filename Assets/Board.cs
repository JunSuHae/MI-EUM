using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Stage stage = this.transform.parent.GetComponent<Stage>();
        int bh = stage.boardHeight;
        for (int i = 0; i < bh; i++) {
            var col = new GameObject((bh - i - 1).ToString());
            col.transform.position = new Vector3(0, bh - i, 0);
            col.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
