using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator instance;

	public GameObject
		roadPrefab,
		grass_Prefab,
		groundPrefab_1,
		groundPrefab_2,
		groundPrefab_3,
		groundPrefab_4,
		grass_Bottom_Prefab;

	public GameObject
		road_Holder,
		top_Near_Side_Walk_Holder,
		top_Far_Side_Walk_Holder,
		bottom_Near_Side_Walk_Holder;

	public int
		start_Road_Tile_Amount,  // initialization number of ' road ' tiles
		start_Grass_Tile_Amount,   // initialization number of ' grass ' tiles
		start_Ground3_Tile_Amount,   // initialization number of ' ground3 ' tiles
		start_Land_Tile_Amount;  // initialization number of ' land ' tiles

	public List<GameObject>
		road_Tiles,
		top_Near_Grass_Tiles,
		top_Far_Grass_Tiles,
		bottom_Near_Grass_Tiles;
		

	// positions for ground1 on top from 0 to startGround3Tile
	public int[] pos_For_Top_Ground_1;

	// positions for ground2 on top from 0 to startGround3Tile
	public int[] pos_For_Top_Ground_2;

	// positions for ground4 on top from 0 to startGround3Tile
	public int[] pos_For_Top_Ground_4;

	

	// position for road tile on road from 0 to startRoadTile
	public int pos_For_Road_Tile_1;

	// position for road tile on road from 0 to startRoadTile
	public int pos_For_Road_Tile_2;

	// position for road tile on road from 0 to startRoadTile
	public int pos_For_Road_Tile_3;

	

	[HideInInspector]
	public Vector3
		last_Pos_Of_Road_Tile,
		last_Pos_Of_Top_Near_Grass,
		last_Pos_Of_Top_Far_Grass,
		last_Pos_Of_Bottom_Near_Grass;

	[HideInInspector]
	public int
		last_Order_Of_Road,
		last_Order_Of_Top_Near_Grass,
		last_Order_Of_Top_Far_Grass,
		last_Order_Of_Bottom_Near_Grass;

	void Awake() {
		MakeInstance ();
	}

	void Start () {
		Initialize ();
	}

	void MakeInstance() {
		if (instance == null) {
			instance = this;
		} else if (instance != null) {
			Destroy (gameObject);
		}
	}

	void Initialize() {

        // create road
        InitializePlatform(roadPrefab, ref last_Pos_Of_Road_Tile, roadPrefab.transform.position,
			start_Road_Tile_Amount, road_Holder, ref road_Tiles, ref last_Order_Of_Road,
			new Vector3(1.5f, 0f, 0f));
        // create grass top near road
        InitializePlatform(grass_Prefab, ref last_Pos_Of_Top_Near_Grass, grass_Prefab.transform.position,
			start_Grass_Tile_Amount, top_Near_Side_Walk_Holder, ref top_Near_Grass_Tiles,
			ref last_Order_Of_Top_Near_Grass, new Vector3(1.2f, 0f, 0f));
        // create ground top far from road
        InitializePlatform(groundPrefab_3, ref last_Pos_Of_Top_Far_Grass, groundPrefab_3.transform.position,
			start_Ground3_Tile_Amount, top_Far_Side_Walk_Holder, ref top_Far_Grass_Tiles,
			ref last_Order_Of_Top_Far_Grass, new Vector3(4.8f, 0f, 0f));
        // create grass bottom near road
        InitializePlatform(grass_Bottom_Prefab, ref last_Pos_Of_Bottom_Near_Grass,
			grass_Bottom_Prefab.transform.position,
			start_Grass_Tile_Amount, bottom_Near_Side_Walk_Holder, ref bottom_Near_Grass_Tiles,
			ref last_Order_Of_Bottom_Near_Grass, new Vector3(1.2f, 0f, 0f));


		
	} // Initialize

	//create list tile inside holder
	void InitializePlatform(GameObject prefab, ref Vector3 last_Pos, Vector3 current_Prefab_Pos,
		int amountTile, GameObject holder, ref List<GameObject> list_Tile, ref int last_Order, Vector3 offset) {

		int orderAppearInLayer = 0;
		last_Pos = current_Prefab_Pos; 

		for (int i = 0; i < amountTile; i++) {

			GameObject firstTileCreated = Instantiate (prefab, last_Pos, prefab.transform.rotation) as GameObject;
			firstTileCreated.GetComponent<SpriteRenderer> ().sortingOrder = orderAppearInLayer;

			

			if (firstTileCreated.tag == MyTags.TOP_FAR_GRASS) {
				//in case clone is `ground3`
				// create child ground on parent ground (clone)
				CreateGround(ref firstTileCreated, ref orderAppearInLayer);
				
			}
            //in case clone is road or land -> just create continously
            firstTileCreated.transform.SetParent (holder.transform);
			list_Tile.Add (firstTileCreated);

			orderAppearInLayer += 1;
			last_Order = orderAppearInLayer;

			last_Pos += offset;

		} // FOR LOOP

	} // InitializePlatform

    // create big grass with tree on top (near road)

	void CreateTreeOrGround(GameObject prefab, ref GameObject parent_Tile, Vector3 localPos) {

		GameObject clone_Child = Instantiate (prefab, parent_Tile.transform.position, prefab.transform.rotation)
			as GameObject;

		SpriteRenderer parentCloneRenderer = parent_Tile.GetComponent<SpriteRenderer> ();
		SpriteRenderer childCloneRenderer = clone_Child.GetComponent<SpriteRenderer> ();
        // in case tree prefab and ground prefab
        childCloneRenderer.sortingOrder = parentCloneRenderer.sortingOrder;
		
        clone_Child.transform.SetParent (parent_Tile.transform);
		clone_Child.transform.localPosition = localPos;

			parentCloneRenderer.enabled = false;

	} // CreateTreeOrGround

	void CreateGround(ref GameObject firstTile, ref int orderInLayer) {

		for (int i = 0; i < pos_For_Top_Ground_1.Length; i++) {
			if (orderInLayer == pos_For_Top_Ground_1 [i]) {
				
				CreateTreeOrGround (groundPrefab_1, ref firstTile, Vector3.zero);
				break;

			}
		}

		for (int i = 0; i < pos_For_Top_Ground_2.Length; i++) {
			if (orderInLayer == pos_For_Top_Ground_2 [i]) {

				CreateTreeOrGround (groundPrefab_2, ref firstTile, Vector3.zero);
				break;

			}
		}

		for (int i = 0; i < pos_For_Top_Ground_4.Length; i++) {
			if (orderInLayer == pos_For_Top_Ground_4 [i]) {

				CreateTreeOrGround (groundPrefab_4, ref firstTile, Vector3.zero);
				break;

			}
		}

	} // Create Ground


} // class


























































