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

public class GridMatrixSizeChangedEvent { }

public partial class GridMatrix: MonoBehaviour
{
	public GameObject gridPrefab;
	public int activeLayerIndex = 0;
	public float drag_rotation_speed = 0.05f;
	public bool Active { get; private set; } = false;
	public Probe Probe { get; private set; }

	//Transform cameraPivot;
	//Transform dummyCamera;
	//public Transform DummyCamera
	//{
	//	get { Debug.Assert(dummyCamera != null); return dummyCamera; }
	//}

	//Subscription<GridMatrixDragEvent> dragEvent;
	//public static GridMatrix Get(Util.WaypointName waypoint_name)
	//{
	//	Debug.Assert(grid_matrices.ContainsKey(waypoint_name));
	//	return grid_matrices[waypoint_name];
	//}
	//bool Active
	//{
	//	get { return active; }
	//	set {
	//		if (active != value)
	//		{
	//			active = value;
	//			if (active)
	//			{
	//				Activate();
	//			}
	//			else
	//			{
	//				Deactivate();
	//			}
	//		}
	//	}
	//}
	GridCell[,,] grids;
	public CrateComponent[,,] crates;
	public AccessoryComponent[,,] accessories;
	public LoadComponent[,,] loads;


	CrateComponent[,,] phantom_crates;
	AccessoryComponent[,,] phantom_accessories;
	LoadComponent[,,] phantom_loads;

	GridCell LastSelectedGrid { get; set; } = null;
	RaycastHit[] hits = new RaycastHit[20];
	public GridCell SelectedGrid{get; set;}
	// static Dictionary<Util.WaypointName, GridMatrix> grid_matrices = new();
	// public bool DisableDesign { get; set; } = false;
	//public static GridMatrix Current
	//{
	//	get {
	//		Debug.Assert(current != null, "Current Grid Matrix not set");
	//		return current;
	//	}
	//}
	//static GridMatrix current;

	static GridMatrix inst;
	public static GridMatrix Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	/// <summary>
	/// Activate the GridMatrix corresponding to the level num, deactivate other GridMatrices, and update GridMatrix.Current
	/// </summary>
	//public static void SelectGridMatrix(Util.WaypointName waypoint_name, bool disable_design)
	//{
	//	Debug.Assert(grid_matrices.ContainsKey(waypoint_name));
	//	if (current != null)
	//	{
	//		current.Active = false;
	//	}
	//	current = grid_matrices[waypoint_name];
	//	Debug.Assert(current != null);
	//	current.DisableDesign = disable_design;
	//	current.Active = true;
	//}
	/// <summary>
	/// Deactivate GridMatrix.Current and set it to null
	/// </summary>
	//public static void DeselectGridMatrix()
	//{
	//	if (current != null)
	//	{
	//		current.Active = false;
	//		current = null;
	//	}
	//}

	public void MoveToCheckpoint(Util.WaypointName waypoint_name)
	{
		Checkpoint checkpoint = Checkpoint.Get(waypoint_name);
		transform.position = checkpoint.transform.position;
		transform.rotation = checkpoint.transform.rotation;
	}
	private void Start()
	{
		// Debug.Assert(!grid_matrices.ContainsKey(waypoint_name));
		// grid_matrices.Add(waypoint_name, this);
		// cameraPivot = transform.Find("CameraPivot");
		// dummyCamera = cameraPivot.Find("DummyCamera");
		// Debug.Assert(cameraPivot != null);
		// Debug.Assert(dummyCamera != null);
		Debug.Assert(inst == null);
		inst = this;
		Probe = transform.Find("Probe").GetComponent<Probe>();
		EventBus.Subscribe<GridMatrixSizeChangedEvent>(OnGridMatrixSizeChanged);
		InitMemory();
		InitComponentArray();
		InitPhantom();
		ProbeResize();
		SpawnGrids();
		Util.Delay(this, () =>
		{
			Deactivate();
		});
	}
	private void OnDestroy()
	{
		// grid_matrices.Clear();
		inst = null;
	}

	
	public void OnGridMatrixSizeChanged(GridMatrixSizeChangedEvent e)
	{
		DestroyGrids();
		SpawnGrids();
		InitComponentArray();
		InitMemory();
		InitPhantom();
		ProbeResize();
	}
	
	
	void RebuildVehicle()
	{
		Debug.Assert(GameSave.MemAccessories != null);
		(int h, int w, int l) = GameSave.GridSize;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					if (GameSave.MemAccessories[i, j, k] != Util.Component.None)
					{
						Util.Component component = GameSave.MemAccessories[i, j, k];
						var inst = DragImage.DragImages[component].InstantiateComponent(grids[i, j, k].transform.localPosition, true, GameSave.AccessoryDirections[i, j, k]) as AccessoryComponent;
						AddComponent(grids[i, j, k], Util.ComponentType.Accessory, inst);
						DragImage.DragImages[component].Count--;
					}
					if (GameSave.MemLoads[i, j, k] != Util.Component.None)
					{
						Debug.Assert(GameSave.MemLoads != null);
						Util.Component content = GameSave.MemLoads[i, j, k];
						// Debug.Log(content);
						var inst = DragImage.DragImages[content].InstantiateComponent(grids[i, j, k].transform.localPosition, true, 0) as LoadComponent;
						Debug.Assert(loads != null);
						Debug.Assert(inst != null);
						AddComponent(grids[i, j, k], Util.ComponentType.Load, inst);
						DragImage.DragImages[content].Count--;
					}
					if (GameSave.MemCrates[i, j, k] != Util.Component.None)
					{
						Util.Component content = GameSave.MemCrates[i, j, k];
						var inst = DragImage.DragImages[content].InstantiateComponent(grids[i, j, k].transform.localPosition, true, 0) as CrateComponent;
						Debug.Assert(inst != null);
						AddComponent(grids[i, j, k], Util.ComponentType.Crate, inst);
						DragImage.DragImages[content].Count--;
					}
				}
			}
		}
		ConfirmButton.Inst.OnGridStateChanged();
	}
	// Vector3 dragEulerAngle = Vector3.zero;
	//void OnGridMatrixDrag(GridMatrixDragEvent e)
	//{
	//	float rotationX = -e.deltaY * drag_rotation_speed;  // Vertical rotation
	//	float rotationY = e.deltaX * drag_rotation_speed;  // Horizontal rotation											   // Rotate the camera accordingly
	//	dragEulerAngle += new Vector3(rotationX, rotationY, 0);
	//	cameraPivot.rotation = Quaternion.Euler(dragEulerAngle);
	//}


	// collider 
	// core disable
	// activate: space enable, enable build canvas, update selected grid
	public void Activate()
	{
		Active = true;
		// dragEvent = EventBus.Subscribe<GridMatrixDragEvent>(OnGridMatrixDrag);
		// dragEulerAngle = new Vector3(32, -90, 0);
		// cameraPivot.rotation = Quaternion.Euler(dragEulerAngle);

		ResetActiveLayer();

		//if (!DisableDesign && Util.forced_designs.ContainsKey(waypoint_name))
		//{
		//	LoadDesignVisuals();
		//	if (waypoint_name != Util.WaypointName.PreStory1)
		//	{
		//		ShowDesign();
		//	}
		//}
		// reset counts


		//if (Util.WaypointItems.ContainsKey(waypoint_name))
		//{
		//	DragImage.ClearCountAll();
		//	var items = Util.WaypointItems[waypoint_name];
		//	foreach (var item in items)
		//	{
		//		DragImage.DragImages[item.Item1].SetInitialCount(item.Item2);
		//	}
		//}
		//else
		//{
		//	DragImage.ClearCountAll();
		//	var items = GameState.Inventory;
		//	foreach (var item in items)
		//	{
		//		DragImage.DragImages[item.Key].SetInitialCount(item.Value);
		//	}
		//}

		// BuildCanvas.Inst.InitializeItems();
		
		
		//if (waypoint_name == Util.WaypointName.PreStory1)
		//{
		//	mem_crates = new Util.Component[initial_height, initial_width,initial_length];
		//	mem_loads = new Util.Component[initial_height, initial_width,initial_length];
		//	mem_accessories = new Util.Component[initial_height, initial_width,initial_length];
		//	accessory_directions = new int[initial_height, initial_width,initial_length];
		//	mem_crates[0, 0, 0] = Util.Component.WoodenCrate;
		//	mem_crates[1, 0, 0] = Util.Component.WoodenCrate;
		//	mem_crates[1, 1, 0] = Util.Component.WoodenCrate;
		//	mem_crates[1, 1, 1] = Util.Component.WoodenCrate;
		//	mem_accessories[1, 0, 1] = Util.Component.Wheel;
		//	mem_accessories[1, 1, 2] = Util.Component.Wheel;
		//	accessory_directions[1, 0, 1] = 1;
		//	accessory_directions[1, 1, 2] = 2;
		//}

		Util.Delay(this, 5, RebuildVehicle);
	}
	//public void ShowDesign()
	//{
	//	Debug.Assert(phantom_crates != null);
	//	for (int i = 0; i < initial_height; i++)
	//	{
	//		for (int j = 0; j < initial_width; j++)
	//		{
	//			for (int k = 0; k < initial_length; k++)
	//			{
	//				if (phantom_crates[i, j, k] != null)
	//				{
	//					phantom_crates[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
	//				}
	//				if (phantom_accessories[i, j, k] != null)
	//				{
	//					phantom_accessories[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
	//				}
	//				if (phantom_loads[i, j, k] != null)
	//				{
	//					phantom_loads[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
	//				}
	//			}
	//		}
	//	}
	//}
	public void ShowDesign(Util.WaypointName waypoint_name)
	{
		Debug.Assert(phantom_crates != null);
		(var design_crates_type, var design_accessories_type, var design_loads_type) = Util.forced_designs[waypoint_name];
		(int h, int w, int l) = GameSave.GridSize;
		Debug.Assert(design_crates_type.GetLength(0) == h);
		Debug.Assert(design_crates_type.GetLength(1) == w);
		Debug.Assert(design_crates_type.GetLength(2) == l);
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					if (design_crates_type[i, j, k] != Util.Component.None)
					{
						phantom_crates[i, j, k] = DragImage.DragImages[design_crates_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as CrateComponent;
						phantom_crates[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
					}
					if (design_accessories_type[i, j, k] != Util.Component.None)
					{
						Debug.Log($"Type: {design_accessories_type[i, j, k]}");
						phantom_accessories[i, j, k] = DragImage.DragImages[design_accessories_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as AccessoryComponent;
						phantom_accessories[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
						phantom_accessories[i, j, k].GridMatrix = this;
						phantom_accessories[i, j, k].Pos = (i, j, k);
					}
					if (design_loads_type[i, j, k] != Util.Component.None)
					{
						phantom_loads[i, j, k] = DragImage.DragImages[design_loads_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as LoadComponent;
						phantom_loads[i, j, k].MoveGlobal(new Vector3(0, 0, 0));
					}
				}
			}
		}
		EventBus.Publish(new NeighborChangedEvent());
	}
	// two modes: closest to ray, closest to player

	// called if built
	public void Deactivate()
	{
		Active = false;
		// EventBus.Unsubscribe(dragEvent);
		// remove all grids
		//for (int i = 0; i < initial_height; i++)
		//{
		//	for (int j = 0; j < initial_width; j++)
		//	{
		//		for (int k = 0; k < initial_length; k++)
		//		{
		//			Grid grid = grids[i, j, k];
		//			Debug.Assert(grid != null);
		//			Destroy(grid.gameObject);
		//		}
		//	}
		//}
		Dump();
		if (phantom_crates != null)
		{
			for(int i = 0;i < phantom_crates.GetLength(0); i++)
			{
				for(int j = 0;j < phantom_crates.GetLength(1); j++)
				{
					for(int k = 0;k < phantom_crates.GetLength(2); k++)
					{
						if (phantom_crates[i, j, k] != null)
						{
							Destroy(phantom_crates[i, j, k].gameObject);
						}
						if (phantom_accessories[i, j, k] != null)
						{
							Destroy(phantom_accessories[i, j, k].gameObject);
						}
						if (phantom_loads[i, j, k] != null)
						{
							Destroy(phantom_loads[i, j, k].gameObject);
						}
					}
				}
			}
		}
		transform.position = Vector3.zero;
	}
	public ref Util.Component GetMemCrate(Vec3 pos)
	{
		return ref GameSave.MemCrates[pos.h, pos.w, pos.l];
	}
	public ref Util.Component GetMemAccessory(Vec3 pos)
	{
		return ref GameSave.MemAccessories[pos.h, pos.w, pos.l];
	}
	public ref Util.Component GetMemLoad(Vec3 pos)
	{
		return ref GameSave.MemLoads[pos.h, pos.w, pos.l];
	}
	void BuildAndStickCrates(Vec3 pos)
	{
		CrateComponent crate = GetCrate(pos);
		if (crate != null)
		{
			GameState.Inst.Components.Add(crate);
			// GetMemCrate(pos) = crate.Component;
			crate.Build();
			List<Vec3> deltas = new(){ (1, 0, 0), (0, 1, 0), (0, 0, 1) };
			foreach (var delta in deltas)
			{
				Vec3 new_pos = pos + delta;
				if (InGrid(new_pos) && GetCrate(new_pos)!=null)
				{
					Util.CreateJoint(crate, GetCrate(new_pos), Util.position_spring, Util.position_damper);
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
			// mem_accessories[h_idx, w_idx, l_idx] = accessory.Component;
			// accessory_directions[h_idx, w_idx, l_idx] = accessory.Direction;
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
			// mem_loads[h_idx, w_idx, l_idx] = load.Component;
			load.Build();
			if (crates[h_idx, w_idx, l_idx] != null)
			{
				Util.CreateJoint(load, crates[h_idx, w_idx, l_idx], Util.position_spring, Util.position_damper);
			}
		}
	}
	public void BuildAndDeactivate()
	{
		// GameState.Inst.Components.Clear();
		ws = false;
		ad = false;

		Memorize();
		(int h, int w, int l) = GameSave.GridSize;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					BuildAndStickCrates((i, j, k));
				}
			}
		}
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					BuildAndStickAccessories(i, j, k);
				}
			}
		}
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					BuildAndStickLoads(i, j, k);
				}
			}
		}
		AttachToCarCore();
		ClearComponents(false);
		// Active = false;
		PlayButtonsDisplayer.Inst.UpdateWASD(ws, ad);
		Deactivate();
	}

	public void Dump()
	{
		(int h, int w, int l) = GameSave.GridSize;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
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
		ConfirmButton.Inst.OnGridStateChanged();
	}
	
	void OnClick()
	{
		if (CurrentCursorMode == Util.CursorMode.Erase)
		{
			if (SelectedGrid != null)
			{
				(var h, var w, var l) = SelectedGrid.Pos;
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
				ConfirmButton.Inst.OnGridStateChanged();
			}
		}
		else if(CurrentCursorMode == Util.CursorMode.ChangeDirection)
		{
            if (SelectedGrid != null)
            {
                (var h, var w, var l) = SelectedGrid.Pos;
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
		if (!Active)
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
		//if (Input.mouseScrollDelta.y!= 0)
		//{
		//	PivotDistance = Mathf.Clamp(PivotDistance - Input.mouseScrollDelta.y * zoom_speed, min_dist, max_dist);
		//}
	}
	//float pivot_distance = 5.0f;
	//public float min_dist = 3.0f;
	//public float max_dist = 7.0f;
	//float PivotDistance
	//{
	//	get { return pivot_distance; }
	//	set
	//	{
	//		pivot_distance = value;
	//		Vector3 position = dummyCamera.localPosition;
	//		position.z = -pivot_distance;
	//		dummyCamera.localPosition = position;
	//	}
	//}
	//public float zoom_speed = 0.25f;

	// build mode (crate and load special)
	// direction mode
	// erase mode (any grid that contains something)
	public Util.CursorMode CurrentCursorMode { get; set; }
	public bool ForceDesign { get; set; } = false;
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
			(int h, int w, int l) = hitGrid.Pos;
			switch(CurrentCursorMode)
			{
				case Util.CursorMode.Idle:
					break;
				case Util.CursorMode.AddComponent:
					if (!ForceDesign)
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
							((phantom_crates[h, w, l] != null && crates[h, w, l] == null && DragImage.Current.content == phantom_crates[h, w, l].Component)
							|| (phantom_accessories[h, w, l] !=null && accessories[h, w, l] == null && DragImage.Current.content == phantom_accessories[h, w, l].Component)
							|| (phantom_loads[h, w, l] != null && loads[h, w, l] == null && DragImage.Current.content == phantom_loads[h, w, l].Component)))
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
		if (closestGrid != null && LastSelectedGrid != null && LastSelectedGrid == closestGrid)
		{
			// 
		}
		else if (closestGrid != null && LastSelectedGrid != null && LastSelectedGrid != closestGrid)
		{
			LastSelectedGrid.Deselect();
			closestGrid.Select();
			LastSelectedGrid = closestGrid;
			SelectedGrid = closestGrid;
		}
		else if (closestGrid != null && LastSelectedGrid == null)
		{
			closestGrid.Select();
			LastSelectedGrid = closestGrid;
			SelectedGrid = closestGrid;
		}
		else if (closestGrid == null && LastSelectedGrid != null)
		{
			LastSelectedGrid.Deselect();
			LastSelectedGrid = null;
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
		(var h, var w, var l) = grid.Pos;
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
				//PiggyPreview preview = loadPreview as PiggyPreview;
				//if (preview != null)
				//{
				//	GameState.Inst.Piggy = preview;
				//	ConfirmButton.Inst.OnGridStateChanged();
				//}
				break;
		}
		EventBus.Publish(new NeighborChangedEvent());
	}
	public void Scan()
	{
		Debug.Assert(!Active);
		StartCoroutine(ScanHelper());
	}
	public float start_scan_height = -2.0f;
	public float end_scan_height = 3.0f;
	public float scan_time = 2.0f;
	IEnumerator ScanHelper()
	{
		CarCore.Inst.Fix();
		ShowProbe();
		Probe.MovePosition(transform.position + ProbeTargetPos);
		Probe.MoveRotation(transform.rotation);
		Vector3 start_position = CarCore.Inst.transform.position + new Vector3(0, start_scan_height, 0);
		Vector3 end_position = CarCore.Inst.transform.position + new Vector3(0, end_scan_height, 0);
		transform.position = start_position;
		Vector3 up_vector = CarCore.Inst.transform.up;
		float y_rotation = CarCore.Inst.transform.rotation.eulerAngles.y;
		float angle = Vector3.Angle(up_vector, new Vector3(0, 1, 0));
		Debug.Log($"angle: {angle}");
		if (angle > 20)
		{
			transform.rotation = Quaternion.Euler(0, y_rotation, 0);
		}
		else
		{
			transform.rotation = CarCore.Inst.transform.rotation;
		}
		float start_time = Time.time;
		yield return new WaitForSeconds(0.2f);
		while (Time.time - start_time < scan_time)
		{
			transform.position = Vector3.Lerp(start_position, end_position, (Time.time - start_time) / scan_time);
			Probe.MovePosition(transform.position + ProbeTargetPos);
			// Debug.Log($"Collision count: {CollisionCount}");
			if (CollisionCount <=0)
			{
				Debug.Log("Success!");
				EventBus.Publish(new ScanSuccessEvent());
				HideProbe();
				yield break;
			}
			yield return null;
		}
		EventBus.Publish(new ScanFailEvent());
		Probe.MovePosition(transform.position + ProbeTargetPos);
		HideProbe();
		CarCore.Inst.Unfix();
	}
}

public class ScanSuccessEvent { }
public class ScanFailEvent { }