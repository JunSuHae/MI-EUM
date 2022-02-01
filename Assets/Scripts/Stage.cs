using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [Header("Editor Objects")]
    public static Stage stage;
    public GameObject cubePrefab;
    public GameObject projectionPrefab;
    public GameObject projections;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetracubeNode;
    public GameObject gamePanel;
    public GameObject gameoverPanel;
    public GameObject pausePanel;
    public GameObject startPanel;
    private GameObject[] panels;
    public Text result;
    private string oldGameState;
    private string gameState;

    [Header("Game Settings")]
    [Range(4, 40)]
    public int boardWidth = 5;
    [Range(5, 20)]
    public int boardHeight = 10;
    public float fallCycle = 2.0f;
    private bool fall = false;
    public bool getFall()
    {
        return this.fall;
    }
    private Score score;

    public Cube CreateCube(Transform parent, Vector3 position, Color color, Color emission, float intensity, int order = 1)
    {
        var go = Instantiate(cubePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;
        go.layer = 3;

        var cube = go.GetComponent<Cube>();
        cube.color = color;
        cube.sortingOrder = order;

        go.GetComponent<Renderer>().material.SetColor("_EmissionColor", emission * Mathf.Pow(2.0f, intensity));

        return cube;
    }

    public Projection CreateProjection(Vector3 position, Color color, int order = 1)
    {
        var go = Instantiate(projectionPrefab);
        go.transform.position = position;
        go.transform.parent = projections.transform;

        var projection = go.GetComponent<Projection>();
        projection.color = color;
        projection.sortingOrder = order;

        return projection;
    }

    private int halfWidth;
    private int halfHeight;
    private float nextFallTime;
    private bool downy = false;

    private void Start()
    {
        stage = this;
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
        score = GameObject.Find("Score").GetComponent<Score>();
        projections = GameObject.Find("Projections");
        CreateTetracube();
        panels = new GameObject[] { startPanel, pausePanel, gameoverPanel, gamePanel};
        oldGameState = "";
        gameState = "start";
        ControlScene(gameState);
    }

    private void Update()
    {
        ControlScene(gameState);

        switch (gameState)
        {
            case "start":
                break;
            case "game":
                GameObject lastColumn = GameObject.Find((boardHeight - 1).ToString());
                if (lastColumn.transform.childCount != 0)
                {
                    gameState = "end";
                    result.text = score.getScore().ToString();
                    //gameoverPanel.SetActive(true);
                    return;
                }
                int moveDir = 0;
                int rotDir = 0;
                bool isRotate = false;
                // Move
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    moveDir = 3;
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    moveDir = 4;
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    moveDir = 2;
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    moveDir = 1;
                }

                // Rotate
                if (Input.GetKeyDown(KeyCode.W))
                {
                    rotDir = 1;
                    isRotate = true;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    rotDir = 2;
                    isRotate = true;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    rotDir = 3;
                    isRotate = true;
                }

                if (Time.time > nextFallTime)
                {
                    nextFallTime = Time.time + fallCycle;
                    maxFallTime = Time.time + 1.0f;
                    moveDir = -1;
                    isRotate = false;
                    fall = true;
                }
                else
                {
                    fall = false;
                }

                if (moveDir != 0)
                {
                    MoveTetracube(moveDir);
                }
                if (isRotate)
                {
                    RotateTetracube(rotDir);
                }

                foreach (Transform child in projections.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                ShowCubeProjection();
                break;
            case "pause":
                break;
            case "end":
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
                break;
        }

        if (gameoverPanel.activeSelf)
        {

        }
        else
        {
        }
    }

    public void ClearChildren(Transform t) {
        foreach (Transform c in t) {
            ClearChildren(c);
            Destroy(c.gameObject);
        }
        t.DetachChildren();
    }
    public void Restart()
    {
        gameState = "game";
        score.initScore();
        Transform board = GameObject.Find("Board").transform;
        foreach (Transform column in board) {
            ClearChildren(column);
        }
        ClearChildren(tetracubeNode);
        CreateTetracube();
    }
    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    //solution 2. calculate x, z by column and get min(y of tetracube, max(board)) 
    //ㅁ 바깥에 위치한 건 바닥으로 projection
    //미리 simulation을 깔아두고 투명도 조절: 이건 큐브 이동에 따라 효율성이 떨어질지도

    private void ShowCubeProjection()
    {
        Color lemon = new Color32(255, 255, 0, 0);

        HashSet<string> xz_position = new HashSet<string>();

        for (int i = 0; i < tetracubeNode.childCount; ++i)
        {
            var node = tetracubeNode.GetChild(i);

            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);

            if (y < 0 || (xz_position.Contains(x.ToString() + ", " + z.ToString())))
                continue;

            xz_position.Add(x.ToString() + ", " + z.ToString());

            int absx = Mathf.Abs(x);
            int absz = Mathf.Abs(z);
            if (y > 0) {
                if ((absx == 2 || absz == 2) && absx <= 2 && absz <= 2)
                {
                    var column = boardNode.Find((y - 1).ToString());
                    if (column.Find(x.ToString() + ", " + z.ToString()) == null) {
                        bool b = true;
                        int j = y - 2;
                        for (j = y - 2; j >= 0; j--)
                        {
                            column = boardNode.Find(j.ToString());
                            if (column.Find(x.ToString() + ", " + z.ToString()) != null)
                            {
                                CreateProjection(new Vector3(x, j + 1.1f, z), lemon);
                                b = false;
                                break;
                            }
                        }
                        if (j == -1 && b)
                        {
                            CreateProjection(new Vector3(x, 0.05f, z), lemon);
                        }
                    }
                }
                else {
                    CreateProjection(new Vector3(x, 0.05f, z), Color.red);
                }
            }


            /*
            1 아무것도 없을 때: (그냥 board 바닥 +) y=0 구역
            2 블럭이 있을 때: 해당 x, z의 블럭에 있는 y값을 전부 찾은 뒤
            내려오는 블럭보다 작은 값들 중 최댓값
            */

        }
    }

    // private bool MoveTetracube(Vector3 moveDir) {
    //     Vector3 oldPos = tetracubeNode.transform.position;
    //     Quaternion oldRot = tetracubeNode.transform.rotation;

    //     float rotAngle = GameObject.Find("CameraBase").transform.eulerAngles.y;
    //     moveDir = Quaternion.Euler(0, rotAngle, 0) * moveDir;

    //     tetracubeNode.transform.position += moveDir;

    //     if (!CanMoveTo(tetracubeNode))
    //     {
    //         tetracubeNode.transform.position = oldPos;
    //         tetracubeNode.transform.rotation = oldRot;

    //         if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && (int)moveDir.z == 0)
    //         {
    //             AddToBoard(tetracubeNode);
    //             CheckBoardColumn();
    //             CreateTetracube();
    //         }

    //         return false;
    //     }
    //     return true;
    // }

    public void MoveTetracube(int i)
    {
        Vector3 oldPos = tetracubeNode.transform.position;
        Quaternion oldRot = tetracubeNode.transform.rotation;
        Vector3 moveDir = new Vector3(0, 0, 0);
        switch (i)
        {
            case -1:
                moveDir = new Vector3(0, -1, 0);
                break;
            case 1:
                moveDir = new Vector3(-1, 0, 0);
                break;
            case 2:
                moveDir = new Vector3(1, 0, 0);
                break;
            case 3:
                moveDir = new Vector3(0, 0, 1);
                break;
            case 4:
                moveDir = new Vector3(0, 0, -1);
                break;
        }

        float rotAngle = GameObject.Find("CameraBase").transform.eulerAngles.y;
        moveDir = Quaternion.Euler(0, rotAngle, 0) * moveDir;

        tetracubeNode.transform.position += moveDir;

        if (!CanMoveTo(tetracubeNode))
        {
            tetracubeNode.transform.position = oldPos;
            tetracubeNode.transform.rotation = oldRot;

            if ((int)moveDir.y == -1 && (int)moveDir.x == 0 && (int)moveDir.z == 0)
            {
                AddToBoard(tetracubeNode);
                CheckBoardColumn();
                CreateTetracube();
                if (downy) fallCycle = 0.2f;
                else fallCycle = 1.0f;
                nextFallTime = Time.time + fallCycle;
                maxFallTime = Time.time + 1.0f;
            }
        }
    }

    // private bool RotateTetracube(Quaternion rotDir) {
    //     tetracubeNode.transform.rotation *= rotDir;
    //     return true;
    // }

    public void RotateTetracube(int rotDir)
    {
        Vector3 oldPos = tetracubeNode.transform.position;
        Quaternion oldRot = tetracubeNode.transform.rotation;
        float rotAngle = GameObject.Find("CameraBase").transform.eulerAngles.y;
        tetracubeNode.Rotate(new Vector3(0, -rotAngle, 0), Space.World);

        switch (rotDir)
        {
            case 1:
                tetracubeNode.Rotate(new Vector3(0, 90, 0), Space.World);
                break;
            case 2:
                tetracubeNode.Rotate(new Vector3(90, 0, 0), Space.World);
                break;
            case 3:
                tetracubeNode.Rotate(new Vector3(0, 0, 90), Space.World);
                break;
        }
        tetracubeNode.Rotate(new Vector3(0, rotAngle, 0), Space.World);

        if (!CanRotateTo(tetracubeNode))
        {
            tetracubeNode.transform.position = oldPos;
            tetracubeNode.transform.rotation = oldRot;
        }
    }

    bool CanMoveTo(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);

            // if (x < 0 || x > boardWidth - 1)
            //     return false;

            if (y < 0)
                return false;

            var column = boardNode.Find(y.ToString());

            if (column != null && column.Find(x.ToString() + ", " + z.ToString()) != null)
            {
                return false;
            }
        }

        return true;
    }

    bool CanRotateTo(Transform root)
    {
        for (int i = 0; i < root.childCount; ++i)
        {
            var node = root.GetChild(i);
            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);

            var column = boardNode.Find(y.ToString());

            if (column != null && column.Find(x.ToString() + ", " + z.ToString()) != null)
            {
                return false;
            }
        }

        return true;
    }

    void AddToBoard(Transform root)
    {
        while (root.childCount > 0)
        {
            var node = root.GetChild(0);

            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);
            if ((Mathf.Abs(x) == 2 || Mathf.Abs(z) == 2) &&
                (Mathf.Abs(x) <= 2 && Mathf.Abs(z) <= 2))
            {
                node.parent = boardNode.Find(y.ToString());
                node.name = x.ToString() + ", " + z.ToString();
            }
            else
            {
                node.parent = boardNode.Find("trash");
                score.substractScore();
            }
        }
        foreach (Transform t in boardNode.Find("trash")) {
            t.gameObject.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }

    void CheckBoardColumn()
    {
        bool isCleared = false;
        int fullBlockNum = 4 * boardWidth;
        int clearedLine = 0;
        foreach (Transform column in boardNode)
        {
            if (column.name == "trash")
            {
                continue;
            }
            if (column.transform.childCount == fullBlockNum)
            {
                Debug.Log("Destroy");
                foreach (Transform tile in column)
                {
                    Destroy(tile.gameObject);
                }
                column.DetachChildren();
                clearedLine += 1;
                isCleared = true;
            }
        }
        if (isCleared)
        {
            score.addScore(clearedLine);
            for (int i = 0; i < boardHeight; ++i)
            {
                var column = boardNode.Find(i.ToString());

                // 이미 비어 있는 행은 무시
                if (column.transform.childCount == 0)
                {
                    continue;
                }

                int emptyCol = 0;
                int j = i - 1;
                while (j >= 0)
                {
                    if (boardNode.Find(j.ToString()).childCount == 0)
                    {
                        emptyCol++;
                    }
                    j--;
                }

                if (emptyCol > 0)
                {
                    var targetColumn = boardNode.Find((i - emptyCol).ToString());

                    while (column.childCount > 0)
                    {
                        Transform tile = column.GetChild(0);
                        tile.parent = targetColumn;
                        tile.transform.position += new Vector3(0, -emptyCol, 0);
                    }
                    column.DetachChildren();
                }
            }
        }
    }

    // int globalindex = 0;

    private void CreateTetracube()
    {
        // int index = globalindex % 8;
        // globalindex++;
        int index = Random.Range(0, 8);
        // int index = 0;
        // Debug.Log(index);
        Color32 color = Color.white;
        Color32 emission;
        float intensity;

        tetracubeNode.rotation = Quaternion.identity;
        tetracubeNode.position = new Vector3(0f, boardHeight + 0.5f, 0f);
        // Debug.Log("switch");

        switch (index)
        {
            // Cube(1) : �ϴû�
            case 0:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(0, 219, 219, 255);
                intensity = 2.9f;
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 1.0f, 0.0f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube(2) : �Ķ���
            case 1:
                color = new Color32(0, 33, 245, 255);
                emission = new Color32(4, 4, 255, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 1.0f, 0.0f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube(3) : �ֻ�
            case 2:
                color = new Color32(255, 195, 0, 255);
                emission = new Color32(234, 4, 0, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 1.0f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube(4) : �����
            case 3:
                color = new Color32(16, 0, 255, 255);
                emission = new Color32(224, 214, 0, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 1.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1.0f, 0.0f, 0.0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0.0f, 0.0f, 2.0f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube 5 : ������
            case 4:
                color = new Color32(84, 255, 0, 255);
                emission = new Color32(9, 215, 0, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, -1f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube 6 : �Ͼ��
            case 5:
                color = new Color32(0, 255, 216, 255);
                emission = new Color32(255, 0, 178, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(-1f, 0f, 1f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube 7 : ���ֻ�
            case 6:
                color = new Color32(255, 95, 196, 255);
                emission = new Color32(2, 0, 255, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, -1f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 2f), color, emission, intensity);
                // Debug.Log("1");
                break;

            // Cube 8 : ������
            case 7:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(228, 0, 0, 255);
                intensity = 3.0f;
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 0f, 1f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(1f, 0f, 0f), color, emission, intensity);
                CreateCube(tetracubeNode, new Vector3(0f, 1f, 1f), color, emission, intensity);
                // Debug.Log("1");
                break;
        }
    }

    private void TurnOffAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    public void ControlScene(string newGameState)
    {
        // start == 0, game == 1, pause == 2, end == 3
        if (oldGameState == newGameState) return;
        Debug.Log(oldGameState + ", " + newGameState);
        oldGameState = newGameState;
        gameState = newGameState;
        switch (newGameState)
        {
            case "start":
                TurnOffAllPanels();
                startPanel.SetActive(true);
                gameObject.SetActive(false);
                break;
            case "game":
                TurnOffAllPanels();
                gamePanel.SetActive(true);
                gameObject.SetActive(true);
                break;
            case "restart":
                TurnOffAllPanels();
                gamePanel.SetActive(true);
                gameObject.SetActive(true);
                Restart();
                break;
            case "pause":
                TurnOffAllPanels();
                pausePanel.SetActive(true);
                break;
            case "end":
                TurnOffAllPanels();
                gameoverPanel.SetActive(true);
                break;
        }
    }

    private float maxFallTime;
    public void DownButtonPressed()
    {
        downy = true;
        maxFallTime = nextFallTime;
        nextFallTime = Mathf.Min(maxFallTime, Time.time + 0.2f);
        fallCycle = 0.2f;
    }
    public void DownButtonReleased()
    {
        downy = false;
        nextFallTime = maxFallTime;
        fallCycle = 1.0f;
    }
    public void DownButtonClicked()
    {
        downy = false;
        nextFallTime = Time.time;
        fallCycle = 0.03f;
    }
}