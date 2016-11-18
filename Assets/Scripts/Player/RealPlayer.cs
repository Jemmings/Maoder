using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RealPlayer : PlayerBase
{
	private LayerMask playingCardLayer;
	private Transform colliderHitTransform;
	private Vector3 colliderStartPoint;
	private bool playerChosen = true;

	public RealPlayer(GameController controller, List<PlayingCard> deck,List<PlayingCard> playedCards,Vector2 handPosition,Vector2 handDirection,Quaternion handRotation)
		: base(controller,deck,playedCards,handPosition,handDirection,handRotation)
	{
		playingCardLayer = ((1 << 8) | (1 << 9));
	}

	public override void StateChange()
	{
		// Set card colliders to true.
		EnableColliders(true);
		CheckForPenalty();
		playerChosen = false;
	}

	public override void UpdateState()
	{
		base.UpdateState();

		if(Input.GetKeyDown(KeyCode.Space))
		{
			TurnOver = true;
		}

		// delay waits for any penalty cards to be given. 
		if(!playerChosen && !delay)
		{
			CheckInput();
		}
	}

	void CheckInput()
	{
		Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		RaycastHit2D hit = Physics2D.Raycast(mousePos,Vector2.zero,100f,playingCardLayer);

		if(hit.collider != null && hit.collider.transform != colliderHitTransform && hit.collider.tag == "Playing Card")
		{
			if(colliderHitTransform != null)
			{
				colliderHitTransform.position = colliderStartPoint;
			}

			colliderHitTransform = hit.collider.transform;
			colliderStartPoint = hit.collider.transform.position;
			hit.collider.transform.position = hit.collider.transform.position + new Vector3(0,0.5f,0);
		}
		else if(hit.collider == null)
		{
			if(colliderHitTransform != null)
			{
				colliderHitTransform.position = colliderStartPoint;
			}

			colliderHitTransform = null;
		}

		if(Input.GetMouseButtonDown(0) && hit.collider != null)
		{
			if(hit.collider.tag == "Playing Card")
			{
				playerChosen = true;
				EnableColliders( false );
				PlayACard( hit.collider.gameObject );
			}
			else if(hit.collider.tag == "Deck")
			{
				playerChosen = true;
				PickUpACard(1,true);
			}

			colliderHitTransform = null;
		}
	}

	void CheckForPenalty()
	{
		// If the last played card is a penalty card, pickup.
		int penaltyAmount = controller.rules.CheckCardValidity( playedCards[playedCards.Count-1] );
		if(penaltyAmount > 0)
		{
			delay = true;
			PickUpACard(2, false);
		}
	}

	void EnableColliders(bool enable)
	{
		for(var i = 0; i < hand.Count; i++)
		{
			hand[i].CardGO.GetComponent<BoxCollider2D>().enabled = enable;
		}
	}
		
	void PlayACard(GameObject hitCardGO)
	{
		PlayingCard proposedCard = null;

		foreach(PlayingCard card in hand)
		{
			if(hitCardGO == card.CardGO)
			{
				proposedCard = card;
			}
		}

		// If the proposed card break the rules, pick up a penalty card.
		if(!controller.rules.CheckCardValidity( playedCards[playedCards.Count-1], proposedCard))
		{
			PickUpACard(2, true);
		}
		// Play the card.
		else
		{
			RemoveCard( proposedCard );
		}

	}

	void PickUpACard(int numberOfCards, bool end)
	{
		if(deck.Count < numberOfCards)
		{
			numberOfCards = deck.Count;
		}

		if(numberOfCards == 1)
		{
			AddCard( deck[0],end );
			deck.RemoveAt( 0 );
		}
		else if(numberOfCards > 1)
		{
			for(var i = 0; i < numberOfCards; i++)
			{
				hand.Add( deck[0] );
				deck.RemoveAt( 0 );
			}
			AddCard( numberOfCards,end );
		}

		if(!end)
		{
			EnableColliders( true );
		}
	}


}
