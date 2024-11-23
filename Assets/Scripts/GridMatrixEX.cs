using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class GridMatrix: MonoBehaviour
{
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
	void SetLayerActive(int index, bool active)
	{
		if (index >= grids.GetLength(0))
		{
			Debug.LogError("Layer out of bound");
		}
		for (int i = 0; i < grids.GetLength(1); i++)
		{
			for (int j = 0; j < grids.GetLength(2); j++)
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
	void SpawnGrids()
	{
		(int h, int w, int l) = GameSave.GridSize;
		grids = new GridCell[h, w, l];
		Debug.Log($"Spawn grid: {h}, {w}, {l}");
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				for (int k = 0; k < l; k++)
				{
					Debug.Assert(gridPrefab != null);
					GameObject grid = Instantiate(gridPrefab, transform);
					float offset_height = 0.0f;
					float offset_width = (float)w / 2 - 0.5f;
					float offset_length = (float)l / 2 - 0.5f;
					grid.transform.localPosition = new Vector3(j - offset_width, i - offset_height, k - offset_length);
					grid.transform.localRotation = Quaternion.identity;
					Debug.Assert(grid != null);
					GridCell gridComponent = grid.GetComponent<GridCell>();
					Debug.Assert(gridComponent != null);
					gridComponent.Pos = (i, j, k);
					grids[i, j, k] = gridComponent;
				}
			}
		}
	}
	void DestroyGrids()
	{
		foreach(Transform child in transform)
		{
			if (child.GetComponent<GridCell>() != null)
			{
				Destroy(child.gameObject);
			}	
		}
	}
	void ClearComponents(bool destroy_object)
	{
		for (int i = 0; i < crates.GetLength(0); i++)
		{
			for (int j = 0; j < crates.GetLength(1); j++)
			{
				for (int k = 0; k < crates.GetLength(2); k++)
				{
					if (destroy_object)
					{
						if (crates[i, j, k] != null)
						{
							Destroy(crates[i, j, k].gameObject);
						}
						if (accessories[i, j, k] != null)
						{
							Destroy(accessories[i, j, k].gameObject);
						}
						if (loads[i, j, k] != null)
						{
							Destroy(loads[i, j, k].gameObject);
						}
					}
					else
					{
						crates[i, j, k] = null;
						accessories[i, j, k] = null;
						loads[i, j, k] = null;
					}
				}
			}
		}
	}
	void InitComponentArray()
	{
		(int h, int w, int l) = GameSave.GridSize;
		bool rebuild = crates != null;
		// to do
		if (rebuild)
		{
			Debug.LogWarning("Resizing component array and rebuilding vehicle.");
			// Memorize();
			ClearComponents(true);
		}
		crates = new CrateComponent[h, w, l];
		accessories = new AccessoryComponent[h, w, l];
		loads = new LoadComponent[h, w, l];
		if (rebuild)
		{
			// RebuildVehicle();
			// ConfirmButton.Inst.OnGridStateChanged();
		}
	}
	
	void InitPhantom()
	{
		if (phantom_crates != null)
		{
			Debug.Assert(phantom_accessories != null);
			Debug.Assert(phantom_loads != null);
			for (int i = 0; i < phantom_crates.GetLength(0); i++)
			{
				for (int j = 0; j < phantom_crates.GetLength(1); j++)
				{
					for (int k = 0; k < phantom_crates.GetLength(2); k++)
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
		(int h, int w, int l) = GameSave.GridSize;
		phantom_crates = new CrateComponent[h, w, l];
		phantom_accessories = new AccessoryComponent[h, w, l];
		phantom_loads = new LoadComponent[h, w, l];
	}
	void ResetActiveLayer()
	{
		activeLayerIndex = -1;
		SetAllLayerActive();
	}
	void SetAllLayerActive()
	{
		for (int i = 0; i < grids.GetLength(0); i++)
		{
			SetLayerActive(i, true);
		}
	}
	void SetAllLayerInactive()
	{
		for (int i = 0; i < grids.GetLength(0); i++)
		{
			SetLayerActive(i, false);
		}
	}
	public void Memorize(GameSave.GridMemory grid_memory)
	{
		(int h, int w, int l) = (crates.GetLength(0), crates.GetLength(1), crates.GetLength(2));
		Debug.Assert(grid_memory.MemCrates.GetLength(0) >= h);
		Debug.Assert(grid_memory.MemCrates.GetLength(1) >= w);
		Debug.Assert(grid_memory.MemCrates.GetLength(2) >= l);
		for (int i = 0;i < h;i++)
		{
			for(int j = 0;j <  w;j++)
			{
				for(int k = 0; k < l;k++)
				{
					if (crates[i, j, k] != null)
					{
						grid_memory.MemCrates[i, j, k] = crates[i, j, k].Component;
					}
					else
					{
						grid_memory.MemCrates[i, j, k] = Util.Component.None;
					}
					if (accessories[i, j, k] != null)
					{
						grid_memory.MemAccessories[i, j, k] = accessories[i, j, k].Component;
						grid_memory.AccessoryDirections[i, j, k] = accessories[i, j, k].Direction;
					}
					else
					{
						grid_memory.MemAccessories[i, j, k] = Util.Component.None;
					}
					if (loads[i, j, k] != null)
					{
						grid_memory.MemLoads[i, j, k] = loads[i, j, k].Component;
					}
					else
					{
						grid_memory.MemLoads[i, j, k] = Util.Component.None;
					}
				}
			}
		}
	}
	public bool InGrid(Vec3 pos)
	{
		(int h, int w, int l) = GameSave.GridSize;
		(int h_idx, int w_idx, int l_idx) = pos;
		return h_idx >= 0 && h_idx < h && w_idx >= 0 && w_idx < w && l_idx >= 0 && l_idx < l;
	}
	void SwitchLayer()
	{
		EventBus.Publish(new SwitchLayerEvent());
		if (activeLayerIndex != -1)
		{
			SetLayerActive(activeLayerIndex, false);
		}
		activeLayerIndex++;
		if (activeLayerIndex >= grids.GetLength(0))
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
	IEnumerator AttachToCarCore()
	{
		yield return CarCore.Inst.Build();
		for (int i = 0; i < crates.GetLength(0); i++)
		{
			for (int j = 0; j < crates.GetLength(1); j++)
			{
				for (int k = 0; k < crates.GetLength(2); k++)
				{
					if (crates[i, j, k] != null)
					{
						crates[i, j, k].transform.parent = CarCore.Inst.Container.transform;
					}
					if (accessories[i, j, k] != null)
					{
						accessories[i, j, k].transform.parent = CarCore.Inst.Container.transform;
					}
					if (loads[i, j, k] != null)
					{
						loads[i, j, k].transform.parent = CarCore.Inst.Container.transform;
					}
				}
			}
		}
		// CarCore.Inst.AttachPiggy();
		CarCore.Inst.Unfix();
	}
	public Vector3 ProbeTargetPos { get; private set; }
	void ProbeResize()
	{
		(int h, int w, int l) = GameSave.GridSize;
		Probe.transform.localScale = new Vector3(w, h, l);
		ProbeTargetPos = new Vector3(0, ((float)h - 1.0f) / 2.0f, 0);
		// MoveProbeToGrid();
	}
	public bool CollisionFlag { get; set; } = false;
	
	public MeshRenderer mesh_renderer;
	void ShowProbe()
	{
		mesh_renderer.enabled = true;
	}
	void HideProbe()
	{
		mesh_renderer.enabled = false;
	}
}
