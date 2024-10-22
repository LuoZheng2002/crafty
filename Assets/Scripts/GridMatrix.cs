using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GridSelectionChangedEvent
{
	public GridCell gridCell;
    public GridSelectionChangedEvent(GridCell gridCell)
    {
        this.gridCell = gridCell;
    }
}


public class ContentSelectionChangedEvent
{
	public ImageDragHandler imageDragHandler;
    public ContentSelectionChangedEvent(ImageDragHandler imageDragHandler)
    {
		this.imageDragHandler = imageDragHandler;
    }
}

public class SwitchLayerEvent
{

}
public class GridMatrix : MonoBehaviour
{
	public bool first_person = true;
	public float camera_move_speed = 5.0f;
	public float camera_rotation_time = 0.5f;
	public GameObject gridPrefab;
	public int height = 2;
	public int width = 3;
	public int length = 5;
	GridCell[,,] grids;
	public float position_spring = 1000.0f;
	public float position_damper = 1000.0f;

	PiggyPreview piggyPreview;

	CratePreview[,,] crates;
	AccessoryPreview[,,] accessories;
	LoadPreview[,,] loads;

	// 
	Util.Content[,,] mem_crates;
	Util.Content[,,] mem_accessories;
	Util.Content[,,] mem_loads;
	int[,,] accessory_directions;
	int[,,] load_directions;
	bool wa;
	bool sd;
	// Util.GridContentInfo[,,] infos;

	GridCell lastSelectedGrid = null;
	ImageDragHandler currentImageDragHandler;
	public int activeLayerIndex = 0;
	private RaycastHit[] hits = new RaycastHit[10];

	// public event Action<GridCell> GridSelectionChanged;

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

	// instantiator
	Subscription<TrashEvent> s0;
	Subscription<GameStateChangedEvent> s1;
	Subscription<AddContentEvent> s2;
	Subscription<ContentTypeChangedEvent> s3;
	Subscription<ContentSelectionChangedEvent> s4;
	Subscription<SwitchLayerEvent> s5;
	Subscription<EraseChangedEvent> s6;

	private void Start()
	{
		Transform cube = transform.Find("Cube");
		Destroy(cube.gameObject);
		
	}
	void OnEraseChanged(EraseChangedEvent e)
	{
		if (e.active)
		{
			currentContentType = Util.ContentType.Erase;
		}
		else
		{
			currentContentType = Util.ContentType.None;
			if (lastSelectedGrid!= null)
			{
				Debug.Log("Erase called");
				var load = loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
				if (load != null)
				{
					EventBus.Publish(new ContentRecycleEvent(load.Content));
					Destroy(load.gameObject);
					if (load.Content == Util.Content.Pig)
					{
						EventBus.Publish(new PiggyRemovedEvent());
					}
					loads[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
				}
				else
				{
					var accessory = accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
					if (accessory != null)
					{
						EventBus.Publish(new ContentRecycleEvent(accessory.Content));
						Destroy(accessory.gameObject);
						accessories[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
					}
					var crate = crates[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx];
					if (crate != null)
					{
						EventBus.Publish(new ContentRecycleEvent(crate.Content));
						Destroy(crate.gameObject);
						crates[lastSelectedGrid.heightIdx, lastSelectedGrid.widthIdx, lastSelectedGrid.lengthIdx] = null;
					}
				}
				
			}
		}
	}

	IEnumerator RebuildVehicle()
	{
		yield return new WaitForSeconds(0.2f);
		if (mem_accessories != null)
		{
			Debug.Log("Rebuilding vehicle");
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					for (int k = 0; k < length; k++)
					{
						if (mem_accessories[i, j, k] != Util.Content.None)
						{
							Debug.Log("Rebuit an accessory");
							Util.Content content = mem_accessories[i, j, k];
							var inst = ContentInstantiator.Instance.InstantiateContent(content, transform, new Vector3(j, i, k), true, accessory_directions[i, j, k]) as AccessoryPreview;
							StartCoroutine(ChangePosition(inst, new Vector3(j, i, k)));
							accessories[i, j, k] = inst;
							Debug.Assert(inst != null);
							EventBus.Publish(new ContentUsedEvent(content));
						}
						if (mem_loads[i, j, k] != Util.Content.None)
						{
							Debug.Log("Rebuit a load");
							Util.Content content = mem_loads[i, j, k];
							var inst = ContentInstantiator.Instance.InstantiateContent(content, transform, new Vector3(j, i, k), true, load_directions[i, j, k]) as LoadPreview;
							StartCoroutine(ChangePosition(inst, new Vector3(j, i, k)));
							loads[i, j, k] = inst;
							Debug.Assert(inst != null);
							if (content == Util.Content.Pig)
							{
								EventBus.Publish(new PiggyInstantiatedEvent(inst as PiggyPreview));
							}
							EventBus.Publish(new ContentUsedEvent(content));
						}
						if (mem_crates[i, j, k] != Util.Content.None)
						{
							Debug.Log("Rebuit a crate");
							Util.Content content = mem_crates[i, j, k];
							var inst = ContentInstantiator.Instance.InstantiateContent(content, transform, new Vector3(j, i, k), true, 0) as CratePreview;
							StartCoroutine(ChangePosition(inst, new Vector3(j, i, k)));
							crates[i, j, k] = inst;
							Debug.Assert(inst != null);
							EventBus.Publish(new ContentUsedEvent(content));
						}
					}
				}
			}
		}
	}
	private void OnEnable()
	{
		Debug.Log("Grid matrix enabled");

		s0 = EventBus.Subscribe<TrashEvent>(OnTrash);
		s1 = EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
		s2 = EventBus.Subscribe<AddContentEvent>(AddContent);
		s3 = EventBus.Subscribe<ContentTypeChangedEvent>(OnContentTypeChanged);
		s4 = EventBus.Subscribe<ContentSelectionChangedEvent>(OnContentSelectionChanged);
		s5 = EventBus.Subscribe<SwitchLayerEvent>(OnSwitchLayer);
		s6 = EventBus.Subscribe<EraseChangedEvent>(OnEraseChanged);


		grids = new GridCell[height, width, length];
		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];

		
		// infos = new Util.GridContentInfo[height, width, length];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					Debug.Assert(gridPrefab != null);
					GameObject grid = Instantiate(gridPrefab, transform.position, Quaternion.identity);
					grid.transform.parent = transform;
					grid.transform.localPosition = new Vector3(j, i, k);
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
		StartCoroutine(RebuildVehicle());
	}
	private void OnDisable()
	{
		Debug.Log("Grid Matrix disabled");

		EventBus.Unsubscribe(s0);
		EventBus.Unsubscribe(s1);
		EventBus.Unsubscribe(s2);
		EventBus.Unsubscribe(s3);
		EventBus.Unsubscribe(s4);
		EventBus.Unsubscribe(s5);
		EventBus.Unsubscribe(s6);


		Trash();
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					GridCell grid = grids[i, j, k];
					Debug.Assert(grid != null);
					Destroy(grid.gameObject);
				}
			}
		}		
	}
	void OnContentTypeChanged(ContentTypeChangedEvent e)
	{
		currentContentType = e.contentType;
	}
	
	void OnContentSelectionChanged(ContentSelectionChangedEvent e)
	{
		currentImageDragHandler = e.imageDragHandler;
	}

	void BuildAndStickCrates(int h_idx, int w_idx, int l_idx)
	{
		CratePreview crate = crates[h_idx, w_idx, l_idx];
		if (crate != null)
		{
			mem_crates[h_idx, w_idx, l_idx] = crate.Content;
			crate.Build();
			List<(int, int, int)> deltas = new(){ (1, 0, 0), (0, 1, 0), (0, 0, 1) };
			foreach (var delta in deltas)
			{
				(int h, int w, int l) = delta;
				(int new_h, int new_w, int new_l) = (h_idx+h, w_idx + w, l_idx+l);
				if (InGrid(new_h, new_w, new_l) && crates[new_h, new_w, new_l]!=null)
				{
					Util.CreateJoint(crate, crates[new_h, new_w, new_l], position_spring, position_damper);
				}
			}
		}	
	}
	void BuildAndStickAccessories(int h_idx, int w_idx, int l_idx)
	{
		AccessoryPreview accessory = accessories[h_idx, w_idx, l_idx];
		if (accessory != null)
		{
			mem_accessories[h_idx, w_idx, l_idx] = accessory.Content;
			accessory_directions[h_idx, w_idx, l_idx] = accessory.Direction;
			// if (accessory.)
			(bool _wa, bool _sd) = accessory.GetWASD();
			if (_wa) wa = true;
			if (_sd) sd = true;
			accessory.Build();
			// to do
			(var h, var w, var l) = accessory.AttachDir();
			(var new_h, var new_w, var new_l) = (h + h_idx, w + w_idx, l + l_idx);
			if (InGrid(new_h, new_w, new_l) && crates[new_h, new_w, new_l] != null)
			{
				Util.CreateJoint(accessory, crates[new_h, new_w, new_l], position_spring, position_damper);
			}
		}
	}
	void BuildAndStickLoads(int h_idx, int w_idx, int l_idx)
	{
		LoadPreview load = loads[h_idx, w_idx, l_idx];
		if (load != null)
		{
			mem_loads[h_idx, w_idx, l_idx] = load.Content;
			load_directions[h_idx, w_idx, l_idx] = load.Direction;
			load.Build();
			// to do
			if (crates[h_idx, w_idx, l_idx] != null)
			{
				Util.CreateJoint(load, crates[h_idx, w_idx, l_idx], position_spring, position_damper);
			}
		}
	}
	void Build()
	{
		wa = false;
		sd = false;

		mem_crates = new Util.Content[height, width, length];
		mem_accessories = new Util.Content[height, width, length];
		mem_loads = new Util.Content[height, width, length];
		accessory_directions = new int[height, width, length];
		load_directions = new int[height, width, length];

		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickCrates(i, j, k);
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickAccessories(i, j, k);
				}
			}
		}
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
				{
					BuildAndStickLoads(i, j, k);
				}
			}
		}

		crates = new CratePreview[height, width, length];
		accessories = new AccessoryPreview[height, width, length];
		loads = new LoadPreview[height, width, length];
		StartCoroutine(UpdateWASD());
	}
	IEnumerator UpdateWASD()
	{
		yield return null;
		EventBus.Publish(new UpdateWASDEvent(wa, sd));
		enabled = false;
	}
	// build -> play -> end
	
	void OnTrash(TrashEvent e)
	{
		Trash();
	}
	void Trash()
	{
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < length; k++)
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
	}
	bool InGrid(int h, int w, int l)
	{
		return h >= 0 && h < height && w >= 0 && w < width && l >= 0 && l < length;
	}
	void OnGameStateChanged(GameStateChangedEvent e)
	{
		currentImageDragHandler = null;
		if (e.state == Util.GameStateType.Play)
		{
			Build();
		}		
	}
	bool switch_layer = false;
	void OnSwitchLayer(SwitchLayerEvent e)
	{
		switch_layer = true;
	}
	void TrySwitchLayer()
	{
		if (switch_layer)
		{
			switch_layer = false;
			Debug.Log("Switch layer called");
			SetLayerActive(activeLayerIndex, false);
			activeLayerIndex = (activeLayerIndex + 1) % height;
			SetLayerActive(activeLayerIndex, true);
		}		
	}
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			OnSwitchLayer(null);
		}
		TrySwitchLayer();
		UpdateActiveFromMouse();

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			int hitCount = Physics.RaycastNonAlloc(ray, hits);
			GridCell closestOccupiedGrid = null;
			GridCell closestEmptyGrid = null;
			float minOccupiedDistance = 100000.0f;
			float minEmptyDistance = 100000.0f;
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
				float distance = Util.GetDistanceFromRayToPoint(ray, hitObject.transform.position);
				if (accessories[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null
					&& loads[hitGrid.heightIdx, hitGrid.widthIdx, hitGrid.lengthIdx] == null)
				{
					if (distance < minEmptyDistance)
					{
						closestEmptyGrid = hitGrid;
						minEmptyDistance = distance;
					}
				}
				else
				{
					if (distance < minOccupiedDistance)
					{
						closestOccupiedGrid = hitGrid;
						minOccupiedDistance = distance;
					}
				}				
			}
			if (closestEmptyGrid != null)
			{
				Debug.Log("Clicking on empty grid");
				if (currentImageDragHandler != null)
				{
					Debug.Log("current image drage handler is not null");
					currentImageDragHandler.OnSelectionChanged(new GridSelectionChangedEvent(closestEmptyGrid));
					currentImageDragHandler.OnBeginDrag(null);
					currentImageDragHandler?.OnEndDrag(null);
				}
			}
			else if (closestOccupiedGrid!= null)
			{
				Debug.Log("Capture a component to change direction");
				DirectionalPreview directionalPreview = null;
				if (accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
				{
					directionalPreview = accessories[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
				}
				else if (loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx] != null)
				{
					directionalPreview = loads[closestOccupiedGrid.heightIdx, closestOccupiedGrid.widthIdx, closestOccupiedGrid.lengthIdx];
				}
				Debug.Assert(directionalPreview != null);
				directionalPreview.ChangeDirection();
			}
		}
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
			EventBus.Publish(new GridSelectionChangedEvent(closestGrid));
			// GridSelectionChanged?.Invoke(closestGrid);
			// fire event
		}
		else if (closestGrid != null && lastSelectedGrid == null)
		{
			closestGrid.Select();
			lastSelectedGrid = closestGrid;
			EventBus.Publish(new GridSelectionChangedEvent(closestGrid));
			// GridSelectionChanged?.Invoke(closestGrid);
		}
		else if (closestGrid == null && lastSelectedGrid != null)
		{
			lastSelectedGrid.Deselect();
			lastSelectedGrid = null;
			EventBus.Publish(new GridSelectionChangedEvent(closestGrid));
			// GridSelectionChanged?.Invoke(null);
		}
		else // ==null both
		{
			
		}
	}
	// very dirty fix; don't know the cause
	IEnumerator ChangePosition(ContentPreview content, Vector3 position)
	{
		for(int i = 0;i < 10 && content != null && enabled;i++)
		{
			content.transform.localPosition = position;
			content.transform.localRotation = Quaternion.identity;
			yield return new WaitForSeconds(0.1f);
		}
	}
	void AddContent(AddContentEvent e)
	{
		GameState.placed_a_component = true;
		Debug.Assert(e.content != null);
		GridCell grid = e.selectedGrid;
		(var h, var w, var l) = (grid.heightIdx, grid.widthIdx, grid.lengthIdx);
		Debug.Log($"h:{h}, w:{w}, l:{l}");
		Debug.Log($"{crates.GetLength(0)}, {crates.GetLength(1)}, {crates.GetLength(2)}");
		StartCoroutine(ChangePosition(e.content, new Vector3(w, h, l)));
		Debug.Log("Add content called");
		switch (e.contentType)
		{
			case Util.ContentType.Crate:
				CratePreview cratePreview = e.content as CratePreview;
				Debug.Assert(cratePreview != null);
				crates[h, w, l] = cratePreview;
				break;
			case Util.ContentType.Accessory:
				AccessoryPreview accessoryPreview = e.content as AccessoryPreview;
				Debug.Assert(accessoryPreview != null);
				accessories[h, w, l] = accessoryPreview;
				break;
			case Util.ContentType.Load:
				LoadPreview loadPreview = e.content as LoadPreview;
				Debug.Assert(loadPreview != null);
				loads[h, w, l] = loadPreview;
				PiggyPreview preview = loadPreview as PiggyPreview;
				if (preview != null)
				{
					piggyPreview = preview;
					EventBus.Publish(new PiggyInstantiatedEvent(piggyPreview));
				}
				break;
		}
	}
}