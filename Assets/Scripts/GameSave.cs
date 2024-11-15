using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSave
{
    public static Vec3 GridSize { get; set; } = new(2, 2, 3);
    public static Util.Component[,,] MemCrates { get; set; }
	public static Util.Component[,,] MemAccessories { get; set; }
	public static Util.Component[,,] MemLoads { get; set; }
	public static int[,,] AccessoryDirections { get; set; }
	public static Dictionary<Util.Component, int> Inventory { get; set; } = new()
	{
		{Util.Component.Pig, 1 },
		{Util.Component.WoodenCrate, 9 },
		{Util.Component.Wheel, 4 },
		{Util.Component.TurnWheel, 4 },
		{Util.Component.MotorWheel, 4 },
		{Util.Component.Rocket, 8 },
		{Util.Component.Umbrella, 8 }
	};
}
