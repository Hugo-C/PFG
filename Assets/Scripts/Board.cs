using System;
using System.Collections;
using System.ComponentModel;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour {
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

    public bool isRunning;
    
    private Cell[,] map;
    private GameObject[,] gameObjectMap;

    private void Start() {
        map = new Cell[width, height];
        gameObjectMap = new GameObject[width, height];
        for (int x = 0; x <= map.GetUpperBound(0); x++) {
            for (int y = 0; y <= map.GetUpperBound(1); y++) {
                map[x, y] = Cell.Empty;
                ChangeGameObject(map, x, y);
            }
        }


        isRunning = true;
        StartCoroutine(RunningAutomata());
    }

    private IEnumerator RunningAutomata() {
        yield return new WaitForSeconds(secondsBetweenSteps);
        int stepCount = 0;
        while (isRunning) {
            map = Step();
            stepCount++;
            Debug.Log("Step : " + stepCount);
            yield return new WaitForSeconds(secondsBetweenSteps);
        }
    }

    private void ChangeGameObject(Cell[,] mapToUse, int x, int y) {
        var prevGo = gameObjectMap[x, y];
        if(prevGo != null)
            Destroy(prevGo);
            
        GameObject prefabToInstantiate;
        switch (mapToUse[x, y]) {
            case Cell.Tree:
                prefabToInstantiate = treeCellPrefab;
                break;
            case Cell.Empty:
                prefabToInstantiate = emptyCellPrefab;
                break;
            case Cell.Burning:
                prefabToInstantiate = burningCellPrefab;
                break;
            default:
                Debug.LogError("unknown cell state");
                prefabToInstantiate = burningCellPrefab;
                break;
        }

        var go = Instantiate(prefabToInstantiate, new Vector3(x, y), Quaternion.identity);
        gameObjectMap[x, y] = go;
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
}