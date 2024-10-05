using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
	public float interval = 2.0f;
    public GameObject _1By3_1;
	public GameObject _1By3_2;
	public GameObject _1By3_3;
	public GameObject _2By3_1;
	public GameObject _2By3_2;
	public GameObject _2By3_3;
	public GameObject _3By3_1;
	public GameObject _3By3_2;
	public GameObject _3By3_3;
	public GameObject _1By3Prefab;
	public GameObject _2By3Prefab;
	public GameObject _3By3Prefab;
	List<GameObject> _1By3s;
	List<GameObject> _2By3s;
	List<GameObject> _3By3s;
	private void Start()
	{
		_1By3s = new() { _1By3_1, _1By3_2, _1By3_3 };
		_2By3s = new() { _2By3_1, _2By3_2, _2By3_3 };
		_3By3s = new() { _3By3_1, _3By3_2, _3By3_3 };
		StartCoroutine(SpawnRepeatedely());
	}
	IEnumerator SpawnRepeatedely()
	{
		while (true)
		{
			int random1 = Random.Range(0, 3);
			int random2 = Random.Range(0, 3);
			switch (random1)
			{
				case 0:
					{
						GameObject pos = _1By3s[random2];
						Instantiate(_1By3Prefab, pos.transform.position + new Vector3(0, 1, 0), pos.transform.rotation);
						break;
					}
				case 1:
					{
						GameObject pos = _2By3s[random2];
						Instantiate(_2By3Prefab, pos.transform.position + new Vector3(0, 1, 0), pos.transform.rotation);
						break;
					}
				case 2:
					{
						GameObject pos = _3By3s[random2];
						Instantiate(_3By3Prefab, pos.transform.position + new Vector3(0, 1, 0), pos.transform.rotation);
						break;
					}
			}
			yield return new WaitForSeconds(interval);
		}
	}
}
