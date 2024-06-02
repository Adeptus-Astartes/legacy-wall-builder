using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour 
{

	public List<Tower> Towers;
	[HideInInspector]
	public Transform SelectedWall;
	public Wall WallObject;

	public LayerMask Mask;

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
		ClearWall();
		for(int i = 0; i<Towers.Count; i++)
		{
			//float distance = 0;
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
			count = Mathf.FloorToInt(t_distance/WallObject.Lenght)/2;

			Towers[i].BuildWalls(nextTower,count,WallObject);
		}
	}

	private void ClearWall()
	{
		Towers.ForEach(x => x.MyWalls.ForEach(y => Destroy(y)));
		Towers.ForEach(x => x.MyWalls.Clear());
	}


}
