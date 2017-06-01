﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ResourceManager : MonoBehaviour {

	private UIManager uiM;

	void Awake() {
		uiM = GetComponent<UIManager>();
	}

	public enum ResourceGroupsEnum { Natural, Materials };

	public enum ResourcesEnum { Dirt, Stone, Granite, Limestone, Marble, Sandstone, Slate, Clay, Wood, Firewood };

	public List<ResourceGroup> resourceGroups = new List<ResourceGroup>();

	public class ResourceGroup {

		public ResourceGroupsEnum type;
		public string name;

		public List<ResourcesEnum> resourceTypes = new List<ResourcesEnum>();
		public List<Resource> resources = new List<Resource>();

		public ResourceGroup(List<string> resourceGroupData, ResourceManager rm) {
			type = (ResourceGroupsEnum)System.Enum.Parse(typeof(ResourceGroupsEnum),resourceGroupData[0]);
			name = type.ToString();

			List<string> resourceData = resourceGroupData[1].Split('`').ToList();
			foreach (string resourceString in resourceData) {
				Resource resource = new Resource(resourceString.Split('/').ToList(),this,rm);
				resourceTypes.Add(resource.type);
				resources.Add(resource);
				rm.resources.Add(resource);
			}
		}
	}

	public List<Resource> resources = new List<Resource>();

	public class Resource {
		public ResourcesEnum type;
		public string name;
		
		public ResourceGroup resourceGroup;

		public int value;

		public Resource(List<string> resourceData,ResourceGroup resourceGroup, ResourceManager rm) {
			type = (ResourcesEnum)System.Enum.Parse(typeof(ResourcesEnum),resourceData[0]);
			name = type.ToString();

			this.resourceGroup = resourceGroup;

			value = int.Parse(resourceData[1]);
		}
	}

	public void CreateResources() {
		List<string> resourceGroupsDataString = UnityEngine.Resources.Load<TextAsset>(@"Data/resources").text.Replace("\n",string.Empty).Replace("\t",string.Empty).Split('~').ToList();
		foreach (string resourceGroupDataString in resourceGroupsDataString) {
			List<string> resourceGroupData = resourceGroupDataString.Split(':').ToList();
			ResourceGroup resourceGroup = new ResourceGroup(resourceGroupData,this);
			resourceGroups.Add(resourceGroup);
		}
	}

	public Resource GetResourceByEnum(ResourcesEnum resourceEnum) {
		return resources.Find(o => o.type == resourceEnum);
	}

	public class ResourceAmount {
		public Resource resource;
		public int amount;
		public ResourceAmount(Resource resource,int amount) {
			this.resource = resource;
			this.amount = amount;
		}
	}

	public class ReservedResources {
		public List<ResourceAmount> resources = new List<ResourceAmount>();
		public ColonistManager.Colonist colonist;

		public ReservedResources(List<ResourceAmount> resourcesToReserve,ColonistManager.Colonist colonistReservingResources) {
			resources.AddRange(resourcesToReserve);
			colonist = colonistReservingResources;
		}
	}

	/*
	 * <Type> -> <SubType> -> <Object>
	*/
	public enum TileObjectPrefabGroupsEnum {
		Structure, Furniture
	};
	public enum TileObjectPrefabSubGroupsEnum {
		Walls, Doors, Floors, Containers
	};
	public enum TileObjectPrefabsEnum {
		StoneWall, WoodenWall, WoodenDoor, StoneFloor, WoodenFloor, WoodenChest
	};

	List<TileObjectPrefabsEnum> BitmaskingTileObjects = new List<TileObjectPrefabsEnum>() {
		TileObjectPrefabsEnum.StoneWall, TileObjectPrefabsEnum.WoodenWall, TileObjectPrefabsEnum.WoodenDoor, TileObjectPrefabsEnum.StoneFloor, TileObjectPrefabsEnum.WoodenFloor
	};
	List<TileObjectPrefabsEnum> FloorEquivalentTileObjects = new List<TileObjectPrefabsEnum>() {
		TileObjectPrefabsEnum.StoneFloor, TileObjectPrefabsEnum.WoodenFloor
	};
	List<TileObjectPrefabsEnum> WallEquivalentTileObjects = new List<TileObjectPrefabsEnum>() {
		TileObjectPrefabsEnum.StoneWall, TileObjectPrefabsEnum.WoodenWall
	};

	public List<TileObjectPrefabGroup> tileObjectPrefabGroups = new List<TileObjectPrefabGroup>();
	public List<TileObjectPrefab> tileObjectPrefabs = new List<TileObjectPrefab>();

	public void CreateTileObjectPrefabs() {
		List <string> tileObjectPrefabGroupsData = Resources.Load<TextAsset>(@"Data/tileobjectprefabs").text.Replace("\n",string.Empty).Replace("\t",string.Empty).Split(new string[] { "<Group>" },System.StringSplitOptions.RemoveEmptyEntries).ToList();
		foreach (string tileObjectPrefabGroupDataString in tileObjectPrefabGroupsData) {
			tileObjectPrefabGroups.Add(new TileObjectPrefabGroup(tileObjectPrefabGroupDataString));
		}
		uiM.CreateBuildMenuButtons();
	}

	public TileObjectPrefab GetTileObjectPrefabByEnum(TileObjectPrefabsEnum topEnum) {
		return tileObjectPrefabs.Find(top => top.type == topEnum);
	}

	public class TileObjectPrefabGroup {
		public TileObjectPrefabGroupsEnum type;
		public string name;

		public List<TileObjectPrefabSubGroup> tileObjectPrefabSubGroups = new List<TileObjectPrefabSubGroup>();

		public TileObjectPrefabGroup(string data) {
			List<string> tileObjectPrefabSubGroupsData = data.Split(new string[] { "<SubGroup>" },System.StringSplitOptions.RemoveEmptyEntries).ToList();

			type = (TileObjectPrefabGroupsEnum)System.Enum.Parse(typeof(TileObjectPrefabGroupsEnum),tileObjectPrefabSubGroupsData[0]);
			name = type.ToString();

			foreach (string tileObjectPrefabSubGroupDataString in tileObjectPrefabSubGroupsData.Skip(1)) {
				tileObjectPrefabSubGroups.Add(new TileObjectPrefabSubGroup(tileObjectPrefabSubGroupDataString,this));
			}
		}
	}

	public class TileObjectPrefabSubGroup {
		public TileObjectPrefabSubGroupsEnum type;
		public string name;

		public TileObjectPrefabGroup tileObjectPrefabGroup;
		public List<TileObjectPrefab> tileObjectPrefabs = new List<TileObjectPrefab>();

		public TileObjectPrefabSubGroup(string data,TileObjectPrefabGroup tileObjectPrefabGroup) {
			this.tileObjectPrefabGroup = tileObjectPrefabGroup;

			List<string> tileObjectPrefabsData = data.Split(new string[] { "<Object>" },System.StringSplitOptions.RemoveEmptyEntries).ToList();

			type = (TileObjectPrefabSubGroupsEnum)System.Enum.Parse(typeof(TileObjectPrefabSubGroupsEnum),tileObjectPrefabsData[0]);
			name = type.ToString();

			foreach (string tileObjectPrefabDataString in tileObjectPrefabsData.Skip(1)) {
				tileObjectPrefabs.Add(new TileObjectPrefab(tileObjectPrefabDataString,this));
			}
		}
	}

	

	public class TileObjectPrefab {

		private ResourceManager resourceM;
		private UIManager uiM;

		void GetScriptReferences() {

			GameObject GM = GameObject.Find("GM");

			resourceM = GM.GetComponent<ResourceManager>();
			uiM = GM.GetComponent<UIManager>();
		}

		public TileObjectPrefabsEnum type;
		public string name;

		public TileObjectPrefabSubGroup tileObjectPrefabSubGroup;

		public Sprite baseSprite;
		public List<Sprite> bitmaskSprites = new List<Sprite>();

		public int timeToBuild;
		public List<ResourceAmount> resourcesToBuild = new List<ResourceAmount>();
		public List<JobManager.SelectionModifiersEnum> selectionModifiers = new List<JobManager.SelectionModifiersEnum>();
		public JobManager.JobTypesEnum jobType;

		public float flammability;

		public bool walkable;
		public float walkSpeed;

		public int layer;

		public TileObjectPrefab(string data,TileObjectPrefabSubGroup tileObjectPrefabSubGroup) {

			GetScriptReferences();

			this.tileObjectPrefabSubGroup = tileObjectPrefabSubGroup;

			List<string> properties = data.Split('/').ToList();

			type = (TileObjectPrefabsEnum)System.Enum.Parse(typeof(TileObjectPrefabsEnum),properties[0]);
			name = uiM.SplitByCapitals(type.ToString());

			timeToBuild = int.Parse(properties[1]);

			if (float.Parse(properties[2].Split(',')[0]) != 0) {
				int resourceIndex = 0;
				foreach (string resourceName in properties[3].Split(',').ToList()) {
					resourcesToBuild.Add(new ResourceAmount(resourceM.GetResourceByEnum((ResourcesEnum)System.Enum.Parse(typeof(ResourcesEnum),resourceName)),int.Parse(properties[2].Split(',')[resourceIndex])));
					resourceIndex += 1;
				}
			}

			foreach (string selectionModifierString in properties[4].Split(',')) {
				selectionModifiers.Add((JobManager.SelectionModifiersEnum)System.Enum.Parse(typeof(JobManager.SelectionModifiersEnum),selectionModifierString));
			}

			jobType = (JobManager.JobTypesEnum)System.Enum.Parse(typeof(JobManager.JobTypesEnum),properties[5]);

			flammability = float.Parse(properties[6]);

			walkable = bool.Parse(properties[7]);
			walkSpeed = float.Parse(properties[8]);

			layer = int.Parse(properties[9]);

			baseSprite = Resources.Load<Sprite>(@"Sprites/TileObjects/" + name + "/" + name.Replace(' ','-') + "-base");
			bitmaskSprites = Resources.LoadAll<Sprite>(@"Sprites/TileObjects/" + name + "/" + name.Replace(' ','-') + "-bitmask").ToList();
			if (baseSprite == null && bitmaskSprites.Count > 0) {
				baseSprite = bitmaskSprites[0];
			}

			resourceM.tileObjectPrefabs.Add(this);
		}
	}

	public Dictionary<TileObjectPrefab,List<TileObjectInstance>> tileObjectInstances = new Dictionary<TileObjectPrefab,List<TileObjectInstance>>();

	public List<TileObjectInstance> GetTileObjectInstanceList(TileObjectInstance tileObjectInstance) {
		if (tileObjectInstances.ContainsKey(tileObjectInstance.prefab)) {
			return tileObjectInstances[tileObjectInstance.prefab];
		}
		print("Tried accessing a tile object instance which isn't already in the list...");
		return null;
	}

	public void AddTileObjectInstance(TileObjectInstance tileObjectInstance) {
		if (tileObjectInstances.ContainsKey(tileObjectInstance.prefab)) {
			tileObjectInstances[tileObjectInstance.prefab].Add(tileObjectInstance);
		} else {
			tileObjectInstances.Add(tileObjectInstance.prefab,new List<TileObjectInstance>() { tileObjectInstance });
		}
	}

	public void RemoveTileObjectInstance(TileObjectInstance tileObjectInstance) {
		if (tileObjectInstances.ContainsKey(tileObjectInstance.prefab)) {
			tileObjectInstances[tileObjectInstance.prefab].Remove(tileObjectInstance);
		} else {
			print("Tried removing a tile object instance which isn't in the list...");
		}
	}

	public class TileObjectInstance {

		private ResourceManager resourceM;

		void GetScriptReferences() {

			GameObject GM = GameObject.Find("GM");

			resourceM = GM.GetComponent<ResourceManager>();
		}

		public TileManager.Tile tile;
		
		public TileObjectPrefab prefab;
		public GameObject obj;

		public TileObjectInstance(TileObjectPrefab prefab, TileManager.Tile tile) {

			GetScriptReferences();

			this.prefab = prefab;
			this.tile = tile;

			obj = Instantiate(Resources.Load<GameObject>(@"Prefabs/Tile"),tile.obj.transform,false);
			obj.GetComponent<SpriteRenderer>().sortingOrder = 1;
			obj.GetComponent<SpriteRenderer>().sprite = prefab.baseSprite;
		}

		public void FinishCreation() {
			List<TileManager.Tile> bitmaskingTiles = new List<TileManager.Tile>() { tile };
			bitmaskingTiles.AddRange(tile.surroundingTiles);
			resourceM.Bitmask(bitmaskingTiles);
		}
	}

	public class Container : TileObjectInstance {
		public Inventory inventory;
		public Container(TileObjectPrefab prefab,TileManager.Tile tile) : base(prefab,tile) {
			this.prefab = prefab;
			this.tile = tile;
			inventory = new Inventory(null,this);
		}
	}

	public List<Container> containers = new List<Container>();

	public class Inventory {
		public List<ResourceAmount> resources = new List<ResourceAmount>();
		public List<ReservedResources> reservedResources = new List<ReservedResources>();

		public ColonistManager.Colonist colonist;
		public Container container;

		public Inventory(ColonistManager.Colonist colonist, Container container) {
			this.colonist = colonist;
			this.container = container;
		}

		public void ChangeResourceAmount(Resource resource,int amount) {
			ResourceAmount existingResourceAmount = resources.Find(ra => ra.resource == resource);
			if (existingResourceAmount != null) {
				if (amount >= 0 || (amount - existingResourceAmount.amount) >= 0) {
					print("Added an additional " + amount + " of " + resource.name + " to " + colonist.name);
					existingResourceAmount.amount += amount;
				} else {
					Debug.LogError("Trying to remove " + amount + " of " + resource.name + " on " + colonist.name + " when only " + existingResourceAmount.amount + " of that resource exist in this inventory");
				}
			} else {
				if (amount > 0) {
					print("Adding " + resource.name + " to " + colonist.name + " with a starting amount of " + amount);
					resources.Add(new ResourceAmount(resource,amount));
				} else if (amount < 0) {
					Debug.LogError("Trying to remove " + amount + " of " + resource.name + " that doesn't exist in " + colonist.name);
				}
			}
			if (existingResourceAmount.amount == 0) {
				print("Removed " + existingResourceAmount.resource.name + " from " + colonist.name + " as its amount was 0");
				resources.Remove(existingResourceAmount);
			} else if (existingResourceAmount.amount < 0) {
				Debug.LogError("There is a negative amount of " + resource.name + " on " + colonist.name + " with " + existingResourceAmount.amount);
			} else {
				print(colonist.name + " now has " + existingResourceAmount.amount + " of " + existingResourceAmount.resource.name);
			}
		}

		public bool ReserveResources(List<ResourceAmount> resourcesToReserve, ColonistManager.Colonist colonistReservingResources) {
			bool allResourcesFound = true;
			foreach (ResourceAmount raReserve in resourcesToReserve) {
				ResourceAmount raInventory = resources.Find(o => o.resource == raReserve.resource);
				if (!(raInventory != null && raInventory.amount >= raReserve.amount)) {
					allResourcesFound = false;
				}
			}
			if (allResourcesFound) {
				foreach (ResourceAmount raReserve in resourcesToReserve) {
					ResourceAmount raInventory = resources.Find(o => o.resource == raReserve.resource);
					ChangeResourceAmount(raInventory.resource,-raReserve.amount);
				}
				reservedResources.Add(new ReservedResources(resourcesToReserve,colonistReservingResources));
			}
			return allResourcesFound;
		}

		public List<ReservedResources> TakeReservedResources(ColonistManager.Colonist colonistReservingResources) {
			List<ReservedResources> reservedResourcesByColonist = new List<ReservedResources>();
			foreach (ReservedResources rr in reservedResources) {
				if (rr.colonist == colonistReservingResources) {
					reservedResourcesByColonist.Add(rr);
				}
			}
			foreach (ReservedResources rr in reservedResourcesByColonist) {
				reservedResources.Remove(rr);
			}
			return reservedResourcesByColonist;
		}
	}

	Dictionary<int,int> bitmaskMap = new Dictionary<int,int>() {
		{ 19,16 },{ 23,17 },{ 27,18 },{ 31,19 },{ 38,20 },{ 39,21 },{ 46,22 },
		{ 47,23 },{ 55,24 },{ 63,25 },{ 76,26 },{ 77,27 },{ 78,28 },{ 79,29 },
		{ 95,30 },{ 110,31 },{ 111,32 },{ 127,33 },{ 137,34 },{ 139,35 },{ 141,36 },
		{ 143,37 },{ 155,38 },{ 159,39 },{ 175,40 },{ 191,41 },{ 205,42 },{ 207,43 },
		{ 223,44 },{ 239,45 },{ 255,46 }
	};
	Dictionary<int,List<int>> diagonalCheckMap = new Dictionary<int,List<int>>() {
		{4,new List<int>() {0,1 } },
		{5,new List<int>() {1,2 } },
		{6,new List<int>() {2,3 } },
		{7,new List<int>() {3,0 } }
	};

	int BitSumTileObjects(List<TileObjectPrefabsEnum> compareTileObjectTypes,List<TileManager.Tile> tileSurroundingTiles) {
		//Dictionary<int,List<int>> layerTiles = new Dictionary<int,List<int>>();

		List<int> layers = new List<int>();
		foreach (TileManager.Tile tile in tileSurroundingTiles) {
			if (tile != null) {
				foreach (KeyValuePair<int,TileObjectInstance> kvp in tile.objectInstances) {
					if (!layers.Contains(kvp.Key)) {
						layers.Add(kvp.Key);
					}
				}
			}
		}
		layers.Sort();
		print(layers.Count);

		Dictionary<int,List<int>> layersSumTiles = new Dictionary<int,List<int>>();
		foreach (int layer in layers) {
			List<int> layerSumTiles = new List<int>() { 0,0,0,0,0,0,0,0 };
			for (int i = 0;i < tileSurroundingTiles.Count;i++) {
				if (tileSurroundingTiles[i] != null && tileSurroundingTiles[i].GetObjectInstanceAtLayer(layer) != null) {
					if (compareTileObjectTypes.Contains(tileSurroundingTiles[i].GetObjectInstanceAtLayer(layer).prefab.type)) {
						bool ignoreTile = false;
						if (compareTileObjectTypes.Contains(tileSurroundingTiles[i].GetObjectInstanceAtLayer(layer).prefab.type) && diagonalCheckMap.ContainsKey(i)) {
							List<TileManager.Tile> surroundingHorizontalTiles = new List<TileManager.Tile>() { tileSurroundingTiles[diagonalCheckMap[i][0]],tileSurroundingTiles[diagonalCheckMap[i][1]] };
							List<TileManager.Tile> similarTiles = surroundingHorizontalTiles.Where(tile => tile != null && tile.GetObjectInstanceAtLayer(layer) != null && compareTileObjectTypes.Contains(tile.GetObjectInstanceAtLayer(layer).prefab.type)).ToList();
							if (similarTiles.Count < 2) {
								ignoreTile = true;
							}
						}
						if (!ignoreTile) {
							layerSumTiles[i] = 1;
						}
					}
				} else {
					if (tileSurroundingTiles.Find(tile => tile != null && tileSurroundingTiles.IndexOf(tile) <= 3 && tile.GetObjectInstanceAtLayer(layer) != null && !compareTileObjectTypes.Contains(tile.GetObjectInstanceAtLayer(layer).prefab.type)) == null) {
						layerSumTiles[i] = 1;
					} else {
						if (i <= 3) {
							layerSumTiles[i] = 1;
						} else {
							List<TileManager.Tile> surroundingHorizontalTiles = new List<TileManager.Tile>() { tileSurroundingTiles[diagonalCheckMap[i][0]],tileSurroundingTiles[diagonalCheckMap[i][1]] };
							if (surroundingHorizontalTiles.Find(tile => tile != null && tile.GetObjectInstanceAtLayer(layer) != null && !compareTileObjectTypes.Contains(tile.GetObjectInstanceAtLayer(layer).prefab.type)) == null) {
								layerSumTiles[i] = 1;
							}
						}
					}
				}
			}
			layersSumTiles.Add(layer,layerSumTiles);
		}

		List<bool> sumTiles = new List<bool>() { false,false,false,false,false,false,false,false };

		foreach (KeyValuePair<int,List<int>> layerSumTiles in layersSumTiles) {
			foreach (TileObjectPrefabsEnum topEnum in compareTileObjectTypes) {
				TileObjectPrefab top = GetTileObjectPrefabByEnum(topEnum);
				if (top.layer == layerSumTiles.Key) {
					foreach (TileManager.Tile tile in tileSurroundingTiles) {
						TileObjectInstance topInstance = tile.GetAllObjectInstances().Find(instances => instances.prefab == top);
						if (topInstance != null) {
							if (layerSumTiles.Value[tileSurroundingTiles.IndexOf(tile)] > 0) {
								sumTiles[tileSurroundingTiles.IndexOf(tile)] = true;
							}
						}
					}
				}
			}
		}

		int sum = 0;

		for (int i = 0;i < sumTiles.Count;i++) {
			if (sumTiles[i]) {
				sum += Mathf.RoundToInt(Mathf.Pow(2,i));
			}
		}

		return sum;
	}

	void BitmaskTileObjects(TileObjectInstance objectInstance,bool includeDiagonalSurroundingTiles,bool customBitSumInputs,bool compareEquivalentTileObjects, List<TileObjectPrefabsEnum> customCompareTileObjectTypes) {
		int sum = 0;
		if (customBitSumInputs) {
			sum = BitSumTileObjects(customCompareTileObjectTypes,(includeDiagonalSurroundingTiles ? objectInstance.tile.surroundingTiles : objectInstance.tile.horizontalSurroundingTiles));
		} else {
			if (compareEquivalentTileObjects) {
				if (FloorEquivalentTileObjects.Contains(objectInstance.prefab.type)) {
					sum = BitSumTileObjects(FloorEquivalentTileObjects,(includeDiagonalSurroundingTiles ? objectInstance.tile.surroundingTiles : objectInstance.tile.horizontalSurroundingTiles));
				} else if (WallEquivalentTileObjects.Contains(objectInstance.prefab.type)) {
					sum = BitSumTileObjects(WallEquivalentTileObjects,(includeDiagonalSurroundingTiles ? objectInstance.tile.surroundingTiles : objectInstance.tile.horizontalSurroundingTiles));
				} else {
					sum = BitSumTileObjects(new List<TileObjectPrefabsEnum>() { objectInstance.prefab.type },(includeDiagonalSurroundingTiles ? objectInstance.tile.surroundingTiles : objectInstance.tile.horizontalSurroundingTiles));
				}
			} else {
				sum = BitSumTileObjects(new List<TileObjectPrefabsEnum>() { objectInstance.prefab.type },(includeDiagonalSurroundingTiles ? objectInstance.tile.surroundingTiles : objectInstance.tile.horizontalSurroundingTiles));
			}
		}
		SpriteRenderer oISR = objectInstance.obj.GetComponent<SpriteRenderer>();
		if (sum >= 16) {
			oISR.sprite = objectInstance.prefab.bitmaskSprites[bitmaskMap[sum]];
		} else {
			oISR.sprite = objectInstance.prefab.bitmaskSprites[sum];
		}
	}

	void Bitmask(List<TileManager.Tile> tilesToBitmask) {
		foreach (TileManager.Tile tile in tilesToBitmask) {
			if (tile != null && tile.GetAllObjectInstances().Count > 0) {
				foreach (TileObjectInstance tileObjectInstance in tile.GetAllObjectInstances()) {
					if (BitmaskingTileObjects.Contains(tileObjectInstance.prefab.type)) {
						BitmaskTileObjects(tileObjectInstance,true,false,false,null);
					} else {
						tileObjectInstance.obj.GetComponent<SpriteRenderer>().sprite = tileObjectInstance.prefab.baseSprite;
					}
				}
			}
		}
	}
}
