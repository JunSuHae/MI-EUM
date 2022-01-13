using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    // Start is called before the first frame update
    private int floorNum = 5;
    public void setFloorNum(int num) {
        floorNum = num;
    }
    [Header("Editor Objects")]
    public GameObject floorPrefab;
    
    void Start()
    {
        //GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        //plane.transform.localScale = new Vector3(0.1f, 1.0f, (floorNum-1)*0.1f);
        floorPrefab.transform.localScale = new Vector3(0.1f * (floorNum - 1), 1f, 0.1f);
        for ( int i = 0; i < 4; i++ ) {
            Quaternion rotation = Quaternion.Euler(0, 90 * i, 0);
            Instantiate(floorPrefab, rotation * new Vector3(-0.5f, 0f, (floorNum-1) / 2.0f), rotation);
            //plane.transform.position = new Vector3(floorNum / 2.0f, 0f, 0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
