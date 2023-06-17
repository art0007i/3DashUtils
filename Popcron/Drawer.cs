using _3DashUtils.Popcron;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Popcron;

public abstract class Drawer
{
	private static Dictionary<Type, Drawer> typeToDrawer;

	public abstract int Draw(ref Vector3[] buffer, params object[] args);

	public Drawer()
	{
	}

	public static Drawer Get<T>() where T : class
	{
		if (typeToDrawer == null)
		{
			typeToDrawer = new Dictionary<Type, Drawer>();
			typeToDrawer.Add(typeof(CubeDrawer), new CubeDrawer());
			typeToDrawer.Add(typeof(LineDrawer), new LineDrawer());
			typeToDrawer.Add(typeof(PolygonDrawer), new PolygonDrawer());
			typeToDrawer.Add(typeof(SquareDrawer), new SquareDrawer());
			typeToDrawer.Add(typeof(SphereDrawer), new SphereDrawer());
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				Type[] types = assemblies[i].GetTypes();
				foreach (Type type in types)
				{
					if (!type.IsAbstract && type.IsSubclassOf(typeof(Drawer)) && !typeToDrawer.ContainsKey(type))
					{
						try
						{
							Drawer value = (Drawer)Activator.CreateInstance(type);
							typeToDrawer[type] = value;
						}
						catch (Exception ex)
						{
							Debug.LogError($"couldnt register drawer of type {type} because {ex.Message}");
						}
					}
				}
			}
		}
		if (typeToDrawer.TryGetValue(typeof(T), out var value2))
		{
			return value2;
		}
		return null;
	}
}
