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
}
