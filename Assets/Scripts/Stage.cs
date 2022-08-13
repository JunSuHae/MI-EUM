using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Stage : MonoBehaviour
{
    [Header("Editor Objects")]
    public static Stage stage;
    public GameObject cubePrefab;
    public GameObject projectionPrefab;
    public GameObject projections;
    public LifeLine lifeLine;
    public GameObject lifePlane;
    public Transform backgroundNode;
    public Transform boardNode;
    public Transform tetracubeNode;
    public Transform aniTetracubeNode;
    public Transform nextTetracubeNode;
    public GameObject gamePanel;
    public GameObject gameoverPanel;
    public GameObject pausePanel;
    public GameObject startPanel;
    private GameObject[] panels;
    public GameObject nextCube;
    public Text result;
    public AnimationCurve easyOutCurve;
    public AnimationCurve brrrCurve;
    private string oldGameState;
    private string gameState;
    private Quaternion realRot;

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

    public void ChangeAllLayer(GameObject go, int layernum)
    {
        ChangeAllLayer(go.transform, layernum);
    }
    
    public void ChangeAllLayer(Transform trans, int layernum)
    {
        trans.gameObject.layer = layernum;
        foreach(Transform child in trans)
        {
            ChangeAllLayer(child, layernum);
        }
    }

    public GameObject CreateCube(Transform parent, Vector3 position, Color color, Color emission, float intensity, int order = 1, int layernum = 3, string name = "Cube")
    {
        var go = Instantiate(cubePrefab);
        go.transform.parent = parent;
        go.transform.localPosition = position;
        ChangeAllLayer(go, layernum);

        var cube = go.GetComponent<Cube>();
        cube.color = color;
        cube.sortingOrder = order;

        go.GetComponent<Renderer>().material.SetColor("_EmissionColor", emission * Mathf.Pow(2.0f, intensity));
        go.name = name;

        return go;
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
    private int nextindex;

    private void Start()
    {
        stage = this;
        halfWidth = Mathf.RoundToInt(boardWidth * 0.5f);
        halfHeight = Mathf.RoundToInt(boardHeight * 0.5f);
        score = GameObject.Find("Score").GetComponent<Score>();
        projections = GameObject.Find("Projections");
        nextindex = Random.Range(0, 8);
        CreateTetracube();
        panels = new GameObject[] { startPanel, pausePanel, gameoverPanel, gamePanel};
        oldGameState = "";
        gameState = "start";
        lifeLine.InitLife();
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
                    EndGame();
                    //gameoverPanel.SetActive(true);
                    return;
                }
                int moveDir = 0;
                // int rotDir = 0;
                // bool isRotate = false;
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
                // if (Input.GetKeyDown(KeyCode.W))
                // {
                //     rotDir = 1;
                //     isRotate = true;
                // }
                // if (Input.GetKeyDown(KeyCode.A))
                // {
                //     rotDir = 2;
                //     isRotate = true;
                // }
                // if (Input.GetKeyDown(KeyCode.D))
                // {
                //     rotDir = 3;
                //     isRotate = true;
                // }
                if (Input.GetKeyDown(KeyCode.X)) {
                    RotateTetracube(1);
                }
                if (Input.GetKeyDown(KeyCode.Z)) {
                    RotateTetracube(2);
                }
                if (Input.GetKeyDown(KeyCode.C)) {
                    RotateTetracube(3);
                }

                if (Time.time > nextFallTime)
                {
                    nextFallTime = Time.time + fallCycle;
                    maxFallTime = Time.time + 1.0f;
                    moveDir = -1;
                    // isRotate = false;
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
                // if (isRotate)
                // {
                //     // RotateTetracube(rotDir);
                // }

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
        score.initSC();
        lifeLine.InitLife();
        // lifePlane.transform.localScale = new Vector3(1, 0, 1);
        Transform board = GameObject.Find("Board").transform;
        foreach (Transform column in board) {
            ClearChildren(column);
        }
        ClearChildren(tetracubeNode);
        ClearChildren(aniTetracubeNode);
        ClearChildren(nextTetracubeNode);
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
                    var column = boardNode.Find((Mathf.Min(y, boardHeight) - 1).ToString());
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
        aniTetracubeNode.transform.position += moveDir;

        if (!CanShiftTo(tetracubeNode) || CanBeDeadAt(tetracubeNode))
        {
            bool b = !CanFallTo(tetracubeNode) || !CanBeAliveAt(tetracubeNode);
            if (!CanShiftTo(tetracubeNode)) {
                tetracubeNode.transform.position = oldPos;
                aniTetracubeNode.transform.position = oldPos;
            }
            if (moveDir.y == -1) {
                foreach (Transform node in tetracubeNode) {
                    int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
                    if (y < 0) {
                        DamageBy(node);
                        DamageBy(aniTetracubeNode.Find(node.name));
                    }
                }
                if (b) {
                    AddToBoard(tetracubeNode);
                    ClearChildren(aniTetracubeNode);
                    ClearChildren(nextTetracubeNode);
                    CheckBoardColumn();
                    CreateTetracube();
                    realRot = tetracubeNode.transform.rotation;
                    if (downy) fallCycle = 0.2f;
                    else fallCycle = 1.0f;
                    nextFallTime = Time.time + fallCycle;
                    maxFallTime = Time.time + 1.0f;
                }
            }
        }
    }

    // public void RotateTetracube(int rotDir)
    // {
    //     foreach (Transform node in tetracubeNode) {
    //         int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
    //         if (y < 0) {
    //             return;
    //         }
    //     }
        
    //     Vector3 oldPos = tetracubeNode.transform.position;
    //     Quaternion oldRot = tetracubeNode.transform.rotation;
    //     float rotAngle = GameObject.Find("CameraBase").transform.eulerAngles.y;
    //     tetracubeNode.Rotate(new Vector3(0, -rotAngle, 0), Space.World);

    //     switch (rotDir)
    //     {
    //         case 1:
    //             tetracubeNode.Rotate(new Vector3(0, 90, 0), Space.World);
    //             break;
    //         case 2:
    //             tetracubeNode.Rotate(new Vector3(90, 0, 0), Space.World);
    //             break;
    //         case 3:
    //             tetracubeNode.Rotate(new Vector3(0, 0, 90), Space.World);
    //             break;
    //     }
    //     tetracubeNode.Rotate(new Vector3(0, rotAngle, 0), Space.World);

    //     if (!CanShiftTo(tetracubeNode) || CanBeDeadAt(tetracubeNode))
    //     {
    //         if (!CanShiftTo(tetracubeNode)) {
    //             tetracubeNode.transform.position = oldPos;
    //             tetracubeNode.transform.rotation = oldRot;
    //         }
    //         foreach (Transform node in tetracubeNode) {
    //             int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
    //             if (y < 0) {
    //                 DamageBy(node);
    //             }
    //         }
    //     }
    // }

    bool CanShiftTo(Transform root) {
        return (CanMoveTo(root)) && CanFallTo(root) && CanBeAliveAt(root);
    }

    bool CanMoveTo(Transform root)
    {
        foreach (Transform node in root)
        {
            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);

            if (x < -4 || x > boardWidth)
                return false;
            if (z < -4 || z > boardWidth)
                return false;
            
            if ((Mathf.Abs(x) == 2 || Mathf.Abs(z) == 2) &&
                (Mathf.Abs(x) <= 2 && Mathf.Abs(z) <= 2) && (y < 0))
            {
                return false;
            }
        }

        return true;
    }
    bool CanFallTo(Transform root)
    {
        foreach (Transform node in root)
        {
            int x = Mathf.RoundToInt(node.transform.position.x);
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            int z = Mathf.RoundToInt(node.transform.position.z);var column = boardNode.Find(y.ToString());

            if (column != null && column.Find(x.ToString() + ", " + z.ToString()) != null)
            {
                return false;
            }

            if ((Mathf.Abs(x) == 2 || Mathf.Abs(z) == 2) &&
                (Mathf.Abs(x) <= 2 && Mathf.Abs(z) <= 2) && (y < 0))
            {
                return false;
            }
        }

        return true;
    }

    bool CanBeDeadAt(Transform root)
    {
        foreach (Transform node in root)
        {
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            if (y < 0)
                return true;
        }

        return false;
    }

    bool CanBeAliveAt(Transform root)
    {
        foreach (Transform node in root)
        {
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            if (y >= 0)
                return true;
        }

        return false;
    }

    void DamageBy(Transform node) {
        Material m = node.gameObject.GetComponent<Renderer>().material;
        // Debug.Log(lifeLine.Dead());
        if (m.IsKeywordEnabled("_EMISSION")) {
            m.DisableKeyword("_EMISSION");
            lifeLine.Damage();
            // lifeLine.transform.position = new Vector3(0, Mathf.Min(20.0f, 10.0f/maxLives * (maxLives - lives)), 0);
            // lifePlane.transform.localScale = new Vector3(1, 2.0f/maxLives * (maxLives - lives), 1);
            if (lifeLine.Dead()) {
                EndGame();
            }
        }
    }

    void AddToBoard(Transform root)
    {
        ChangeAllLayer(root, 3);
        int n = 0;
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
                n += 1;
            }
            else
            {
                node.parent = boardNode.Find("trash");
                DamageBy(node);
                DamageBy(aniTetracubeNode.Find(node.name));
            }
        }


        score.addScore(n * 10);

        if (n == 4) {
            score.addCombo();
        } else {
            score.initCombo();
        }


    }

    void EndGame()
    {
        gameState = "end";
        result.text = score.getScore().ToString();
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
                // Debug.Log("Destroy");
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
            score.addLineScore(clearedLine);
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
        int index = nextindex;
        nextindex = Random.Range(0, 8);
        // int index = 0;
        // Debug.Log(index);
        Color32 color = Color.white;
        Color32 emission = Color.white;
        float intensity = 0.0f;

        tetracubeNode.rotation = Quaternion.identity;
        tetracubeNode.position = new Vector3(0f, boardHeight + 0.5f, 0f);
        aniTetracubeNode.rotation = Quaternion.identity;
        aniTetracubeNode.position = new Vector3(0f, boardHeight + 0.5f, 0f);
        // Debug.Log("switch");
        Color32 transparent = new Color32(0, 0, 0, 255);
        Vector3[] blockmap = new Vector3[4];

        switch (index)
        {
            // Cube(1) : �ϴû�
            case 0:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(0, 219, 219, 255);
                intensity = 2.9f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f)};
                break;

            // Cube(2) : �Ķ���
            case 1:
                color = new Color32(0, 33, 245, 255);
                emission = new Color32(4, 4, 255, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f)};
                break;

            // Cube(3) : �ֻ�
            case 2:
                color = new Color32(255, 195, 0, 255);
                emission = new Color32(234, 4, 0, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f)};
                break;

            // Cube(4) : �����
            case 3:
                color = new Color32(16, 0, 255, 255);
                emission = new Color32(224, 214, 0, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 2.0f)};
                break;

            // Cube 5 : ������
            case 4:
                color = new Color32(84, 255, 0, 255);
                emission = new Color32(9, 215, 0, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)};
                break;

            // Cube 6 : �Ͼ��
            case 5:
                color = new Color32(0, 255, 216, 255);
                emission = new Color32(255, 0, 178, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(-1.0f, 0.0f, 1.0f)};
                break;

            // Cube 7 : ���ֻ�
            case 6:
                color = new Color32(255, 95, 196, 255);
                emission = new Color32(2, 0, 255, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, 2.0f)};
                break;

            // Cube 8 : ������
            case 7:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(228, 0, 0, 255);
                intensity = 3.0f;
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f)};
                break;
        }
        
        int blockorder = 0;
        foreach (Vector3 block in blockmap) {
            CreateCube(tetracubeNode, block, color, emission, intensity, layernum: 9, name: blockorder.ToString());
            CreateCube(aniTetracubeNode, block, color, emission, intensity, name: blockorder.ToString());
            blockorder++;
        }
        
        nextTetracubeNode.rotation = Quaternion.identity;
        nextTetracubeNode.position = new Vector3(0f, 0.5f, 0f);
        Vector3 deltapos = new Vector3(0f, 0f, 0f);
        // Debug.Log("switch");

        switch (nextindex)
        {
            // Cube(1) : �ϴû�
            case 0:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(0, 219, 219, 255);
                intensity = 2.9f;
                deltapos = new Vector3(-0.5f, -0.5f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f)};
                break;

            // Cube(2) : �Ķ���
            case 1:
                color = new Color32(0, 33, 245, 255);
                emission = new Color32(4, 4, 255, 255);
                intensity = 3.0f;
                deltapos = new Vector3(-0.5f, -0.5f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f)};
                break;

            // Cube(3) : �ֻ�
            case 2:
                color = new Color32(255, 195, 0, 255);
                emission = new Color32(234, 4, 0, 255);
                intensity = 3.0f;
                deltapos = new Vector3(-0.5f, 0.0f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f)};
                break;

            // Cube(4) : �����
            case 3:
                color = new Color32(16, 0, 255, 255);
                emission = new Color32(224, 214, 0, 255);
                intensity = 3.0f;
                deltapos = new Vector3(0.0f, 0.0f, -1.0f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 2.0f)};
                break;

            // Cube 5 : ������
            case 4:
                color = new Color32(84, 255, 0, 255);
                emission = new Color32(9, 215, 0, 255);
                intensity = 3.0f;
                deltapos = new Vector3(-0.5f, 0f, 0f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f)};
                break;

            // Cube 6 : �Ͼ��
            case 5:
                color = new Color32(0, 255, 216, 255);
                emission = new Color32(255, 0, 178, 255);
                intensity = 3.0f;
                deltapos = new Vector3(0f, 0f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(-1.0f, 0.0f, 1.0f)};
                break;

            // Cube 7 : ���ֻ�
            case 6:
                color = new Color32(255, 95, 196, 255);
                emission = new Color32(2, 0, 255, 255);
                intensity = 3.0f;
                deltapos = new Vector3(0f, 0f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 0.0f, 2.0f)};
                break;

            // Cube 8 : ������
            case 7:
                color = new Color32(255, 255, 255, 255);
                emission = new Color32(228, 0, 0, 255);
                intensity = 3.0f;
                deltapos = new Vector3(-0.5f, -0.5f, -0.5f);
                blockmap = new Vector3[] {new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f)};
                break;
        }
        foreach (Vector3 block in blockmap) {
            CreateCube(nextTetracubeNode, deltapos + block, color, emission, intensity, layernum : 8);
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
        // Debug.Log(oldGameState + ", " + newGameState);
        oldGameState = newGameState;
        gameState = newGameState;
        Debug.Log(newGameState);
        TurnOffAllPanels();
        switch (newGameState)
        {
            case "start":
                startPanel.SetActive(true);
                gameObject.SetActive(false);
                break;
            case "game":
                gamePanel.SetActive(true);
                gameObject.SetActive(true);
                break;
            case "restart":
                gamePanel.SetActive(true);
                gameObject.SetActive(true);
                Restart();
                break;
            case "pause":
                pausePanel.SetActive(true);
                break;
            case "end":
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

    private bool coroutineBlock = false;

    public void RotateTetracube(int rotDir) {
        if (coroutineBlock == false) {
            StopCoroutine("RotateCoroutine");
            StartCoroutine("RotateCoroutine", rotDir);
        }
    }

    public IEnumerator RotateCoroutine(int rotDir)
    {
        float rotTime = 0.3f;
        bool dead = false;
        foreach (Transform node in tetracubeNode) {
            int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
            if (y < 0) {
                dead = true;
                break;
            }
        }
        
        Vector3 oldPos = tetracubeNode.transform.position;
        Quaternion oldRot = tetracubeNode.transform.rotation;
        float rotAngle = GameObject.Find("CameraBase").transform.eulerAngles.y;
        tetracubeNode.rotation = realRot;
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
        Quaternion newRot = tetracubeNode.transform.rotation;

        float timer = 0.0f;
        if (!CanShiftTo(tetracubeNode) || dead) {
            tetracubeNode.transform.position = oldPos;
            tetracubeNode.transform.rotation = oldRot;
            Quaternion realOldRot = oldRot;
            Quaternion pureRot;
            oldRot = aniTetracubeNode.transform.rotation;
            while (timer < rotTime) {
                timer += Time.deltaTime;
                float percentageComplete = timer / rotTime;
                pureRot = Quaternion.Slerp(oldRot, realOldRot, easyOutCurve.Evaluate(percentageComplete));
                aniTetracubeNode.transform.rotation = Quaternion.Slerp(pureRot, newRot, brrrCurve.Evaluate(percentageComplete));
                // Debug.Log(aniTetracubeNode.transform.rotation);
                yield return null;
            }
            aniTetracubeNode.transform.rotation = realOldRot;
            yield break;

        }
        if (CanBeDeadAt(tetracubeNode))
        {
            foreach (Transform node in tetracubeNode) {
                int y = Mathf.RoundToInt(node.transform.position.y - 0.5f);
                if (y < 0) {
                    DamageBy(node);
                    DamageBy(aniTetracubeNode.Find(node.name));
                    coroutineBlock = true;
                }
            }
        }

        realRot = newRot;
        oldRot = aniTetracubeNode.transform.rotation;
        while (timer < rotTime) {
            timer += Time.deltaTime;
            float percentageComplete = timer / rotTime;
            aniTetracubeNode.transform.rotation = Quaternion.Slerp(oldRot, newRot, easyOutCurve.Evaluate(percentageComplete));
            // Debug.Log(aniTetracubeNode.transform.rotation);
            yield return null;
        }
        aniTetracubeNode.transform.rotation = newRot;
        coroutineBlock = false;
   }
}