using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(HexGrid))]
public class HexMapGenerator : MonoBehaviour {
    public DayNightShifting dayNight;
    public enum WorldSize { Small = 0, Mediam, Large }

    public string worldSeed = "Survios";
    public bool useRandomSeed = true;
    public WorldSize worldSize = WorldSize.Mediam;
    [Range(0, 100)]
    public int randomFillPercent = 35;
    [Range(1, 10)]
    public int smoothTimes = 5;

    System.Random pseudoRandom;
    HexGrid hexGrid;
    int[,] map;
    int width = 0, height = 0;
    void Awake() {
        hexGrid = GetComponent<HexGrid>();
        GenerateMap();
    }

    void Start() {

    }

    public void GenerateMap() {
        switch (worldSize) {
            default:
            case WorldSize.Small:
                width = 30;
                height = 30;
                break;
            case WorldSize.Mediam:
                width = 50;
                height = 50;
                break;
            case WorldSize.Large:
                width = 80;
                height = 80;
                break;
        }

        if (useRandomSeed) {
            var seed = DateTime.UtcNow;
            worldSeed = seed.ToString("yyyyMMddHHmmssffff");
        }
        pseudoRandom = new System.Random(worldSeed.GetHashCode());

        hexGrid.CreateMap(width, height);

        map = new int[width, height];

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
        CreateWorldBoundary();
        CreateTerrainFeatures();
        dayNight.RefreshStarPosition();
    }

    void CreateWorldBoundary() {
        var cells = hexGrid.GetCells();
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var cell = cells[width * x + y];
                cell.Elevation = (map[x, y] > 0) ? HexMetrics.maxElevation : HexMetrics.minElevation;
                if (cell.Elevation == 0)
                    cell.WaterLevel = 2;
            }
        }
    }

    void CreateTerrainFeatures() {
        var flatLandCells = hexGrid.GetCells().ToList().FindAll(x => x.Elevation < 9).ToList();

        //level 1
        foreach (var cell in flatLandCells) {
            cell.Elevation = (pseudoRandom.Next(0, 100) < randomFillPercent * 2.5f) ? 1 : 0;
        }
        flatLandCells = flatLandCells.FindAll(x => x.Elevation >= 1 && x.Elevation < 9).ToList();
        //level 2
        foreach (var cell in flatLandCells) {
            cell.Elevation = (pseudoRandom.Next(0, 100) < randomFillPercent * 2.5f) ? 2 : 1;
        }
        flatLandCells = flatLandCells.FindAll(x => x.Elevation >= 2 && x.Elevation < 9).ToList();

        //level 3
        foreach (var cell in flatLandCells) {
            cell.Elevation = (pseudoRandom.Next(0, 100) < randomFillPercent * 2.0f) ? 3 : 2;
        }
        flatLandCells = flatLandCells.FindAll(x => x.Elevation >= 3 && x.Elevation < 9).ToList();

        //level 4
        foreach (var cell in flatLandCells) {
            cell.Elevation = (pseudoRandom.Next(0, 100) < randomFillPercent * 1.5f) ? 4 : 3;
        }
        flatLandCells = flatLandCells.FindAll(x => x.Elevation >= 4 && x.Elevation < 9).ToList();

        //level 5
        foreach (var cell in flatLandCells) {
            cell.Elevation = (pseudoRandom.Next(0, 100) < randomFillPercent * 1.0f) ? 5 : 4;
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


