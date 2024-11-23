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
	int design_number = 0;
	public int DesignNumber
	{
		get { return design_number; }
		set
		{
			design_number = value;
			StartCoroutine(OnDesignNumberChanged());
		}
	}

	public IEnumerator OnDesignNumberChanged()
	{
		Dump();
		yield return null;
		yield return null;
		yield return null;
		yield return null;
		RebuildVehicle();
	}

	public GameObject gridPrefab;
	public int activeLayerIndex = 0;
	public float drag_rotation_speed = 0.05f;
	public bool Active { get; private set; } = false;
	public Probe Probe { get; private set; }
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
	static GridMatrix inst;
	public static GridMatrix Inst
	{
		get { Debug.Assert(inst != null); return inst; }
	}
	public void MoveToCheckpoint(Util.WaypointName waypoint_name)
	{
		Checkpoint checkpoint = Checkpoint.Get(waypoint_name);
		transform.position = checkpoint.transform.position;
		transform.rotation = checkpoint.transform.rotation;
	}
	private void Start()
	{
		
		Util.Delay(this, () =>
		{
			Probe = transform.Find("Probe").GetComponent<Probe>();
			(int h, int w, int l) = GameSave.GridSize;
			Probe.transform.localScale = new Vector3(w, h, l);
			ProbeTargetPos = new Vector3(0, ((float)h - 1.0f) / 2.0f, 0);


			Debug.Assert(inst == null);
			inst = this;
			
			EventBus.Subscribe<GridMatrixSizeChangedEvent>(OnGridMatrixSizeChanged);
			InitComponentArray();
			InitPhantom();
			ProbeResize();
			SpawnGrids();
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
		// InitMemory();
		InitPhantom();
		ProbeResize();
	}
	
	
	void RebuildVehicle()
	{
		Debug.Assert(GameSave.CurrentMemory.MemAccessories != null);
		var mem_accessories = GameSave.CurrentMemory.MemAccessories;
		var mem_crates = GameSave.CurrentMemory.MemCrates;
		var mem_loads = GameSave.CurrentMemory.MemLoads;
		var accessory_directions = GameSave.CurrentMemory.AccessoryDirections;
		(int h, int w, int l) = GameSave.GridSize;
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					if (mem_accessories[i, j, k] != Util.Component.None)
					{
						Util.Component component = mem_accessories[i, j, k];
						var inst = DragImage.DragImages[component].InstantiateComponent(grids[i, j, k].transform.localPosition, true, accessory_directions[i, j, k]) as AccessoryComponent;
						AddComponent(grids[i, j, k], Util.ComponentType.Accessory, inst);
						DragImage.DragImages[component].Count--;
					}
					if (mem_loads[i, j, k] != Util.Component.None)
					{
						Debug.Assert(mem_loads != null);
						Util.Component content = mem_loads[i, j, k];
						// Debug.Log(content);
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
		ConfirmButton.Inst.OnGridStateChanged();
		Util.Delay(this, 5, () =>
		{
			EventBus.Publish(new NeighborChangedEvent());
		});		
	}
	public IEnumerator Activate()
	{
		Active = true;
		ResetActiveLayer();
		for (int i = 0; i < 5; i++)
		{
			yield return null;
		}
		RebuildVehicle();
		yield return null;
	}
	public void ActivateAsync()
	{
		StartCoroutine(Activate());
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
	public void ShowDesign((Util.Component[,,], Util.Component[,,], Util.Component[,,]) design)
	{
		Debug.Assert(phantom_crates != null);
		(var design_crates_type, var design_accessories_type, var design_loads_type) = design;
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
						phantom_crates[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
					}
					if (design_accessories_type[i, j, k] != Util.Component.None)
					{
						// Debug.Log($"Type: {design_accessories_type[i, j, k]}");
						phantom_accessories[i, j, k] = DragImage.DragImages[design_accessories_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as AccessoryComponent;
						phantom_accessories[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
						phantom_accessories[i, j, k].GridMatrix = this;
						phantom_accessories[i, j, k].Pos = (i, j, k);
					}
					if (design_loads_type[i, j, k] != Util.Component.None)
					{
						phantom_loads[i, j, k] = DragImage.DragImages[design_loads_type[i, j, k]].InstantiateDesignComponent(grids[i, j, k]) as LoadComponent;
						phantom_loads[i, j, k].MoveGlobal(grids[i, j, k].transform.position);
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
		transform.position = new Vector3(0, -1000, 0);
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
					Util.CreateJoint(crate, GetCrate(new_pos), Util.break_force, Util.break_torque);
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
			if (accessory.GetComponent<Umbrella>() != null)
			{
				PlayCanvas.Inst.ShowUmbrella();
			}
			if (accessory.GetComponent<Rocket>() != null)
			{
				PlayCanvas.Inst.ShowRocket();
				q = true;
			}
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
				Util.CreateJoint(load, crates[h_idx, w_idx, l_idx], Util.break_force, Util.break_torque);
			}
		}
	}
	bool q = false;
	public IEnumerator BuildAndDeactivate()
	{
		PlayCanvas.Inst.HideUmbrella();
		PlayCanvas.Inst.HideRocket();

		// GameState.Inst.Components.Clear();
		ws = false;
		ad = false;
		q = false;

		GameSave.CurrentMemory.Memorize();
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
		yield return AttachToCarCore();
		ClearComponents(false);
		// Active = false;
		PlayButtonsDisplayer.Inst.UpdateWASD(ws, ad, q);
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
		EventBus.Publish(new ResetCountEvent());
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
	public void MoveProbeToGrid()
	{
		Probe.MovePosition(transform.position + transform.rotation*ProbeTargetPos);
		Probe.MoveRotation(transform.rotation);
	}
	public void MoveProbeToOrigin()
	{
		Probe.MovePosition(new Vector3(0, -1000, 0));
	}
	IEnumerator ScanHelper()
	{
		Retry.Inst.CanRetry = false;
		CarCore.Inst.Fix();
		ShowProbe();
		Vector3 start_position = CarCore.Inst.transform.position + new Vector3(0, start_scan_height, 0);
		Vector3 end_position = CarCore.Inst.transform.position + new Vector3(0, end_scan_height, 0);
		transform.position = start_position;
		Vector3 up_vector = CarCore.Inst.transform.up;
		float y_rotation = CarCore.Inst.transform.rotation.eulerAngles.y;
		float angle = Vector3.Angle(up_vector, new Vector3(0, 1, 0));
		// Debug.Log($"angle: {angle}");
		if (angle > 20)
		{
			if (angle > 90)
			{
				transform.rotation = Quaternion.Euler(0, y_rotation + 180, 0);
			}
			else
			{
				transform.rotation = Quaternion.Euler(0, y_rotation, 0);
			}
		}
		else
		{
			transform.rotation = CarCore.Inst.transform.rotation;
		}
		yield return null;
		MoveProbeToGrid();
		// Probe.MoveRotation(transform.rotation);

		float start_time = Time.time;
		yield return new WaitForSeconds(0.2f);
		while (Time.time - start_time < scan_time)
		{
			transform.position = Vector3.Lerp(start_position, end_position, (Time.time - start_time) / scan_time);
			Probe.MovePosition(new Vector3(0, - 100, 0));
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			MoveProbeToGrid();			
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			CollisionFlag = false;
			yield return null;
			yield return null;
			yield return null;
			yield return null;
			if (!CollisionFlag)
			{
				Debug.Log($"Success!");
				EventBus.Publish(new ScanSuccessEvent());
				HideProbe();
				Retry.Inst.CanRetry = true;
				yield break;
			}
			yield return null;
		}
		Debug.Log($"Fail!");
		transform.position = new Vector3(0, -100, 0);
		EventBus.Publish(new ScanFailEvent());
		MoveProbeToGrid();
		HideProbe();
		Retry.Inst.CanRetry = true;
	}
}

public class ScanSuccessEvent { }
public class ScanFailEvent { }