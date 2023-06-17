using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Popcron;

[ExecuteInEditMode]
[AddComponentMenu("")]
public class GizmosInstance : MonoBehaviour
{
	private const int DefaultQueueSize = 4096;

	private static GizmosInstance instance;

	private static bool hotReloaded = true;

	private static Material defaultMaterial;

	private static Plane[] cameraPlanes = new Plane[6];

	private Material overrideMaterial;

	private int queueIndex;

	private int lastFrame;

	private Element[] queue = new Element[4096];

	public static Material Material
	{
		get
		{
			GizmosInstance orCreate = GetOrCreate();
			if ((bool)orCreate.overrideMaterial)
			{
				return orCreate.overrideMaterial;
			}
			return DefaultMaterial;
		}
		set
		{
			GetOrCreate().overrideMaterial = value;
		}
	}

	public static Material DefaultMaterial
	{
		get
		{
			if (!defaultMaterial)
			{
				defaultMaterial = new Material(Shader.Find("Hidden/Internal-Colored"))
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				defaultMaterial.SetInt("_SrcBlend", 5);
				defaultMaterial.SetInt("_DstBlend", 10);
				defaultMaterial.SetInt("_Cull", 0);
				defaultMaterial.SetInt("_ZWrite", 0);
			}
			return defaultMaterial;
		}
	}

	private float CurrentTime
	{
		get
		{
			float result = 0f;
			if (Application.isPlaying)
			{
				result = Time.time;
			}
			return result;
		}
	}

	internal static GizmosInstance GetOrCreate()
	{
		if (hotReloaded || !instance)
		{
			GizmosInstance[] array = UnityEngine.Object.FindObjectsOfType<GizmosInstance>();
			for (int i = 0; i < array.Length; i++)
			{
				instance = array[i];
				if (i > 0)
				{
					if (Application.isPlaying)
					{
						UnityEngine.Object.Destroy(array[i]);
					}
					else
					{
						UnityEngine.Object.DestroyImmediate(array[i]);
					}
				}
			}
			if (!instance)
			{
				instance = new GameObject(typeof(GizmosInstance).FullName).AddComponent<GizmosInstance>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
			}
			hotReloaded = false;
		}
		return instance;
	}

	internal static void Submit(Vector3[] points, Color? color, bool dashed)
	{
		GizmosInstance orCreate = GetOrCreate();
		if (orCreate.lastFrame != Time.frameCount)
		{
			orCreate.lastFrame = Time.frameCount;
			orCreate.queueIndex = 0;
		}
		if (orCreate.queueIndex >= orCreate.queue.Length)
		{
			Element[] array = new Element[orCreate.queue.Length + 4096];
			for (int i = orCreate.queue.Length; i < array.Length; i++)
			{
				array[i] = new Element();
			}
			Array.Copy(orCreate.queue, 0, array, 0, orCreate.queue.Length);
			orCreate.queue = array;
		}
		orCreate.queue[orCreate.queueIndex].color = color ?? Color.white;
		orCreate.queue[orCreate.queueIndex].points = points;
		orCreate.queue[orCreate.queueIndex].dashed = dashed;
		orCreate.queueIndex++;
	}

	private void OnEnable()
	{
		queue = new Element[4096];
		for (int i = 0; i < 4096; i++)
		{
			queue[i] = new Element();
		}
		if (GraphicsSettings.renderPipelineAsset == null)
		{
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Combine(Camera.onPostRender, new Camera.CameraCallback(OnRendered));
		}
		else
		{
			RenderPipelineManager.endCameraRendering += OnRendered;
		}
	}

	private void OnDisable()
	{
		if (GraphicsSettings.renderPipelineAsset == null)
		{
			Camera.onPostRender = (Camera.CameraCallback)Delegate.Remove(Camera.onPostRender, new Camera.CameraCallback(OnRendered));
		}
		else
		{
			RenderPipelineManager.endCameraRendering -= OnRendered;
		}
	}

	private void OnRendered(ScriptableRenderContext context, Camera camera)
	{
		OnRendered(camera);
	}

	private bool ShouldRenderCamera(Camera camera)
	{
		if (!camera)
		{
			return false;
		}
		if (false || camera.CompareTag("MainCamera"))
		{
			return true;
		}
		Func<Camera, bool> cameraFilter = Gizmos.CameraFilter;
		if (cameraFilter != null && cameraFilter(camera))
		{
			return true;
		}
		return false;
	}

	private bool IsVisibleByCamera(Element points, Camera camera)
	{
		if (!camera)
		{
			return false;
		}
		for (int i = 0; i < points.points.Length; i++)
		{
			Vector3 vector = camera.WorldToViewportPoint(points.points[i], camera.stereoActiveEye);
			if (vector.x >= 0f && vector.x <= 1f && vector.y >= 0f && vector.y <= 1f)
			{
				return true;
			}
		}
		return false;
	}

	private void Update()
	{
		Gizmos.Line(default(Vector3), default(Vector3));
	}

	private void OnRendered(Camera camera)
	{
		Material.SetPass(Gizmos.Pass);
		if (!Gizmos.Enabled)
		{
			queueIndex = 0;
		}
		if (!ShouldRenderCamera(camera))
		{
			GL.PushMatrix();
			GL.Begin(GL.TRIANGLES);
			GL.End();
			GL.PopMatrix();
			return;
		}
		Vector3 offset = Gizmos.Offset;
		GL.PushMatrix();
		GL.MultMatrix(Matrix4x4.identity);
		GL.Begin(GL.TRIANGLES);
		bool flag = CurrentTime % 1f > 0.5f;
		float num = Mathf.Clamp(Gizmos.DashGap, 0.01f, 32f);
		bool frustumCulling = Gizmos.FrustumCulling;
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < queueIndex && queue.Length > i; i++)
		{
			Element element = queue[i];
			if (frustumCulling && !IsVisibleByCamera(element, camera))
			{
				continue;
			}
			list.Clear();
			if (element.dashed)
			{
				for (int j = 0; j < element.points.Length - 1; j++)
				{
					Vector3 vector = element.points[j];
					Vector3 vector2 = element.points[j + 1];
					Vector3 vector3 = vector2 - vector;
					if (vector3.sqrMagnitude > num * num * 2f)
					{
						float magnitude = vector3.magnitude;
						int num2 = Mathf.RoundToInt(magnitude / num);
						vector3 /= magnitude;
						for (int k = 0; k < num2 - 1; k++)
						{
							if (k % 2 == (flag ? 1 : 0))
							{
								float t = (float)k / ((float)num2 - 1f);
								float t2 = (float)(k + 1) / ((float)num2 - 1f);
								Vector3 item = Vector3.Lerp(vector, vector2, t);
								Vector3 item2 = Vector3.Lerp(vector, vector2, t2);
								list.Add(item);
								list.Add(item2);
							}
						}
					}
					else
					{
						list.Add(vector);
						list.Add(vector2);
					}
				}
			}
			else
			{
				list.AddRange(element.points);
			}
			GL.Color(element.color);
			for (int l = 0; l < list.Count; l++)
			{
				GL.Vertex(list[l] + offset);
			}
		}
		GL.End();
		GL.PopMatrix();
	}
}
