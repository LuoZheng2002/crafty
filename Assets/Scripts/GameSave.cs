using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSave
{
    public static Vec3 GridSize { get; set; } = new(2, 2, 3);
    public static Util.Component[,,] MemCrates { get; set; }
	public static Util.Component[,,] MemAccessories { get; set; }
	public static Util.Component[,,] MemLoads { get; set; }
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
}
