using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTower : MonoBehaviour 
{
	[SerializeField]private MeshFilter m_wallCache;

	public bool IsSelected
	{
		get
		{
			return m_selected;
		}
	}

	private bool m_selected = false;

	public virtual void Select(bool value)//Virtual нужен для того, чтобы с помощью Override можно было расширить эту функцию через дочерние классы, может быть полезно, если нужно добавить эффект свечения, обводки и т.д.
	{
		m_selected = value;
	}

	public virtual void BuildWalls(Vector3 to, Mesh meshData, float distance) //Virtual нужен для того, чтобы с помощью Override можно было расширить эту функцию через дочерние классы, может быть полезно, если нужно добавить эффект строительства и т.д.
	{
		m_wallCache.mesh = meshData;
		m_wallCache.transform.LookAt(to);
		//m_wallCache.transform.localPosition = Vector3.forward * (distance/2);
	}

	public virtual void ClearMesh()
	{
		m_wallCache.mesh.Clear();
	}
}
