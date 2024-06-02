using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWallGenerator : MonoBehaviour 
{
	public List<ProceduralTower> Towers;
	[HideInInspector]public Transform SelectedWall;
	public LayerMask Mask;

	[Range(0.2f,3f)] public float Height = 1f;
	[Range(0.2f,3f)] public float Width = 1f;
	public float SectionSize = 1f;

	private bool m_moving = false;

	void Start()
	{
		BuildWall();
	}

	void Update()
	{
		if(!m_moving)
		{
			if(Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(Physics.Raycast(ray, out hit, 100))
				{
					if(hit.collider.tag == "Tower")
					{
						ClearWall();
						Towers.ForEach(x => x.Select(false));
						SelectedWall = hit.transform;
						hit.collider.SendMessage("Select",true);
						m_moving = true;
					}
					else
					{
						SelectedWall = null;
						Towers.ForEach(x => x.Select(false));
						m_moving = false;
					}
				}
			}
		}
		else
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100,Mask))
			{
				if(SelectedWall != null)
				{
					SelectedWall.position = hit.point;
				}
			}
			if(Input.GetMouseButtonDown(0))
			{
				BuildWall();
				m_moving = false;
			}
		}
	}

	public void BuildWall()
	{
		if(SectionSize == 0)
		{
			Debug.LogError("Set Valid Section Size");
			return;
		}
		
		ClearWall();
		for(int i = 0; i<Towers.Count; i++)
		{
			Vector3 currentTower = Towers[i].transform.position;
			Vector3 nextTower;

			if((i+1) < Towers.Count){
				nextTower = Towers[i+1].transform.position;
			}else{
				nextTower = Towers[0].transform.position;
			}
			var t_heading = currentTower - nextTower;
			var t_distance = t_heading.magnitude;
			var t_direction = t_heading / t_distance;

			int count = 0;
			count = Mathf.FloorToInt(t_distance/SectionSize);
			Debug.Log(i + " Count : " + count);

			Towers[i].BuildWalls(nextTower,Wall(Vector3.right * Width, Vector3.forward * SectionSize, Vector3.up * Height,count,t_distance),t_distance);
		}
	}

	private void ClearWall()
	{
		Towers.ForEach(x => x.ClearMesh());
	}

	#region Primitives

	public static Mesh Wall(Vector3 width, Vector3 length, Vector3 height, int count, float totalLenght)
	{
		var combine = new CombineInstance[count];

		for(int i = 0; i<count; i++)
		{
			if(i + 3 < count)
			{
				combine[i].mesh = Cube(Vector3.forward * (length.z / 2) + Vector3.forward * (i * length.z) + height/2, width, length, height);
			}
			/*else
			{
				float rest = (totalLenght - (count * length.z));
				Debug.LogError("REST : " + rest);
				combine[i].mesh = Cube(Vector3.forward * (1f / 2) + Vector3.forward * (i * 1f) + height/2, width, Vector3.forward * 1f, height);
			}*/
		}

		var mesh = new Mesh();
		mesh.CombineMeshes(combine, true, false);
		return mesh;
	}

	public static Mesh Cube(Vector3 origin, Vector3 width, Vector3 length, Vector3 height)
	{
		var corner0 = origin + (-width/2 - length/2 - height/2);
		var corner1 = origin + ( width/2 + length/2 + height/2);

		var combine = new CombineInstance[3];
		combine[0].mesh = Quad(corner0, height, length);
		combine[1].mesh = Quad(corner1, -width, -length);
		combine[2].mesh = Quad(corner1, -length, -height);


		/*var combine = new CombineInstance[6];
		combine[0].mesh = Quad(corner0, length, width);
		combine[1].mesh = Quad(corner0, width, height);
		combine[2].mesh = Quad(corner0, height, length);
		combine[3].mesh = Quad(corner1, -width, -length);
		combine[4].mesh = Quad(corner1, -height, -width);
		combine[5].mesh = Quad(corner1, -length, -height);*/

		var mesh = new Mesh();
		mesh.CombineMeshes(combine, true, false);
		return mesh;
	}

	public static Mesh Triangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
	{
		var normal = Vector3.Cross((vertex1 - vertex0), (vertex2 - vertex0)).normalized;
		var mesh = new Mesh
		{
			vertices = new [] {vertex0, vertex1, vertex2},
			normals = new [] {normal, normal, normal},
			uv = new [] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)},
			triangles = new [] {0, 1, 2}
		};
		return mesh;
	}

	public static Mesh Quad(Vector3 origin, Vector3 width, Vector3 length)
	{
		var normal = Vector3.Cross(length, width).normalized;
		var mesh = new Mesh
		{
			vertices = new[] { origin, origin + length, origin + length + width, origin + width },
			normals = new[] { normal, normal, normal, normal },
			uv = new[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) },
			triangles = new[] { 0, 1, 2, 0, 2, 3}
		};
		return mesh;
	}

	public static Mesh Plane(Vector3 origin, Vector3 width, Vector3 length, int widthCount, int lengthCount)
	{
		var combine = new CombineInstance[widthCount * lengthCount];

		var i = 0;
		for (var x = 0; x < widthCount; x++)
		{
			for (var y = 0; y < lengthCount; y++)
			{
				combine[i].mesh = Quad(origin + width * x + length * y, width, length);
				i++;
			}
		}

		var mesh = new Mesh();
		mesh.CombineMeshes(combine, true, false);
		return mesh;
	}
		


	#endregion
}
