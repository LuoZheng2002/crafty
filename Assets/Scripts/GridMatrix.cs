using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridMatrix : MonoBehaviour
{
	public bool first_person = true;
	public float camera_move_speed = 5.0f;
	public float camera_rotation_time = 0.5f;
	public GameObject gridPrefab;
	public int height = 2;
	public int width = 3;
	public int length = 5;
	GridCell[,,] grids;
	public float position_spring = 1000.0f;
	public float position_damper = 1000.0f;

	PiggyPreview piggyPreview;

	CratePreview[,,] crates;
	AccessoryPreview[,,] accessories;
	LoadPreview[,,] loads;

	// Util.GridContentInfo[,,] infos;

	GridCell lastSelectedGrid = null;
	public int activeLayerIndex = 0;
	private RaycastHit[] hits = new RaycastHit[10];

	public event Action<GridCell> GridSelectionChanged;

	public Util.ContentType currentContentType;
	void SetLayerActive(int index, bool active)
	{		
		if (index >= height)
		{
			Debug.LogWarning("Layer out of bound");
		}
		for(int i = 0; i < width; i++)
		{
			for (int j = 0; j < length; j++)
			{
				grids[index, i, j].SetActive(active);
			}
		}
	}
	
	bool Occupied(int height, int width, int length)
	{
		return crates[height, width, length] != null || accessories[height, width, length] != null || loads[height, width, length] != null;
	}
	bool AllowCrate(int height, int width, int length)
	{
		return crates[height, width, length] == null && accessories[height, width, length] == null;
	}
	bool AllowLoad(int height, int width, int length)
	{
		return loads[height, width, length] == null && accessories[height, width, length] == null;
	}

	Subscription<TrashEvent> subscriptionTrash;
	Subscription<GameStateChangedEvent> subscriptionGameStateChanged;

	private void OnEnable()
	{
		Debug.Log("Grid matrix enabled");
		subscriptionTrash = EventBus.Subscribe<TrashEvent>(OnTrash);
		subscriptionGameStateChanged = EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);

		grids = new GridCell[height, width, length];
		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];
		// infos = new Util.GridContentInfo[height, width, length];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					Debug.Assert(gridPrefab != null);
					GameObject grid = Instantiate(gridPrefab, transform.position + new Vector3(j, i, k), Quaternion.identity);
					grid.transform.parent = transform;
					Debug.Assert(grid != null);
					GridCell gridComponent = grid.GetComponent<GridCell>();
					Debug.Assert(gridComponent != null);
					gridComponent.heightIdx = i;
					gridComponent.widthIdx = j;
					gridComponent.lengthIdx = k;
					grids[i, j, k] = gridComponent;
				}
			}
		}
		SetLayerActive(activeLayerIndex, true);
	}
	private void OnDisable()
	{
		Debug.Log("Grid Matrix disabled");
		EventBus.Unsubscribe(subscriptionTrash);
		Trash();
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					GridCell grid = grids[i, j, k];
					Debug.Assert(grid != null);
					Destroy(grid.gameObject);
				}
			}
		}		
	}
	private void Start()
	{
		
	}
	
	void BuildAndStickCrates(int h_idx, int w_idx, int l_idx)
	{
		CratePreview crate = crates[h_idx, w_idx, l_idx];
		if (crate != null)
		{
			crate.Build();
			List<(int, int, int)> deltas = new(){ (1, 0, 0), (0, 1, 0), (0, 0, 1) };
			foreach (var delta in deltas)
			{
				(int h, int w, int l) = delta;
				(int new_h, int new_w, int new_l) = (h_idx+h, w_idx + w, l_idx+l);
				if (InGrid(new_h, new_w, new_l) && crates[new_h, new_w, new_l]!=null)
				{
					Util.CreateJoint(crate, crates[new_h, new_w, new_l], position_spring, position_damper);
				}
			}
		}	
	}
	void BuildAndStickAccessories(int h_idx, int w_idx, int l_idx)
	{
		AccessoryPreview accessory = accessories[h_idx, w_idx, l_idx];
		if (accessory != null)
		{
			accessory.Build();
			// to do
			(var h, var w, var l) = accessory.AttachDir();
			(var new_h, var new_w, var new_l) = (h + h_idx, w + w_idx, l + l_idx);
			if (InGrid(new_h, new_w, new_l) && crates[new_h, new_w, new_l] != null)
			{
				Util.CreateJoint(accessory, crates[new_h, new_w, new_l], position_spring, position_damper);
			}
		}
	}
	void BuildAndStickLoads(int h_idx, int w_idx, int l_idx)
	{
		LoadPreview load = loads[h_idx, w_idx, l_idx];
		if (load != null)
		{
			load.Build();
			// to do
			if (crates[h_idx, w_idx, l_idx] != null)
			{
				Util.CreateJoint(load, crates[h_idx, w_idx, l_idx], position_spring, position_damper);
			}
		}
	}
	void Build()
	{
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickCrates(i, j, k);
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickAccessories(i, j, k);
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickLoads(i, j, k);
				}
			}
		}

		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];

		enabled = false;
	}
	// build -> play -> end
	
	void OnTrash(TrashEvent e)
	{
		Trash();
	}
	void Trash()
	{
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					if (crates[i, j, k] != null)
					{
						Destroy(crates[i, j, k].gameObject);
						crates[i, j, k] = null;
					}
					if (loads[i, j, k] != null)
					{
						Destroy(loads[i, j, k].gameObject);
						loads[i, j, k] = null;
					}
					if (accessories[i, j, k] != null)
					{
						Destroy(accessories[i , j, k].gameObject);
						accessories[i, j, k] = null;
					}
				}
			}
		}
	}
	bool InGrid(int h, int w, int l)
	{
		return h >= 0 && h < height && w >= 0 && w < width && l >= 0 && l < length;
	}
	void OnGameStateChanged(GameStateChangedEvent e)
	{
		if (e.state == Util.GameStateType.Play)
		{
			Build();
		}		
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetLayerActive(activeLayerIndex, false);
			activeLayerIndex = (activeLayerIndex + 1) % height;
			SetLayerActive(activeLayerIndex, true);
		}
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			Trash();
		}
		UpdateActiveFromMouse();

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int hitCount = Physics.RaycastNonAlloc(ray, hits);
			GridCell closestGrid = null;
			float minDistance = 100000.0f;
			for (int i = 0; i < hitCount; i++)
			{
				GameObject hitObject = hits[i].collider.gameObject;
				GridCell hitGrid = hitObject.GetComponent<GridCell>();
				if (hitGrid == null)
				{
					continue;
				}
				// grid -> frame/accessory -> contained object
				// multiple grid accessory?
				if (!hitGrid.Active)
				{
					continue;
				}
				if (accessories[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
					&& loads[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null)
				{
					continue;
				}

				float distance = Util.GetDistanceFromRayToPoint(ray, hitObject.transform.position);
				if (distance < minDistance)
				{
					closestGrid = hitGrid;
					minDistance = distance;
				}
			}
			if (closestGrid!= null)
			{
				Debug.Log("Capture a component to change direction");
				DirectionalPreview directionalPreview = null;
				if (accessories[closestGrid.heightIdx, closestGrid.widthIdx, closestGrid.lengthIdx] != null)
				{
					directionalPreview = accessories[closestGrid.heightIdx, closestGrid.widthIdx, closestGrid.lengthIdx];
				}
				else if (loads[closestGrid.heightIdx, closestGrid.widthIdx, closestGrid.lengthIdx] != null)
				{
					directionalPreview = loads[closestGrid.heightIdx, closestGrid.widthIdx, closestGrid.lengthIdx];
				}
				Debug.Assert(directionalPreview != null);
				directionalPreview.ChangeDirection();
			}
		}
	}

	void UpdateActiveFromMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		int hitCount = Physics.RaycastNonAlloc(ray, hits);
		GridCell closestGrid = null;
		float minDistance = 100000.0f;
		for (int i = 0; i < hitCount; i++)
		{
			GameObject hitObject = hits[i].collider.gameObject;
			GridCell hitGrid = hitObject.GetComponent<GridCell>();
			if (hitGrid == null)
			{
				continue;
			}
			// grid -> frame/accessory -> contained object
			// multiple grid accessory?
			if (!hitGrid.Active)
			{
				continue;
			}
			if ((currentContentType == Util.ContentType.None || currentContentType == Util.ContentType.Accessory) && Occupied(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (currentContentType == Util.ContentType.Crate && !AllowCrate(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (currentContentType == Util.ContentType.Load && !AllowLoad(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			
			float distance = Util.GetDistanceFromRayToPoint(ray, hitObject.transform.position);
			if (distance < minDistance)
			{
				closestGrid = hitGrid;
				minDistance = distance;
			}
		}
		// !=, !=, ==
		if (closestGrid != null && lastSelectedGrid != null && lastSelectedGrid == closestGrid)
		{
			// 
		}
		else if (closestGrid != null && lastSelectedGrid != null && lastSelectedGrid != closestGrid)
		{
			lastSelectedGrid.Deselect();
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			GridSelectionChanged?.Invoke(closestGrid);
			// fire event
		}
		else if (closestGrid != null && lastSelectedGrid == null)
		{
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			GridSelectionChanged?.Invoke(closestGrid);
		}
		else if (closestGrid == null && lastSelectedGrid != null)
		{
			lastSelectedGrid.Deselect();
			lastSelectedGrid = null;
			GridSelectionChanged?.Invoke(null);
		}
		else // ==null both
		{
			
		}
	}
	public void AddContent(Util.ContentType contentType, ContentPreview content)
	{
		Debug.Assert(content != null);
		Debug.Assert(lastSelectedGrid != null);
		(var h, var w, var l) = (lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx);
		switch (contentType)
		{
			case Util.ContentType.Crate:
				CratePreview cratePreview = content as CratePreview;
				Debug.Assert(cratePreview != null);
				crates[h, w, l] = cratePreview;
				break;
			case Util.ContentType.Accessory:
				AccessoryPreview accessoryPreview = content as AccessoryPreview;
				Debug.Assert(accessoryPreview != null);
				accessories[h, w, l] = accessoryPreview;
				break;
			case Util.ContentType.Load:
				LoadPreview loadPreview = content as LoadPreview;
				Debug.Assert(loadPreview != null);
				loads[h, w, l] = loadPreview;
				PiggyPreview preview = loadPreview as PiggyPreview;
				if (preview != null)
				{
					piggyPreview = preview;
					EventBus.Publish(new PiggyInstantiatedEvent(piggyPreview));
				}
				break;
		}
	}
}