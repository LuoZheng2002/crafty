using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GameSave
{
	public class GridMemory
	{
		public Util.Component[,,] MemCrates { get; set; }
		public Util.Component[,,] MemAccessories { get; set; }
		public Util.Component[,,] MemLoads { get; set; }
		public int[,,] AccessoryDirections { get; set; }
		public GridMemory()
		{
			MemCrates = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
			MemAccessories = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
			MemLoads = new Util.Component[GridSize.h, GridSize.w, GridSize.l];
			AccessoryDirections = new int[GridSize.h, GridSize.w, GridSize.l];
			EventBus.Subscribe<GridMatrixSizeChangedEvent>(OnGridSizeChanged);
		}
		void OnGridSizeChanged(GridMatrixSizeChangedEvent e)
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
		public bool Empty()
		{
			bool empty = true;
			for (int i = 0; i < MemCrates.GetLength(0); i++)
			{
				for (int j = 0; j < MemCrates.GetLength(1); j++)
				{
					for (int k = 0; k < MemCrates.GetLength(2); k++)
					{
						if (MemCrates[i, j, k] != Util.Component.None)
						{
							empty = false;
							break;
						}
						if (MemAccessories[i, j, k] != Util.Component.None)
						{
							empty = false;
							break;
						}
						if (MemLoads[i, j, k] != Util.Component.None)
						{
							empty = false;
							break;
						}
					}
				}
			}
			return empty;
		}
		public void Clear()
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
		public void Memorize()
		{
			GridMatrix.Inst.Memorize(this);
		}

	}
	public static Vec3 GridSize { get; private set; } = new(2, 2, 3);
	public static GridMemory[] GridMemories { get; set; } = new GridMemory[3];
	public static GridMemory CurrentMemory => GridMemories[GridMatrix.Inst.DesignNumber];
	static GameSave()
	{
		for (int i = 0; i < GridMemories.Length; i++)
		{
			GridMemories[i] = new GridMemory();
		}
	}
	public static void IncrementGridSize(int delta_h, int delta_w, int delta_l)
	{
		GridSize.l += delta_l;
		GridSize.w += delta_w;
		GridSize.h += delta_h;
		EventBus.Publish(new GridMatrixSizeChangedEvent());
	}

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
	public  static Util.WaypointName CurrentCheckpoint { get; set; } = Util.WaypointName.None;
	public static bool BreakTutorialWatched { get; set; } = false;

	static Util.QuestName current_quest_name = Util.QuestName.None;
	public static Util.QuestName CurrentQuestName
	{
		get { return current_quest_name; }
		set
		{
			current_quest_name = value;
			// to do
		}
	}
}
