﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffScreen : MonoBehaviour {

	private SpriteRenderer sprite_Renderer;

	void Awake () {
		sprite_Renderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes (Camera.main);

		if (!GeometryUtility.TestPlanesAABB (planes, sprite_Renderer.bounds)) {
			if (transform.position.x - Camera.main.transform.position.x < 0.0f) {
				CheckTile ();
			}
		}

	}

	void CheckTile () {
	
		if (this.tag == MyTags.ROAD) {
			
			Change (ref MapGenerator.instance.last_Pos_Of_Road_Tile,
				new Vector3(1.5f, 0f, 0f),
				ref MapGenerator.instance.last_Order_Of_Road);
			
		} else if (this.tag == MyTags.TOP_NEAR_GRASS) {
			
			Change (ref MapGenerator.instance.last_Pos_Of_Top_Near_Grass,
				new Vector3(1.2f, 0f, 0f),
				ref MapGenerator.instance.last_Order_Of_Top_Near_Grass);

		} else if (this.tag == MyTags.TOP_FAR_GRASS) {

			Change (ref MapGenerator.instance.last_Pos_Of_Top_Far_Grass,
				new Vector3(4.8f, 0f, 0f),
				ref MapGenerator.instance.last_Order_Of_Top_Far_Grass);

		} else if (this.tag == MyTags.BOTTOM_NEAR_GRASS) {

			Change (ref MapGenerator.instance.last_Pos_Of_Bottom_Near_Grass,
				new Vector3(1.2f, 0f, 0f),
				ref MapGenerator.instance.last_Order_Of_Bottom_Near_Grass);

		}
		 
	}

	void Change(ref Vector3 pos, Vector3 offSet, ref int orderLayer) {
		transform.position = pos;
		pos += offSet;

		sprite_Renderer.sortingOrder = orderLayer;

		orderLayer++;
	}

} // class






































