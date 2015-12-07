﻿using UnityEngine;
using System.Collections;
using System.IO;

public class PlanetPaint : MonoBehaviour {
	public Transform planet;
	private Texture2D tex;
	private Camera cam;
	private PlanetTerrain ps;
	private float[] brush;

	void Start () {
		tex = new Texture2D(512,512);
		
		var c = new Color(0f,0f,0f,1f);
		var nc = new Color[262144];
		for (var i=0; i<262144; i++) {
			nc[i] = c;
		}
		tex.SetPixels(nc);

		cam = gameObject.GetComponent<Camera>();
		ps = planet.GetComponent<PlanetTerrain>();
		brush = new float[1024];
		for (var i=0; i<1024; i++) {
			var x = i%32;
			var y = Mathf.Floor(i/32f);
			var dist = Vector2.Distance(new Vector2(x-16,y-16), Vector2.zero);
			brush[i] = Mathf.Clamp((16f-dist)/16f, 0f, 1f);
		}
		ps.planet.displacementLayers = new DisplacementLayer[]{
			new DisplacementLayer()
		};
		var l = ps.planet.displacementLayers[0];
		l.noise = NOISE.Texture;
		l.height = 50f;
		l.texture = tex;
	}

	void Update () {
		if (Input.GetMouseButton(0)) {
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, cam.farClipPlane)) {
				//Debug.Log(hit.point);
				var tc = PTHelpers.GetHeightmapCoord(hit.point);
				var x = (int)Mathf.Clamp(Mathf.Floor(tc.x*512) - 16, 0f, 480f);
				var y = (int)Mathf.Clamp(Mathf.Floor(tc.y*512) - 16, 0f, 480f);
				var cur = tex.GetPixels(
					x,
					y,
					32,
					32);
				for (var i=0; i<1024; i++) {
					cur[i].r = cur[i].g = cur[i].b = Mathf.Clamp(cur[i].r+brush[i], 0f, 1f);
				}
				tex.SetPixels(
					x,
					y,
					32,
					32,
					cur);
				byte[] bytes = tex.EncodeToPNG();		
				File.WriteAllBytes(Application.dataPath + "/PlanetTex.png", bytes);
				ps.UpdateTerrain();
			}
		}
	}
}