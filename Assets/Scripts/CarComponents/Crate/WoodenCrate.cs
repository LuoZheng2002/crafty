using Unity;
using UnityEngine;

public class WoodenCrate: CrateBase
{
	public override Util.Component Component => Util.Component.WoodenCrate;
	private void Start()
	{
		particle_system.gameObject.SetActive(false);
	}
}