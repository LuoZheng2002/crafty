using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSave
{
	public static void IncrementGridSize(int delta_h, int delta_w, int delta_l)
	{
		GridSize.l += delta_l;
		GridSize.w += delta_w;
		GridSize.h += delta_h;
		EventBus.Publish(new GridMatrixSizeChangedEvent());
	}

	public static Vec3 GridSize { get; private set; } = new(2, 2, 3);
    public static Util.Component[,,] MemCrates { get; set; }
	public static Util.Component[,,] MemAccessories { get; set; }
	public static Util.Component[,,] MemLoads { get; set; }
	public static void ExpandMemory()
	{
		Util.Component[,,] temp_crates = (Util.Component[,,])MemCrates.Clone();
		Util.Component[,,] temp_accessories = (Util.Component[,,])MemAccessories.Clone();
		Util.Component[,,] temp_loads = (Util.Component[,,])MemLoads.Clone();
		int[,,] temp_directions = (int[,,])AccessoryDirections.Clone();
		MemCrates = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
		MemAccessories = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
		MemLoads = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
		AccessoryDirections = new int[GridSize.h, GridSize.w, GridSize.l];
		for (int i = 0; i < temp_crates.GetLength(0); i++)
		{
			for (int j = 0; j < temp_crates.GetLength(1); j++)
			{
				for (int k = 0; k < temp_crates.GetLength(2); k++)
				{
					if (temp_crates[i, j, k] != Util.Component.None)
					{
						Debug.Log("Has a crate!");
					}
					MemCrates[i, j, k] = temp_crates[i, j, k];
					MemAccessories[i, j, k] = temp_accessories[i, j, k];
					MemLoads[i, j, k] = temp_loads[i, j, k];
					AccessoryDirections[i, j, k] = temp_directions[i, j, k];
				}
			}
		}
	}
	public static void ClearMemory()
	{
		for (int i = 0; i < MemCrates.GetLength(0); i++)
		{
			for (int j = 0; j < MemCrates.GetLength(1); j++)
			{
				for (int k = 0; k < MemCrates.GetLength(2); k++)
				{
					MemCrates[i, j, k] = Util.Component.None;
					MemAccessories[i, j, k] = Util.Component.None;
					MemLoads[i, j, k] = Util.Component.None;
				}
			}
		}
	}
	public static int[,,] AccessoryDirections { get; set; }
	public static Dictionary<Util.Component, int> Inventory { get; set; } = new()
	{
		{Util.Component.Pig, 1 },
		{Util.Component.WoodenCrate, 6 },
		{Util.Component.Wheel, 4 },
		{Util.Component.TurnWheel, 0 },
		{Util.Component.MotorWheel, 0 },
		{Util.Component.Rocket, 0 },
		{Util.Component.Umbrella, 0 }
	};
	public static bool IsMainStory { get; set; } = true;
	public static Util.WaypointName CurrentCheckpoint { get; set; } = Util.WaypointName.None;
}
