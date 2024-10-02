using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GridMatrix : MonoBehaviour
{
	public GameObject gridPrefab;
	public int height = 2;
	public int width = 3;
	public int length = 5;
	GridCell[,,] grids;

	PiggyPreview piggyPreview;

	CratePreview[,,] crates;
	AccessoryPreview[,,] accessories;
	LoadPreview[,,] loads;

	// Util.GridContentInfo[,,] infos;

	GridCell lastSelectedGrid = null;
	public int activeLayerIndex = 0;
	private RaycastHit[] hits = new RaycastHit[10];

	public event Action<GridCell> GridSelectionChanged;

	public Util.ContentType currentContentType;
	void SetLayerActive(int index, bool active)
	{		
		if (index >= height)
		{
			Debug.LogWarning("Layer out of bound");
		}
		for(int i = 0; i < width; i++)
		{
			for (int j = 0; j < length; j++)
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
	private void Start()
	{
		grids = new GridCell[height, width, length];
		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];
		// infos = new Util.GridContentInfo[height, width, length];
		for(int i = 0;i < height; i++)
		{
			for(int j = 0;j < width; j++)
			{
				for (int k = 0;k < length; k++)
				{
					Debug.Assert(gridPrefab != null);
					GameObject grid = Instantiate(gridPrefab, transform.position + new Vector3(j, i, k), Quaternion.identity);
					Debug.Assert(grid != null);
					GridCell gridComponent = grid.GetComponent<GridCell>();
					Debug.Assert(gridComponent != null);
					gridComponent.heightIdx = i;
					gridComponent.widthIdx = j;
					gridComponent.lengthIdx = k;
					grids[i, j, k] = gridComponent;
				}
			}
		}
		SetLayerActive(activeLayerIndex, true);
	}
	void CreateJoint(MonoBehaviour a, MonoBehaviour b)
	{
		Debug.Log("Added a configurable joint");
		ConfigurableJoint configurableJoint = (a as MonoBehaviour).AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = (b as MonoBehaviour).GetComponent<Rigidbody>();
		JointDrive drive = new JointDrive();
		drive.positionSpring = 1000;
		drive.maximumForce = 1000000;
		configurableJoint.xDrive = drive;
		configurableJoint.yDrive = drive;
		configurableJoint.zDrive = drive;
		configurableJoint.rotationDriveMode = RotationDriveMode.XYAndZ;
		configurableJoint.angularXDrive = drive;
		configurableJoint.angularYZDrive = drive;
	}
	void Build()
	{
		for(int i = 0;i < height;i++)
		{
			for(int j = 0; j < width; j++)
			{
				for(int k = 0; k < length; k++)
				{
					CratePreview crate = crates[i, j, k];
					if (crates[i, j, k] != null)
					{
						crates[i, j, k].Build();
						if (i+1 < height && crates[i+1, j, k] != null)
						{
							CreateJoint(crate, crates[i + 1, j, k] );
						}
						if (j+1 < width && crates[i, j+1, k] != null)
						{
							CreateJoint(crate, crates[i, j + 1, k] );
						}
						if (k+1 < length && crates[i, j, k+1] != null)
						{
							CreateJoint(crate, crates[i, j, k + 1]);
						}
					}
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{ 
					AccessoryPreview accessory = accessories[i, j, k];
					if (accessory != null)
					{
						accessory.Build();
						// to do
						if (i + 1 < height && crates[i + 1, j, k] != null)
						{
							CreateJoint(accessory, crates[i + 1, j, k]);
						}
					}
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					LoadPreview load = loads[i, j, k];
					if (load != null)
					{
						load.Build();
						// to do
						if (crates[i, j, k] != null)
						{
							CreateJoint(load, crates[i, j, k]);
						}
					}
				}
			}
		}

		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SetLayerActive(activeLayerIndex, false);
			activeLayerIndex = (activeLayerIndex + 1) % height;
			SetLayerActive(activeLayerIndex, true);
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Build();
			if (piggyPreview!= null)
			{
				Camera.main.transform.parent = piggyPreview.transform;
				piggyPreview = null;
			}
		}
		UpdateActiveFromMouse();
	}

	void UpdateActiveFromMouse()
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
			if ((currentContentType == Util.ContentType.None || currentContentType == Util.ContentType.Accessory) && Occupied(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (currentContentType == Util.ContentType.Crate && !AllowCrate(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
				continue;
			}
			if (currentContentType == Util.ContentType.Load && !AllowLoad(hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx))
			{
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
		if (closestGrid != null && lastSelectedGrid != null && lastSelectedGrid == closestGrid)
		{
			// 
		}
		else if (closestGrid != null && lastSelectedGrid != null && lastSelectedGrid != closestGrid)
		{
			lastSelectedGrid.Deselect();
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			GridSelectionChanged?.Invoke(closestGrid);
			// fire event
		}
		else if (closestGrid != null && lastSelectedGrid == null)
		{
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			GridSelectionChanged?.Invoke(closestGrid);
		}
		else if (closestGrid == null && lastSelectedGrid != null)
		{
			lastSelectedGrid.Deselect();
			lastSelectedGrid = null;
			GridSelectionChanged?.Invoke(null);
		}
		else // ==null both
		{
			
		}
	}
	public void AddContent(Util.ContentType contentType, ContentPreview content)
	{
		Debug.Assert(content != null);
		Debug.Assert(lastSelectedGrid != null);
		(var h, var w, var l) = (lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx);
		switch (contentType)
		{
			case Util.ContentType.Crate:
				CratePreview cratePreview = content as CratePreview;
				Debug.Assert(cratePreview != null);
				crates[h, w, l] = cratePreview;
				break;
			case Util.ContentType.Accessory:
				AccessoryPreview accessoryPreview = content as AccessoryPreview;
				Debug.Assert(accessoryPreview != null);
				accessories[h, w, l] = accessoryPreview;
				break;
			case Util.ContentType.Load:
				LoadPreview loadPreview = content as LoadPreview;
				Debug.Assert(loadPreview != null);
				loads[h, w, l] = loadPreview;
				PiggyPreview preview = loadPreview as PiggyPreview;
				if (preview != null)
				{
					piggyPreview = preview;
				}
				break;
		}
	}
}