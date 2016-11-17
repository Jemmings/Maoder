using UnityEngine;
using System.Collections;

public class CardLift : MonoBehaviour {

	private Vector2 downPosition;
	private Vector2 liftPosition;

	void OnEnable()
	{
		downPosition = transform.position;
		liftPosition = new Vector2(downPosition.x,downPosition.y + 1f);
	}
	

	void OnMouseOver()
	{
		transform.position = liftPosition;
	}

	void OnMouseExit()
	{
		transform.position = downPosition;
	}
}
