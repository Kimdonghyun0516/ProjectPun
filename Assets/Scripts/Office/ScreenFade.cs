﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections; 
namespace ChiliGames.VROffice
{
	public class ScreenFade : MonoBehaviour
	{
		[Tooltip("Fade duration")]
		public float fadeTime = 2.0f;

		[Tooltip("Screen color at maximum fade")]
		public Color fadeColor = new Color(0.01f, 0.01f, 0.01f, 1.0f);

		public bool fadeOnStart = true;

		public UnityEvent onFadeIn;
		public UnityEvent onFadeOut;

		public int renderQueue = 5000;

		private float uiFadeAlpha = 0;

		private MeshRenderer fadeRenderer;
		private MeshFilter fadeMesh;
		[SerializeField] Material baseMaterial = null;
		private Material fadeMaterial;
		private bool isFading = false;

		public float currentAlpha { get; private set; }

		void Start()
		{
			fadeMaterial = new Material(baseMaterial);
			fadeMesh = gameObject.AddComponent<MeshFilter>();
			fadeRenderer = gameObject.AddComponent<MeshRenderer>();

			var mesh = new Mesh();
			fadeMesh.mesh = mesh;

			Vector3[] vertices = new Vector3[4];

			float width = 2f;
			float height = 2f;
			float depth = 0.2f;

			vertices[0] = new Vector3(-width, -height, depth);
			vertices[1] = new Vector3(width, -height, depth);
			vertices[2] = new Vector3(-width, height, depth);
			vertices[3] = new Vector3(width, height, depth);

			mesh.vertices = vertices;

			int[] tri = new int[6];

			tri[0] = 0;
			tri[1] = 2;
			tri[2] = 1;

			tri[3] = 2;
			tri[4] = 3;
			tri[5] = 1;

			mesh.triangles = tri;

			Vector3[] normals = new Vector3[4];

			normals[0] = -Vector3.forward;
			normals[1] = -Vector3.forward;
			normals[2] = -Vector3.forward;
			normals[3] = -Vector3.forward;

			mesh.normals = normals;

			Vector2[] uv = new Vector2[4];

			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(1, 0);
			uv[2] = new Vector2(0, 1);
			uv[3] = new Vector2(1, 1);

			mesh.uv = uv;

			SetFadeLevel(0);

			if (fadeOnStart)
			{
				StartCoroutine(Fade(1, 0));
				onFadeIn.Invoke();
			}
		}

		public void FadeOut()
		{
			StartCoroutine(Fade(0, 1));
			onFadeOut.Invoke();
		}

		void OnLevelFinishedLoading(int level)
		{
			StartCoroutine(Fade(1, 0));
		}

		void OnEnable()
		{
			if (!fadeOnStart)
			{
				SetFadeLevel(0);
			}
		}

		void OnDestroy()
		{
			if (fadeRenderer != null)
				Destroy(fadeRenderer);
			if (fadeMaterial != null)
				Destroy(fadeMaterial);

			if (fadeMesh != null)
				Destroy(fadeMesh);
		}

		/// <summary>
		/// Set the UI fade level - fade due to UI in foreground
		/// </summary>
		public void SetUIFade(float level)
		{
			uiFadeAlpha = Mathf.Clamp01(level);
			SetMaterialAlpha();
		}
		/// <summary>
		/// Override current fade level
		/// </summary>
		/// <param name="level"></param>
		public void SetFadeLevel(float level)
		{
			currentAlpha = level;
			SetMaterialAlpha();
		}

		/// <summary>
		/// Fades alpha from 1.0 to 0.0
		/// </summary>
		IEnumerator Fade(float startAlpha, float endAlpha)
		{
			float elapsedTime = 0.0f;
			while (elapsedTime < fadeTime)
			{
				elapsedTime += Time.deltaTime;
				currentAlpha = Mathf.Lerp(startAlpha, endAlpha, Mathf.Clamp01(elapsedTime / fadeTime));
				SetMaterialAlpha();
				yield return new WaitForEndOfFrame();
			}
		}

		private void SetMaterialAlpha()
		{
			Color color = fadeColor;
			color.a = Mathf.Max(currentAlpha, uiFadeAlpha);
			isFading = color.a > 0;
			if (fadeMaterial != null)
			{
				fadeMaterial.SetColor("_BaseColor", color);
				fadeMaterial.renderQueue = renderQueue;
				fadeRenderer.material = fadeMaterial;
				fadeRenderer.enabled = isFading;
			}
		}
	}
}