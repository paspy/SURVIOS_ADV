using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexMapGenerator))]
public class HexMapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (Application.isPlaying) {
            HexMapGenerator mapGen = (HexMapGenerator)target;
            if (GUILayout.Button("Generate New Map")) {
                mapGen.GenerateMap();
            }
        }
    }
}