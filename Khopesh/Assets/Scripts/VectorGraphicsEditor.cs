using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof (VectorGraphics))]
public class VectorGraphicsEditor : Editor {
	
	static float scaleMultiplier = 1.0f;
	static Vector3 jitterAmount = new Vector3(0.5f, 0.5f, 0);

	string error = "";

    public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		var vg = (VectorGraphics)target;

		if (vg.Line != null) {
			if (vg.Line.color != vg.Color) {
				vg.Line.color = vg.Color;
			}
		}

		serializedObject.Update();
		GUILayout.Space(16);

//		var color = serializedObject.FindProperty("color");
//		color.colorValue = new Color(1, 1, 1, 1);

		if (!string.IsNullOrEmpty(error)) {
			var prevColor = GUI.contentColor;
			GUI.contentColor = Color.red;
			GUILayout.Label(error);
			GUI.contentColor = prevColor;
		}

		if (!UnityEditor.EditorApplication.isPlaying) {

			var points = serializedObject.FindProperty("points");

			GUILayout.Space(8);
			GUILayout.Label("Import/Export Points", EditorStyles.boldLabel);
			if (GUILayout.Button("Import Collider Points")) {
				PolygonCollider2D poly;
				BoxCollider2D box;
				if ((poly = vg.GetComponent<PolygonCollider2D>()) != null) {
					error = "";
					points.ClearArray();

					for (int i = 0; i < poly.points.Length; i++) {
						int index = i % poly.points.Length;

						points.InsertArrayElementAtIndex(i);
						var item = poly.points[index];
						var point = points.GetArrayElementAtIndex(i);
						point.vector3Value = item.ToVector3();
					}						
				} else if ((box = vg.GetComponent<BoxCollider2D>()) != null) {
					error = "";
					points.ClearArray();

					Vector2[] positions = new Vector2[] { 
						box.offset - 0.5f * box.size, 
						box.offset + 0.5f * new Vector2(box.size.x, -box.size.y),
						box.offset + 0.5f * box.size,
						box.offset + 0.5f * new Vector2(-box.size.x, box.size.y) };

					for (int i = 0; i < positions.Length; i++) {
						int index = i % positions.Length;

						points.InsertArrayElementAtIndex(i);
						var point = points.GetArrayElementAtIndex(i);
						point.vector3Value = positions[index].ToVector3();
					}
				} else {
					error = "No Polygon or Box collider found.";
				}
			}

			if (GUILayout.Button("Create or Update Polygon Collider")) {
				PolygonCollider2D poly;
				if ((poly = vg.GetComponent<PolygonCollider2D>()) == null) {
					poly = vg.gameObject.AddComponent<PolygonCollider2D>();
				}

				poly.pathCount = 1;
				var positions = new Vector2[points.arraySize];
				for (int i = 0; i < points.arraySize; i++) {
					positions[i] = (Vector2)points.GetArrayElementAtIndex(i).vector3Value;
				}	
				poly.SetPath(0, positions);
			}
		}

		GUILayout.Space(8);
		GUILayout.Label("Edit Scale", EditorStyles.boldLabel);
		scaleMultiplier = EditorGUILayout.FloatField("Multiplier", scaleMultiplier);
		if (GUILayout.Button("Scale Points by Multiplier")) {
			var points = serializedObject.FindProperty("points");
			for (int i = 0; i < points.arraySize; i++) {
				var point = points.GetArrayElementAtIndex(i);
				point.vector3Value *= scaleMultiplier;
			}
		}

		GUILayout.Space(8);
		GUILayout.Label("Add/Remove Vertices", EditorStyles.boldLabel);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Vertices")) {
			var points = serializedObject.FindProperty("points");
			for (int i = 0; i < points.arraySize; i += 2) {
				points.InsertArrayElementAtIndex(i + 1);
				var point = points.GetArrayElementAtIndex(i + 1);
				var start = points.GetArrayElementAtIndex(i);
				var end = points.GetArrayElementAtIndex((i + 2) % points.arraySize);
				point.vector3Value = 0.5f * (start.vector3Value + end.vector3Value);
			}
		}
		if (GUILayout.Button("Remove Vertices")) {
			var points = serializedObject.FindProperty("points");
			for (int i = points.arraySize - 1; i >= 0; i -= 2) {
				points.DeleteArrayElementAtIndex(i);
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(8);
		GUILayout.Label("Add Jitter", EditorStyles.boldLabel);
		jitterAmount = ElevenTools.Abs(EditorGUILayout.Vector3Field("Amount", jitterAmount));
		if (GUILayout.Button("Apply Jitter")) {
			var points = serializedObject.FindProperty("points");
			for (int i = 0; i < points.arraySize; i ++) {
				var point = points.GetArrayElementAtIndex(i);
				point.vector3Value += new Vector3(
					Random.Range(-jitterAmount.x, jitterAmount.x), 
					Random.Range(-jitterAmount.y, jitterAmount.y), 
					Random.Range(-jitterAmount.z, jitterAmount.z));
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
