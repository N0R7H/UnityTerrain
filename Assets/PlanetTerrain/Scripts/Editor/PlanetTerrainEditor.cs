﻿using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor {
	SerializedProperty segmentResolution;
	SerializedProperty minSubdivisions;
	SerializedProperty maxSubdivisions;
	SerializedProperty editorSubdivisions;
	SerializedProperty radius;
	SerializedProperty waterHeight;

	SerializedProperty mainMaterial;
	SerializedProperty waterMaterial;
	SerializedProperty waterMesh;

	SerializedProperty displacements;
	List<bool> displacementBools = new List<bool>();

	
	void OnEnable () {
		var r = serializedObject.FindProperty("planet");
		segmentResolution = r.FindPropertyRelative("segmentResolution");
		minSubdivisions = r.FindPropertyRelative("minSubdivisions");
		maxSubdivisions = r.FindPropertyRelative("maxSubdivisions");
		editorSubdivisions = r.FindPropertyRelative("editorSubdivisions");
		radius = r.FindPropertyRelative("radius");
		waterHeight = r.FindPropertyRelative("waterHeight");

		mainMaterial = r.FindPropertyRelative("mainMaterial");
		waterMaterial = r.FindPropertyRelative("waterMaterial");
		waterMesh = r.FindPropertyRelative("waterSphere");

		displacements = r.FindPropertyRelative("displacementLayers");
	}

	public override void OnInspectorGUI() {
		PlanetTerrain pt = (PlanetTerrain)target;
		serializedObject.Update();

		EditorGUILayout.IntSlider(segmentResolution, 2, 64);
		EditorGUILayout.IntSlider(minSubdivisions, 0, maxSubdivisions.intValue);
		EditorGUILayout.IntSlider(maxSubdivisions, 0, 8);
		EditorGUILayout.IntSlider(editorSubdivisions, 0, maxSubdivisions.intValue);
		EditorGUILayout.Slider(radius, 1f, 20000f, new GUIContent("Planet Radius"));
		EditorGUILayout.Slider(waterHeight, 0f, 500f);

		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(mainMaterial);
		EditorGUILayout.PropertyField(waterMaterial);
		EditorGUILayout.PropertyField(waterMesh);

		EditorGUILayout.Space();

		for (var i=0; i<displacements.arraySize; i++) {
			if (i >= displacementBools.Count) displacementBools.Add(false);
			var b = displacementBools[i] = EditorGUILayout.Foldout(displacementBools[i], "Displacement Layer "+(i+1).ToString());
			if(b) {
				var e = displacements.GetArrayElementAtIndex(i);
				EditorGUILayout.PropertyField(e.FindPropertyRelative("noise"));
				EditorGUILayout.Slider(e.FindPropertyRelative("seed"), 0f, 65536f);
				EditorGUILayout.Slider(e.FindPropertyRelative("height"), 0f, radius.floatValue);
				EditorGUILayout.Slider(e.FindPropertyRelative("detail"), 0f, radius.floatValue, new GUIContent("Noise Frequency"));
				EditorGUILayout.PropertyField(e.FindPropertyRelative("heightStrength"));
				EditorGUILayout.PropertyField(e.FindPropertyRelative("texture"));
			}
		}

		if (GUILayout.Button("New Displacement")) {
			displacements.InsertArrayElementAtIndex(0);
		}

		EditorGUILayout.Space();
		if (GUILayout.Button("Generate")) {
			pt.UpdateTerrain();
		}
		serializedObject.ApplyModifiedProperties();
	}
}
