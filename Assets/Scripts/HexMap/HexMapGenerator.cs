using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(HexGrid))]
public class HexMapGenerator : MonoBehaviour {

    public enum WorldSize {
        Small = 0,
        Mediam,
        Large
    }

    public string worldSeed = "Survios";
    public bool useRandomSeed = true;
    public WorldSize worldSize = WorldSize.Mediam;

    [Range(0, 100)] public int randomFillPercent;
    [Range(1, 10)] public int smoothTimes;
    HexGrid hexGrid;
    int[,] map;
    int width = 0, height = 0;
    void Awake() {
        hexGrid = GetComponent<HexGrid>();

    }

    void Start() {
        GenerateMap();
    }

    public void GenerateMap() {
        switch (worldSize) {
            case WorldSize.Mediam:
                width = 50;
                height = 50;
                break;
            case WorldSize.Large:
                width = 80;
                height = 80;
                break;
            case WorldSize.Small:
            default:
                width = 30;
                height = 30;
                break;
        }
        System.Random pseudoRandom = null;
        if (useRandomSeed)
            pseudoRandom = new System.Random(DateTime.Now.Millisecond);
        else
            pseudoRandom = new System.Random(worldSeed.GetHashCode());
        map = new int[width, height];

        // generate walls and random pillars
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1) {
                    map[x, y] = HexMetrics.maxElevation;
                } else {
                    map[x, y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? HexMetrics.maxElevation : HexMetrics.minElevation;
                }
            }
        }

        // smooth
        for (int i = 0; i < smoothTimes; i++) {
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);
                    if (neighbourWallTiles > 4)
                        map[x, y] = HexMetrics.maxElevation;
                    else if (neighbourWallTiles < 4)
                        map[x, y] = HexMetrics.minElevation;
                }
            }
        }

        ApplyToHexTerrain();
    }

    void ApplyToHexTerrain() {
        var cells = hexGrid.GetCells();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var cell = cells[width * x + y];
                cell.Elevation = (map[x, y] > 0) ? HexMetrics.maxElevation : HexMetrics.minElevation;
                switch (cell.Elevation) {
                    case 0:
                        cell.WaterLevel = 2;
                        break;
                    case 1:
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                    case 6:
                    case 7:
                    default:
                        break;
                }
                if (cell.Elevation > 3)
                    cell.TerrainTypeIndex = 4;
                else
                    cell.TerrainTypeIndex = cell.Elevation;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY) {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++) {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++) {

                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
                    if (neighbourX != gridX || neighbourY != gridY) {
                        wallCount += map[neighbourX, neighbourY] / HexMetrics.maxElevation;
                    }
                } else {
                    wallCount++;
                }

            }
        }

        return wallCount;
    }

    //private void OnDrawGizmos() {
    //    if (map != null) {
    //        var cells = hexGrid.GetCells();

    //        for (int x = 0; x < width; x++) {
    //            for (int y = 0; y < height; y++) {
    //                Gizmos.color = (map[x, y] > 0) ? Color.black : Color.white;
    //                //Vector3 pos = new Vector3(-width / 2 + x + .5f, 0, -height / 2 + y + .5f);
    //                var cell = cells[width * x + y];
    //                Gizmos.DrawCube(cell.transform.position, Vector3.one * 10);
    //            }
    //        }
    //    }
    //}
}


