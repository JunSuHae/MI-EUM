using UnityEngine;

public class Stage : MonoBehaviour {
    [Header("Editor Objects")]
    public GameObject cubePrefab;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetracubeNode;

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 10;
    [Range(5, 20)]
    public int boardHeight = 20;
    public float fallCycle = 1.0f;

    public Cube CreateCube(Transform parent, Vector3 position, Color color, int order = 1) {
        var go = Instantiate(cubePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;

        var cube = go.GetComponent<Cube>();
        cube.color = color;
        cube.sortingOrder = order;

        return cube;
    }

    private int halfWidth;
    private int halfHeight;
    private float nextFallTime;

    private void Start() {
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
        CreateTetracube();
    }

    private void Update() {
        Vector3 moveDir = Vector3.zero;
        Quaternion rotDir = Quaternion.identity;
        bool isRotate = false;
        // Move
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            moveDir.x = -1;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            moveDir.x = 1;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            moveDir.z = 1;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            moveDir.z = -1;
        }

        // Rotate
        if (Input.GetKeyDown(KeyCode.W)) {
            rotDir *= Quaternion.Euler(0, 90, 0);
            isRotate = true;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            rotDir *= Quaternion.Euler(90, 0, 0);
            isRotate = true;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            rotDir *= Quaternion.Euler(0, 0, 90);
            isRotate = true;
        }

        if (Time.time > nextFallTime) {
            nextFallTime = Time.time + fallCycle;
            moveDir = Vector3.down;
            isRotate = false;
        }
        
        if (moveDir != Vector3.zero) {
            MoveTetracube(moveDir);
        }
        if (isRotate) {
            RotateTetracube(rotDir);
        }
    }

    private bool MoveTetracube(Vector3 moveDir) {
        tetracubeNode.transform.position += moveDir;
        return true;
    }

    private bool RotateTetracube(Quaternion rotDir) {
        tetracubeNode.transform.rotation *= rotDir;
        return true;
    }


    private void CreateTetracube() {
        int index = Random.Range(0, 8);
        Debug.Log(index);
        Color32 color = Color.white;

        tetracubeNode.rotation = Quaternion.identity;
        tetracubeNode.position = new Vector3(0f, boardHeight + 0.5f, 0f);
        if (index == 2) {
            tetracubeNode.position += new Vector3(0.5f, 0.5f, 0.5f);
        }

        switch (index) {
            // Cube(1) : 하늘색
            case 0:
                color = new Color32(115, 251, 253, 255);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color);
                CreateCube(tetracubeNode, new Vector3(0.0f, 1.0f, 0.0f), color);
                break;

            // Cube(2) : 파란색
            case 1:
                color = new Color32(0, 33, 245, 255);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color);
                CreateCube(tetracubeNode, new Vector3(1.0f, 1.0f, 0.0f), color);
                break;

            // Cube(3) : 귤색
            case 2:
                color = new Color32(243, 168, 59, 255);
                CreateCube(tetracubeNode, new Vector3(-0.5f, 0.5f, -0.5f), color);
                CreateCube(tetracubeNode, new Vector3(-0.5f, 0.5f, 0.5f), color);
                CreateCube(tetracubeNode, new Vector3(0.5f, 0.5f, -0.5f), color);
                CreateCube(tetracubeNode, new Vector3(0.5f, 0.5f, 0.5f), color);
                break;

            // Cube(4) : 노란색
            case 3:
                color = new Color32(255, 253, 84, 255);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, -1.0f), color);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, -1.0f), color);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color);
                break;

            // Cube 5 : 검은색
            case 4:
                color = new Color32(0, 0, 0, 255);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, -1f), color);
                break;

            // Cube 6 : 하얀색
            case 5:
                color = new Color32(255, 255, 255, 255);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(-1f, 0f, 1f), color);
                break;

            // Cube 7 : 자주색
            case 6:
                color = new Color32(155, 47, 246, 255);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, -1f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 2f), color);
                break;

            // Cube 8 : 빨간색
            case 7:
                color = new Color32(235, 51, 35, 255);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color);
                CreateCube(tetracubeNode, new Vector3(0f, 1f, 1f), color);
                break;
        }
    }
}