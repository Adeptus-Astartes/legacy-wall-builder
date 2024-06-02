using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour 
{

	public List<GameObject> MyWalls;

	[SerializeField]private Transform m_wallCache;

	public bool IsSelected
	{
		get
		{
			return m_selected;
		}
	}

	private bool m_selected = false;

	public virtual void Select(bool value)
	{
		m_selected = value;
	}

	public virtual void BuildWalls(Vector3 to, int count, Wall wall)
	{
		var t_heading = transform.position - to;
		var t_distance = t_heading.magnitude;
		var t_direction = t_heading / t_distance;
	
		m_wallCache.LookAt(to);

		for(int j = 0; j<=count; j++)
		{
			GameObject g_wall = Instantiate(wall.gameObject,m_wallCache) as GameObject;
			//Wall t_wall = g_wall.GetComponent<Wall>();
			g_wall.transform.position = transform.position - (2) * t_direction * j;
			g_wall.transform.localEulerAngles = Vector3.up * 180;
			MyWalls.Add(g_wall);
		}

	}

}
