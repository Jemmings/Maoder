using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiPlayer : PlayerBase
{
	public AiPlayer(GameController controller, List<PlayingCard> deck,List<PlayingCard> playedCards,Vector2 handPosition,Vector2 handDirection,Quaternion handRotation)
		: base(controller,deck,playedCards,handPosition,handDirection,handRotation)
	{
		
	}

	public override void StateChange()
	{

	}


	public override void UpdateState()
	{
		base.UpdateState();

		//Physics2D.Raycast
		//Debug.Log("Awaiting Input");

		if(Input.GetKeyDown(KeyCode.Space))
		{
			TurnOver = true;
		}
	}
}
