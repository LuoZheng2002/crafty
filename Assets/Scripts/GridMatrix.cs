using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemErasedEvent { }
public class ComponentAddedEvent { }
public class SwitchLayerEvent { }
public class FullLayerEvent { }
public class NeighborChangedEvent { }
public class GridMatrix : MonoBehaviour
{
	public Util.WaypointName waypoint_name;
	public float camera_move_speed = 5.0f;
	public float camera_rotation_time = 0.5f;
	public GameObject gridPrefab;
	public int height = 2;
	public int width = 3;
	public int length = 5;
	public float position_spring = 1000.0f;
	public float position_damper = 1000.0f;
	public int activeLayerIndex = 0;
	public float drag_rotation_speed = 0.05f;
	bool active = false;

	Transform cameraPivot;
	Transform dummyCamera;
	public Transform DummyCamera
	{
		get { Debug.Assert(dummyCamera != null); return dummyCamera; }
	}

	Subscription<GridMatrixDragEvent> dragEvent;
	public static GridMatrix Get(Util.WaypointName waypoint_name)
	{
		Debug.Assert(grid_matrices.ContainsKey(waypoint_name));
		return grid_matrices[waypoint_name];
	}
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
	public CrateComponent[,,] crates;
	public AccessoryComponent[,,] accessories;
	public LoadComponent[,,] loads;
	Util.Component[,,] mem_crates;
	Util.Component[,,] mem_accessories;
	Util.Component[,,] mem_loads;
	int[,,] accessory_directions;

	CrateComponent[,,] design_crates;
	AccessoryComponent[,,] design_accessories;
	LoadComponent[,,] design_loads;

	GridCell lastSelectedGrid = null;
	RaycastHit[] hits = new RaycastHit[10];
	public static GridCell SelectedGrid{get; set;}
	static Dictionary<Util.WaypointName, GridMatrix> grid_matrices = new();
	public bool DisableDesign { get; set; } = false;
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
	public static void SelectGridMatrix(Util.WaypointName waypoint_name, bool disable_design)
	{
		Debug.Assert(grid_matrices.ContainsKey(waypoint_name));
		if (current != null)
		{
			current.Active = false;
		}
		current = grid_matrices[waypoint_name];
		Debug.Assert(current != null);
		current.DisableDesign = disable_design;
		current.Active = true;
	}
	/// <summary>
	/// Deactivate GridMatrix.Current and set it to null
	/// </summary>
	public static void DeselectGridMatrix()
	{
		if (current != null)
		{
			current.Active = false;
			current = null;
		}
	}

	public ref CrateComponent GetCrate(Vec3 pos)
	{
		return ref crates[pos.h, pos.w, pos.l];
	}
	public ref AccessoryComponent GetAccessory(Vec3 pos)
	{
		return ref accessories[pos.h, pos.w, pos.l];
	}
	public ref LoadComponent GetLoad(Vec3 pos)
	{
		return ref loads[pos.h, pos.w, pos.l];
	}
	private void Start()
	{
		Debug.Assert(!grid_matrices.ContainsKey(waypoint_name));
		grid_matrices.Add(waypoint_name, this);
		cameraPivot = transform.Find("CameraPivot");
		dummyCamera = cameraPivot.Find("DummyCamera");
		Debug.Assert(cameraPivot != null);
		Debug.Assert(dummyCamera != null);
	}
	private void OnDestroy()
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
	bool HasAccessory(int height, int width, int length) 
	{
		return accessories[height, width, length] != null;
	}
	public void OnEraseEnd()
	{
		if (lastSelectedGrid != null)
		{
			var load = loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
			if (load != null)
			{
				DragImage.DragImages[load.Component].Count++;
				if (load.Component == Util.Component.Pig)
				{
					GameState.Inst.Piggy = null;
					ConfirmButton.Inst.OnGridStateChanged();
				}
				Destroy(load.gameObject);
				loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
			}
			else
			{
				var accessory = accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
				if (accessory != null)
				{
					DragImage.DragImages[accessory.Component].Count++;
					Destroy(accessory.gameObject);
					accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
				}
				var crate = crates[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
				if (crate != null)
				{
					DragImage.DragImages[crate.Component].Count++;
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
						if (mem_accessories[i, j, k] != Util.Component.None)
						{
							Util.Component content = mem_accessories[i, j, k];
							var inst = DragImage.DragImages[content].InstantiateComponent(grids[i, j, k].transform.localPosition, true, accessory_directions[i, j, k]) as AccessoryComponent;
							AddComponent(grids[i, j, k], Util.ComponentType.Accessory, inst);

							DragImage.DragImages[content].Count--;
						}
						if (mem_loads[i, j, k] != Util.Component.None)
						{
							Debug.Assert(mem_loads != null);
							Util.Component content = mem_loads[i, j, k];
							Debug.Log(content);
							var inst = DragImage.DragImages[content].InstantiateComponent(grids[i, j, k].transform.localPosition, true, 0) as LoadComponent;
							Debug.Assert(loads != null);
							Debug.Assert(inst != null);
							AddComponent(grids[i, j, k], Util.ComponentType.Load, inst);
							DragImage.DragImages[content].Count--;
						}
						if (mem_crates[i, j, k] != Util.Component.None)
						{
							Util.Component content = mem_crates[i, j, k];
							var inst = DragImage.DragImages[content].InstantiateComponent(grids[i, j, k].transform.localPosition, true, 0) as CrateComponent;
							Debug.Assert(inst != null);
							AddComponent(grids[i, j, k], Util.ComponentType.Crate, inst);
							DragImage.DragImages[content].Count--;
						}
					}
				}
			}
		}
		ConfirmButton.Inst.OnGridStateChanged();
	}
	Vector3 dragEulerAngle = Vector3.zero;
	void OnGridMatrixDrag(GridMatrixDragEvent e)
	{
		float rotationX = -e.deltaY * drag_rotation_speed;  // Vertical rotation
		float rotationY = e.deltaX * drag_rotation_speed;  // Horizontal rotation											   // Rotate the camera accordingly
		dragEulerAngle += new Vector3(rotationX, rotationY, 0);
		cameraPivot.rotation = Quaternion.Euler(dragEulerAngle);
	}
	void Activate()
	{
		dragEvent = EventBus.Subscribe<GridMatrixDragEvent>(OnGridMatrixDrag);
		dragEulerAngle = new Vector3(32, -90, 0);
		cameraPivot.rotation = Quaternion.Euler(dragEulerAngle);
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
					float offset_height = (float)height / 2 - 0.5f;
					float offset_width = (float)width / 2 - 0.5f;
					float offset_length = (float)length / 2 - 0.5f;
					grid.transform.localPosition = new Vector3(j - offset_width, i - offset_height, k - offset_length);
					grid.transform.localRotation = Quaternion.identity;
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
		activeLayerIndex = -1;
		SetAllLayerActive();

		if (!DisableDesign && Util.forced_designs.ContainsKey(waypoint_name))
		{
			LoadDesignVisuals();
			if (waypoint_name != Util.WaypointName.PreStory1)
			{
				ShowDesign();
			}
		}
		// reset counts
		if (Util.WaypointItems.ContainsKey(waypoint_name))
		{
			DragImage.ClearCountAll();
			var items = Util.WaypointItems[waypoint_name];
			foreach (var item in items)
			{
				DragImage.DragImages[item.Item1].SetInitialCount(item.Item2);
			}
		}
		else
		{
			DragImage.ClearCountAll();
			var items = GameState.Inventory;
			foreach (var item in items)
			{
				DragImage.DragImages[item.Key].SetInitialCount(item.Value);
			}
		}
		BuildCanvas.Inst.InitializeItems();
		if (waypoint_name == Util.WaypointName.PreStory1)
		{
			mem_crates = new Util.Component[height, width,length];
			mem_loads = new Util.Component[height, width,length];
			mem_accessories = new Util.Component[height, width,length];
			accessory_directions = new int[height, width,length];
			mem_crates[0, 0, 0] = Util.Component.WoodenCrate;
			mem_crates[1, 0, 0] = Util.Component.WoodenCrate;
			mem_crates[1, 1, 0] = Util.Component.WoodenCrate;
			mem_crates[1, 1, 1] = Util.Component.WoodenCrate;
			mem_accessories[1, 0, 1] = Util.Component.Wheel;
			mem_accessories[1, 1, 2] = Util.Component.Wheel;
			accessory_directions[1, 0, 1] = 1;
			accessory_directions[1, 1, 2] = 2;
		}
		Util.Delay(this, 5, RebuildVehicle);
	}
	public void ShowDesign()
	{
		if (design_crates == null)
		{
			Debug.LogError("Trying to show a design when there is none.");
			return;
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					if (design_crates[i, j, k] != null)
					{
						design_crates[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
					}
					if (design_accessories[i, j, k] != null)
					{
						design_accessories[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
					}
					if (design_loads[i, j, k] != null)
					{
						design_loads[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
					}
				}
			}
		}
	}
	void LoadDesignVisuals()
	{
		design_crates = new CrateComponent[height, width, length];
		design_accessories = new AccessoryComponent[height, width, length];
		design_loads = new LoadComponent[height, width, length];
		(var design_crates_type, var design_accessories_type, var design_loads_type) = Util.forced_designs[waypoint_name];
		Debug.Assert(design_crates_type.GetLength(0) == height);
		Debug.Assert(design_crates_type.GetLength(1) == width);
		Debug.Assert(design_crates_type.GetLength(2) == length);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					if (design_crates_type[i, j, k] != Util.Component.None)
					{
						design_crates[i, j, k] = DragImage.DragImages[design_crates_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as CrateComponent;
						design_crates[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
					}
					if (design_accessories_type[i, j, k] != Util.Component.None)
					{
						Debug.Log($"Type: {design_accessories_type[i, j, k]}");
						design_accessories[i, j, k] = DragImage.DragImages[design_accessories_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as AccessoryComponent;
						design_accessories[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
						design_accessories[i, j, k].GridMatrix = this;
						design_accessories[i, j, k].Pos = (i, j, k);
					}
					if (design_loads_type[i, j, k] != Util.Component.None)
					{
						design_loads[i, j, k] = DragImage.DragImages[design_loads_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as LoadComponent;
						design_loads[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
					}
				}
			}
		}
		EventBus.Publish(new NeighborChangedEvent());
	}
	// two modes: closest to ray, closest to player

	void SetAllLayerActive()
	{
		for (int i = 0; i < height; i++)
		{
			SetLayerActive(i, true);
		}
		
	}
	void SetAllLayerInactive()
	{
		for (int i = 0; i < height; i++)
		{
			SetLayerActive(i, false);
		}
	}
	void Deactivate()
	{
		EventBus.Unsubscribe(dragEvent);
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
		Dump();
		if (design_crates != null)
		{
			for(int i = 0;i < height; i++)
			{
				for(int j = 0;j < width; j++)
				{
					for(int k = 0;k < length; k++)
					{
						if (design_crates[i, j, k] != null)
						{
							Destroy(design_crates[i, j, k].gameObject);
						}
						if (design_accessories[i, j, k] != null)
						{
							Destroy(design_accessories[i, j, k].gameObject);
						}
						if (design_loads[i, j, k] != null)
						{
							Destroy(design_loads[i, j, k].gameObject);
						}
					}
				}
			}
			design_crates = null;
			design_accessories = null;
			design_loads = null;
		}
	}
	public ref Util.Component GetMemCrate(Vec3 pos)
	{
		return ref mem_crates[pos.h, pos.w, pos.l];
	}
	public ref Util.Component GetMemAccessory(Vec3 pos)
	{
		return ref mem_accessories[pos.h, pos.w, pos.l];
	}
	public ref Util.Component GetMemLoad(Vec3 pos)
	{
		return ref mem_loads[pos.h, pos.w, pos.l];
	}
	void BuildAndStickCrates(Vec3 pos)
	{
		CrateComponent crate = GetCrate(pos);
		if (crate != null)
		{
			GameState.Inst.Components.Add(crate);
			GetMemCrate(pos) = crate.Component;
			crate.Build();
			List<Vec3> deltas = new(){ (1, 0, 0), (0, 1, 0), (0, 0, 1) };
			foreach (var delta in deltas)
			{
				Vec3 new_pos = pos + delta;
				if (InGrid(new_pos) && GetCrate(new_pos)!=null)
				{
					Util.CreateJoint(crate, GetCrate(new_pos), position_spring, position_damper);
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
			mem_accessories[h_idx, w_idx, l_idx] = accessory.Component;
			accessory_directions[h_idx, w_idx, l_idx] = accessory.Direction;
			(bool _wa, bool _sd) = accessory.GetWASD();
			if (_wa) ws = true;
			if (_sd) ad = true;
			accessory.Build();
			accessory.Stick();
		}
	}
	void BuildAndStickLoads(int h_idx, int w_idx, int l_idx)
	{
		LoadComponent load = loads[h_idx, w_idx, l_idx];
		if (load != null)
		{
			GameState.Inst.Components.Add(load);
			mem_loads[h_idx, w_idx, l_idx] = load.Component;
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

		mem_crates = new Util.Component[height, width, length];
		mem_accessories = new Util.Component[height, width, length];
		mem_loads = new Util.Component[height, width, length];
		accessory_directions = new int[height, width, length];

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickCrates((i, j, k));
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
	public bool InGrid(Vec3 pos)
	{
		(int h, int w, int l) = pos.Unwrap();
		return h >= 0 && h < height && w >= 0 && w < width && l >= 0 && l < length;
	}
	public void SwitchLayer()
	{
		EventBus.Publish(new SwitchLayerEvent());
		if (activeLayerIndex != -1)
		{
			SetLayerActive(activeLayerIndex, false);
		}
		activeLayerIndex++;
		if (activeLayerIndex >= height)
		{
			activeLayerIndex = -1;
			SetAllLayerActive();
			EventBus.Publish(new FullLayerEvent());
		}
		else
		{
			if (activeLayerIndex == 0)
			{
				SetAllLayerInactive();
			}
			SetLayerActive(activeLayerIndex, true);
		}
	}
	//void OnClick()
	//{
	//	Debug.LogError("Deprecated!");
	//	Debug.Log("Clicked!");
	//	Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	//	int hitCount = Physics.RaycastNonAlloc(ray, hits);
	//	GridCell closestOccupiedGrid = null;
	//	GridCell closestEmptyGrid = null;
	//	float minOccupiedDistance = 100000.0f;
	//	float minEmptyDistance = 100000.0f;
	//	for (int i = 0; i < hitCount; i++)
	//	{
	//		GameObject hitObject = hits[i].collider.gameObject;
	//		GridCell hitGrid = hitObject.GetComponent<GridCell>();
	//		if (hitGrid == null)
	//		{
	//			continue;
	//		}
	//		if (!hitGrid.Active)
	//		{
	//			continue;
	//		}
	//		float distance = Util.GetDistanceFromRayToPoint(ray, hitObject.transform.position);
	//		if (accessories[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
	//			&& loads[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
	//			&& crates[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null)
	//		{
	//			if (distance < minEmptyDistance)
	//			{
	//				closestEmptyGrid = hitGrid;
	//				minEmptyDistance = distance;
	//			}
	//		}
	//		else
	//		{
	//			if (distance < minOccupiedDistance)
	//			{
	//				closestOccupiedGrid = hitGrid;
	//				minOccupiedDistance = distance;
	//			}
	//		}
	//	}

	//	if (closestOccupiedGrid != null)
	//	{
	//		if (DragImage.Current != null && DragImage.Current.contentType == Util.ContentType.Load
	//			&& accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] == null
	//			&& loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] == null)
	//		{
	//			Debug.Assert(crates[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null);
	//			SelectedGrid = closestOccupiedGrid;
	//			// DragImage.Current.ClickPlace();
	//		}
	//		DirectionalComponent directionalPreview = null;
	//		if (accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
	//		{
	//			directionalPreview = accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
	//		}
	//		else if (loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
	//		{
	//			directionalPreview = loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
	//		}
	//		else
	//		{
	//			Debug.Log("Assert clicking on crate");
	//		}
	//		if (directionalPreview != null)
	//		{
	//			directionalPreview.ChangeDirection();
	//		}
	//	}
	//	else if (closestEmptyGrid != null)
	//	{
	//		if (DragImage.Current != null && DragImage.Current.Count > 0)
	//		{
	//			SelectedGrid = closestEmptyGrid;
	//			// DragImage.Current.ClickPlace();
	//		}
	//	}
	//}

	void OnClick()
	{
		if (CurrentCursorMode == Util.CursorMode.Erase)
		{
			if (SelectedGrid != null)
			{
				(var h, var w, var l) = (SelectedGrid.heightIdx, SelectedGrid.widthIdx, SelectedGrid.lengthIdx);
				var load = loads[h, w, l];
				var crate = crates[h, w, l];
				var accessory = accessories[h, w, l];
				if (load != null)
				{
					Destroy (load.gameObject);
					DragImage.DragImages[load.Component].Count++;
					loads[h, w, l] = null;
				}
				else if (crate != null)
				{
					Destroy(crate.gameObject);
					DragImage.DragImages[crate.Component].Count++;
					crates[h, w, l] = null;
				}
				else if (accessory != null)
				{
					Destroy(accessory.gameObject);
					DragImage.DragImages[accessory.Component].Count++;
					accessories[h, w, l] = null;
				}
				else
				{
					Debug.LogError("An invariant found: selected a grid but cannot erase");
				}
				EventBus.Publish(new ItemErasedEvent());
			}
		}
		else if(CurrentCursorMode == Util.CursorMode.ChangeDirection)
		{
            if (SelectedGrid != null)
            {
                (var h, var w, var l) = (SelectedGrid.heightIdx, SelectedGrid.widthIdx, SelectedGrid.lengthIdx);
                var load = loads[h, w, l];
                var crate = crates[h, w, l];
                var accessory = accessories[h, w, l];

                if (accessory != null)
                {
                    accessory.ChangeDirection();
                }
                else
                {
                    Debug.LogWarning("An invariant found: selected a grid but cannot change direction");
                }
            }
        }
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
		
		MouseMove();

		if (Input.GetMouseButtonDown(0))
		{
			OnClick();
		}
		if (Input.mouseScrollDelta.y!= 0)
		{
			PivotDistance = Mathf.Clamp(PivotDistance - Input.mouseScrollDelta.y * zoom_speed, min_dist, max_dist);
		}
	}
	float pivot_distance = 5.0f;
	public float min_dist = 3.0f;
	public float max_dist = 7.0f;
	float PivotDistance
	{
		get { return pivot_distance; }
		set
		{
			pivot_distance = value;
			Vector3 position = dummyCamera.localPosition;
			position.z = -pivot_distance;
			dummyCamera.localPosition = position;
		}
	}
	public float zoom_speed = 0.25f;

	// build mode (crate and load special)
	// direction mode
	// erase mode (any grid that contains something)
	public Util.CursorMode CurrentCursorMode { get; set; }
	void MouseMove()
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
			(int h, int w, int l) = (hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx);
			switch(CurrentCursorMode)
			{
				case Util.CursorMode.Idle:
					break;
				case Util.CursorMode.AddComponent:
					if (DisableDesign || !Util.forced_designs.ContainsKey(waypoint_name))
					{
						if (!Occupied(h, w, l))
							break;
						if (DragImage.CurrentContentType == Util.ComponentType.Load && AllowLoad(h, w, l))
							break;
						if (DragImage.CurrentContentType == Util.ComponentType.Crate && AllowCrate(h, w, l))
							break;
					}
					else
					{
						if (DragImage.Current != null &&
							((design_crates[h, w, l] != null && crates[h, w, l] == null && DragImage.Current.content == design_crates[h, w, l].Component)
							|| (design_accessories[h, w, l] !=null && accessories[h, w, l] == null && DragImage.Current.content == design_accessories[h, w, l].Component)
							|| (design_loads[h, w, l] != null && loads[h, w, l] == null && DragImage.Current.content == design_loads[h, w, l].Component)))
							break;
					}
					continue;
				case Util.CursorMode.ChangeDirection:
					if (HasAccessory(h, w, l))
						break;
					continue;
				case Util.CursorMode.Erase:
					if (Occupied(h, w, l))
						break;
					continue;
				default:
					Debug.LogError($"Unknown Cursor Mode: {CurrentCursorMode}");
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
			// do nothing
		}
	}
	public void AddComponent(GridCell selectedGrid, Util.ComponentType contentType, VehicleComponent content)
	{
		EventBus.Publish(new ComponentAddedEvent());
		ConfirmButton.Inst.OnGridStateChanged();
		Debug.Assert(content != null);
		GridCell grid = selectedGrid;
		(var h, var w, var l) = (grid.heightIdx, grid.widthIdx, grid.lengthIdx);
		switch (contentType)
		{
			case Util.ComponentType.Crate:
				CrateComponent cratePreview = content as CrateComponent;
				Debug.Assert(cratePreview != null);
				crates[h, w, l] = cratePreview;
				break;
			case Util.ComponentType.Accessory:
				AccessoryComponent accessoryPreview = content as AccessoryComponent;
				Debug.Assert(accessoryPreview != null);
				accessoryPreview.Pos = (h, w, l);
				accessories[h, w, l] = accessoryPreview;
				break;
			case Util.ComponentType.Load:
				LoadComponent loadPreview = content as LoadComponent;
				Debug.Assert(loadPreview != null);
				loads[h, w, l] = loadPreview;
				PiggyPreview preview = loadPreview as PiggyPreview;
				if (preview != null)
				{
					GameState.Inst.Piggy = preview;
					ConfirmButton.Inst.OnGridStateChanged();
				}
				break;
		}
		EventBus.Publish(new NeighborChangedEvent());
	}
}