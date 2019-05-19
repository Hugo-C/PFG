using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour {

    // tags used to retrieve prefab from gameObject
    private const String EmptyCellTag = "undefined";
    private const String TreeCellTag = "treeCell";
    private const String BurningCellTag = "burningCell";

    public GameObject ui;
    public float yPlaneElevation;
    public GameObject terrain;
    
    public GameObject emptyCellPrefab;
    public GameObject treeCellPrefab;
    public GameObject burningCellPrefab;

    public int width;
    public int height;
    
    [Range(0f, 1f)]
    public float chanceTreeBurning;

    [Range(0f, 1f)]
    public float chanceTreeGrowing;
    
    [Range(0.01f, 10f)]
    public float secondsBetweenSteps;

    private bool isRunning;
    private MyUi myUi;

    private Transform centerTransform;
    private List<Transform> cornersTransforms;
    private Plane gamePlane;
    
    private Cell[,] map;
    private GameObject[,] gameObjectMap;
    
    // ObjectPoolers are used to add / remove gameobjects efficiently
    private ObjectPooler emptyCellObjectPooler;
    private ObjectPooler treeCellObjectPooler;
    private ObjectPooler burningCellObjectPooler;

    private void Awake() {
        emptyCellObjectPooler = new ObjectPooler(emptyCellPrefab);
        treeCellObjectPooler = new ObjectPooler(treeCellPrefab);
        burningCellObjectPooler = new ObjectPooler(burningCellPrefab);
        
        map = new Cell[width, height];
        gameObjectMap = new GameObject[width, height];
        InitTerrain();

        var planeElevation = new Vector3(0, yPlaneElevation, 0);
        gamePlane = new Plane(planeElevation, planeElevation + Vector3.right, planeElevation + Vector3.forward);
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                map[x, y] = Cell.Empty;
                ChangeGameObject(map, x, y);
            }
        }


        isRunning = true;
        if(ui != null)
            myUi = ui.GetComponent<MyUi>();
        StartCoroutine(RunningAutomata());
    }

    private IEnumerator RunningAutomata() {
        yield return new WaitForSeconds(secondsBetweenSteps);
        int stepCount = 0;
        while (true) {
            if (isRunning) {
                map = Step();
                stepCount++;
                if(myUi != null)
                    myUi.Step = stepCount;
            }
            yield return new WaitForSeconds(secondsBetweenSteps);
        }
    }

    [ContextMenu("TogglePause")]
    private void TogglePause() {
        isRunning = !isRunning;
    }
    
    /// <summary>
    /// Init a terrain with the terrain prefab
    /// </summary>
    private void InitTerrain() {
        if (terrain != null) {
            // TODO improve terrain gameobject pivot point
            var goTerrain = Instantiate(terrain, new Vector3(width * 2f, 0, height * 2f), terrain.transform.rotation);
            goTerrain.transform.localScale = new Vector3(width / 4.9f, 1f, height / 4.9f);
        }
    }

    private void ChangeGameObject(Cell[,] mapToUse, int x, int y) {
        var prevGo = gameObjectMap[x, y];
        if(prevGo != null)
            AddGameObjectToObjectPooler(prevGo);
            
        // retrieve a gameobject from the objectPooler
        ObjectPooler objectPooler; 
        switch (mapToUse[x, y]) {
            case Cell.Tree:
                objectPooler = treeCellObjectPooler;
                break;
            case Cell.Empty:
                objectPooler = emptyCellObjectPooler;
                break;
            case Cell.Burning:
                objectPooler = burningCellObjectPooler;
                break;
            default:
                Debug.LogError("unknown cell state");
                objectPooler = burningCellObjectPooler;
                break;
        }

        var go = objectPooler.GetObject();
        if (go != null) {
            go.transform.position = new Vector3(x * 2, 0, y * 2);
            gameObjectMap[x, y] = go;   
        }
    }

    private void AddGameObjectToObjectPooler(GameObject go) {
        // add the gameobject to the correct ObjectPooler
        switch (go.tag) {  
            case EmptyCellTag:
                emptyCellObjectPooler.AddObject(go);
                break;
            case TreeCellTag:
                treeCellObjectPooler.AddObject(go);
                break;
            case BurningCellTag:
                burningCellObjectPooler.AddObject(go);
                break;
            default:
                Debug.LogWarning("failed to add gameobject to pool, unknown tag found : " + go.tag);
                break;
        }
    }

    private Cell[,] Step() {
        var result = new Cell[width, height];
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                bool cellChanged = false;
                switch (map[x, y]) {
                    case Cell.Burning:
                        result[x, y] = Cell.Empty;
                        cellChanged = true;
                        break;
                    case Cell.Tree:
                        if (Random.Range(0f, 1f) < chanceTreeBurning) {
                            result[x, y] = Cell.Burning;
                            cellChanged = true;
                        } else if (IsNeighborsBurning(x, y)){
                            result[x, y] = Cell.Burning;
                            cellChanged = true;
                        }
                        break;
                    case Cell.Empty:
                        if (Random.Range(0f, 1f) < chanceTreeGrowing) {  // TODO use a single Random
                            result[x, y] = Cell.Tree;
                            cellChanged = true;
                        } 
                        break;
                    default:
                        Debug.LogError("unknown cell state");
                        break;
                }

                if (cellChanged) {
                    ChangeGameObject(result, x, y);
                } else {
                    result[x, y] = map[x, y];  // we keep the cell at it's previous state
                }
            }
        }
        return result;
    }

    private bool IsNeighborsBurning(int i, int j) {
        for (int x = Math.Max(i - 1, 0); x <= Math.Min(i + 1, map.GetUpperBound(0)); x++) {
            for (int y = Math.Max(j - 1, 0); y <= Math.Min(j + 1, map.GetUpperBound(1)); y++) {
                if ((x == i || y == j) && map[x, y] == Cell.Burning) // we exclude diagonal neighbors
                    return true;
            }
        }
        return false;
    }

    public Transform GetCenterTransform() {
        if (centerTransform == null) {
            var go = new GameObject("center");
            go.transform.position = new Vector3(width, 0, height);
            centerTransform = go.transform;
        }
        return centerTransform;
    }
    
    public List<Transform> GetCornersTransforms() {
        if (cornersTransforms == null) {
            cornersTransforms = new List<Transform>();
            var go1 = new GameObject("corner1");
            go1.transform.position = new Vector3(0, 0, 0);
            cornersTransforms.Add(go1.transform);
            var go2 = new GameObject("corner2");
            go2.transform.position = new Vector3(width * 2, 0, 0);
            cornersTransforms.Add(go2.transform);
            var go3 = new GameObject("corner3");
            go3.transform.position = new Vector3(width * 2, 0, height * 2);
            cornersTransforms.Add(go3.transform);
            var go4 = new GameObject("corner4");
            go4.transform.position = new Vector3(0, 0, height * 2);
            cornersTransforms.Add(go4.transform);
        }
        return cornersTransforms;
    }

    public bool ManualFire(Ray clickRay) {
        if (gamePlane.Raycast(clickRay, out var distance)) {
            var point = clickRay.GetPoint(distance);
            int x = (int) (point.x / 2);
            int y = (int) (point.z / 2);
            if (x >= 0 && x <= map.GetUpperBound(0) && y >= 0 && y <= map.GetUpperBound(1) 
                && map[x, y] == Cell.Tree) {
                map[x, y] = Cell.Burning;
                ChangeGameObject(map, x, y);
                return true;
            }
        }
        
        return false;  // we cannot set this point on fire
    }
}