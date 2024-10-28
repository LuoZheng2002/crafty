using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public class GridMatrix : MonoBehaviour
{
	public int level_num = 0;
	public float camera_move_speed = 5.0f;
	public float camera_rotation_time = 0.5f;
	public GameObject gridPrefab;
	public int height = 2;
	public int width = 3;
	public int length = 5;
	public float position_spring = 1000.0f;
	public float position_damper = 1000.0f;
	public int activeLayerIndex = 0;
	bool active = false;
	bool Active
	{
		get { return active; }
		set {
			if (active != value)
			{
				active = value;
				if (active)
				{
					Activate();
				}
				else
				{
					Deactivate();
				}
			}
		}
	}
	GridCell[,,] grids;
	CrateComponent[,,] crates;
	AccessoryComponent[,,] accessories;
	LoadComponent[,,] loads;
	Util.Content[,,] mem_crates;
	Util.Content[,,] mem_accessories;
	Util.Content[,,] mem_loads;
	int[,,] accessory_directions;
	int[,,] load_directions;
	GridCell lastSelectedGrid = null;
	RaycastHit[] hits = new RaycastHit[10];
	public static GridCell SelectedGrid{get; set;}
	static Dictionary<int, GridMatrix> grid_matrices = new();
	/// <summary>
	/// The Grid Matrix corresponding to the current level
	/// </summary>
	public static GridMatrix Current
	{
		get {
			Debug.Assert(current != null, "Current Grid Matrix not set");
			return current;
		}
	}
	static GridMatrix current;
	/// <summary>
	/// Activate the GridMatrix corresponding to the level num, deactivate other GridMatrices, and update GridMatrix.Current
	/// </summary>
	public static void SelectGridMatrix(int level_num)
	{
		Debug.Assert(grid_matrices.ContainsKey(level_num));
		if (current != null)
		{
			current.Active = false;
		}
		current = grid_matrices[level_num];
		current.Active = true;
	}
	/// <summary>
	/// Deactivate GridMatrix.Current and set it to null
	/// </summary>
	public static void DeselectGridMatrix()
	{
		current.Active = false;
		current = null;
	}
	private void Start()
	{
		Transform cube = transform.Find("Cube");
		Destroy(cube.gameObject);

		Debug.Assert(!grid_matrices.ContainsKey(level_num));
		grid_matrices.Add(level_num, this);
	}
	private void OnDisable()
	{
		grid_matrices.Clear();
	}

	void SetLayerActive(int index, bool active)
	{		
		if (index >= height)
		{
			Debug.LogError("Layer out of bound");
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
	public void OnEraseEnd()
	{
		if (lastSelectedGrid != null)
		{
			var load = loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
			if (load != null)
			{
				DragImage.DragImages[load.Content].Count++;
				if (load.Content == Util.Content.Pig)
				{
					GameState.Inst.Piggy = null;
					ConfirmButton.Inst.OnPiggyRemoved();
				}
				Destroy(load.gameObject);
				loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
			}
			else
			{
				var accessory = accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
				if (accessory != null)
				{
					DragImage.DragImages[accessory.Content].Count++;
					Destroy(accessory.gameObject);
					accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
				}
				var crate = crates[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
				if (crate != null)
				{
					DragImage.DragImages[crate.Content].Count++;
					Destroy(crate.gameObject);
					crates[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
				}
			}

		}
	}

	void RebuildVehicle()
	{
		if (mem_accessories != null)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < length; k++)
					{
						if (mem_accessories[i, j, k] != Util.Content.None)
						{
							Util.Content content = mem_accessories[i, j, k];
							var inst = DragImage.DragImages[content].InstantiateContent(new Vector3(j, i, k), true, accessory_directions[i, j, k]) as AccessoryComponent;
							accessories[i, j, k] = inst;
							Debug.Assert(inst != null);
							DragImage.DragImages[content].Count--;
						}
						if (mem_loads[i, j, k] != Util.Content.None)
						{
							Util.Content content = mem_loads[i, j, k];
							var inst = DragImage.DragImages[content].InstantiateContent(new Vector3(j, i, k), true, load_directions[i, j, k]) as LoadComponent;
							loads[i, j, k] = inst;
							Debug.Assert(inst != null);
							if (content == Util.Content.Pig)
							{
								GameState.Inst.Piggy = inst as PiggyPreview;
								ConfirmButton.Inst.OnPiggyInstantiated();
							}
							DragImage.DragImages[content].Count--;
						}
						if (mem_crates[i, j, k] != Util.Content.None)
						{
							Util.Content content = mem_crates[i, j, k];
							var inst = DragImage.DragImages[content].InstantiateContent(new Vector3(j, i, k), true, 0) as CrateComponent;
							crates[i, j, k] = inst;
							Debug.Assert(inst != null);
							DragImage.DragImages[content].Count--;
						}
					}
				}
			}
		}
	}
	
	void Activate()
	{
		grids = new GridCell[height, width, length];
		crates = new CrateComponent[height, width, length];
		accessories = new AccessoryComponent[height, width, length];
		loads = new LoadComponent[height, width, length];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					Debug.Assert(gridPrefab != null);
					GameObject grid = Instantiate(gridPrefab, transform.position, Quaternion.identity);
					grid.transform.parent = transform;
					grid.transform.localPosition = new Vector3(j, i, k);
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
		Util.Delay(this, 5, RebuildVehicle);
	}
	void Deactivate()
	{
		// remove all grids
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

	void BuildAndStickCrates(int h_idx, int w_idx, int l_idx)
	{
		CrateComponent crate = crates[h_idx, w_idx, l_idx];
		if (crate != null)
		{
			GameState.Inst.Components.Add(crate);
			mem_crates[h_idx, w_idx, l_idx] = crate.Content;
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
	bool ws = false;
	bool ad = false;
	void BuildAndStickAccessories(int h_idx, int w_idx, int l_idx)
	{
		AccessoryComponent accessory = accessories[h_idx, w_idx, l_idx];
		if (accessory != null)
		{
			GameState.Inst.Components.Add(accessory);
			mem_accessories[h_idx, w_idx, l_idx] = accessory.Content;
			accessory_directions[h_idx, w_idx, l_idx] = accessory.Direction;
			(bool _wa, bool _sd) = accessory.GetWASD();
			if (_wa) ws = true;
			if (_sd) ad = true;
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
		LoadComponent load = loads[h_idx, w_idx, l_idx];
		if (load != null)
		{
			GameState.Inst.Components.Add(load);
			mem_loads[h_idx, w_idx, l_idx] = load.Content;
			load_directions[h_idx, w_idx, l_idx] = load.Direction;
			load.Build();
			if (crates[h_idx, w_idx, l_idx] != null)
			{
				Util.CreateJoint(load, crates[h_idx, w_idx, l_idx], position_spring, position_damper);
			}
		}
	}
	public void BuildAndDeactivate()
	{
		GameState.Inst.Components.Clear();
		ws = false;
		ad = false;

		mem_crates = new Util.Content[height, width, length];
		mem_accessories = new Util.Content[height, width, length];
		mem_loads = new Util.Content[height, width, length];
		accessory_directions = new int[height, width, length];
		load_directions = new int[height, width, length];

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
		crates = new CrateComponent[height, width, length];
		accessories = new AccessoryComponent[height, width, length];
		loads = new LoadComponent[height, width, length];
		Active = false;
		PlayButtonsDisplayer.Inst.UpdateWASD(ws, ad);
	}

	public void Dump()
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
	public void SwitchLayer()
	{
		SetLayerActive(activeLayerIndex, false);
		activeLayerIndex = (activeLayerIndex + 1) % height;
		SetLayerActive(activeLayerIndex, true);
	}
	private void Update()
	{
		if (!active)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SwitchLayer();
		}
		UpdateActiveFromMouse();

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int hitCount = Physics.RaycastNonAlloc(ray, hits);
			GridCell closestOccupiedGrid = null;
			GridCell closestEmptyGrid = null;
			float minOccupiedDistance = 100000.0f;
			float minEmptyDistance = 100000.0f;
			for (int i = 0; i < hitCount; i++)
			{
				GameObject hitObject = hits[i].collider.gameObject;
				GridCell hitGrid = hitObject.GetComponent<GridCell>();
				if (hitGrid == null)
				{
					continue;
				}
				if (!hitGrid.Active)
				{
					continue;
				}
				float distance = Util.GetDistanceFromRayToPoint(ray, hitObject.transform.position);
				if (accessories[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
					&& loads[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
					&& crates[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null)
				{
					if (distance < minEmptyDistance)
					{
						closestEmptyGrid = hitGrid;
						minEmptyDistance = distance;
					}
				}
				else
				{
					if (distance < minOccupiedDistance)
					{
						closestOccupiedGrid = hitGrid;
						minOccupiedDistance = distance;
					}
				}
			}
			
			if (closestOccupiedGrid!= null)
			{
				if (DragImage.Current!= null && DragImage.Current.contentType == Util.ContentType.Load
					&& accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] == null
					&& loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] == null)
				{
					Debug.Assert(crates[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null);
					SelectedGrid = closestOccupiedGrid;
					DragImage.Current.ClickPlace();
				}
				DirectionalComponent directionalPreview = null;
				if (accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
				{
					directionalPreview = accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
				}
				else if (loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
				{
					directionalPreview = loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
				}
				else
				{
					Debug.Log("Assert clicking on crate");
				}
				if (directionalPreview != null)
				{
					directionalPreview.ChangeDirection();
				}
			}
			else if (closestEmptyGrid != null)
			{
				if (DragImage.Current != null && DragImage.Current.Count > 0)
				{
					SelectedGrid = closestEmptyGrid;
					DragImage.Current.ClickPlace();
				}
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
			if ((DragImage.CurrentContentType == Util.ContentType.None || DragImage.CurrentContentType == Util.ContentType.Accessory) && Occupied(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (DragImage.CurrentContentType == Util.ContentType.Crate && !AllowCrate(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (DragImage.CurrentContentType == Util.ContentType.Load && !AllowLoad(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
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
			SelectedGrid = closestGrid;
		}
		else if (closestGrid != null && lastSelectedGrid == null)
		{
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			SelectedGrid = closestGrid;
		}
		else if (closestGrid == null && lastSelectedGrid != null)
		{
			lastSelectedGrid.Deselect();
			lastSelectedGrid = null;
			SelectedGrid = closestGrid;
		}
		else // ==null both
		{
			
		}
	}
	public void AddComponent(GridCell selectedGrid, Util.ContentType contentType, VehicleComponent content)
	{
		Eraser.Inst.OnPlacedAComponent();
		Trash.Inst.OnPlacedAComponent();
		Debug.Assert(content != null);
		GridCell grid = selectedGrid;
		(var h, var w, var l) = (grid.heightIdx, grid.widthIdx, grid.lengthIdx);
		switch (contentType)
		{
			case Util.ContentType.Crate:
				CrateComponent cratePreview = content as CrateComponent;
				Debug.Assert(cratePreview != null);
				crates[h, w, l] = cratePreview;
				break;
			case Util.ContentType.Accessory:
				AccessoryComponent accessoryPreview = content as AccessoryComponent;
				Debug.Assert(accessoryPreview != null);
				accessories[h, w, l] = accessoryPreview;
				break;
			case Util.ContentType.Load:
				LoadComponent loadPreview = content as LoadComponent;
				Debug.Assert(loadPreview != null);
				loads[h, w, l] = loadPreview;
				PiggyPreview preview = loadPreview as PiggyPreview;
				if (preview != null)
				{
					GameState.Inst.Piggy = preview;
					ConfirmButton.Inst.OnPiggyInstantiated();
				}
				break;
		}
	}
}