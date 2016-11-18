using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AiPlayer : PlayerBase
{
	private ShuffleInts shuffle;
	private bool penaltyChecked = false;

	public AiPlayer(GameController controller, List<PlayingCard> deck,List<PlayingCard> playedCards,Vector2 handPosition,Vector2 handDirection,Quaternion handRotation)
		: base(controller,deck,playedCards,handPosition,handDirection,handRotation)
	{
		shuffle = new ShuffleInts();
	}

	public override void StateChange()
	{
		CheckForPenalty( LastCardPlayed() );
		penaltyChecked = true;
	}


	public override void UpdateState()
	{
		base.UpdateState();

		if(Input.GetKeyDown(KeyCode.Space))
		{
			TurnOver = true;
		}

		if(!delay && penaltyChecked)
		{
			ChooseAction();
			penaltyChecked = false;
		}
	}

	void ChooseAction()
	{
		Debug.Log("Choosing Action");
		int[] playOrder = shuffle.Shuffle(hand.Count);

		for(var i = 0; i < playOrder.Length; i++)
		{
			// Check if the card breaks the rules, if it does, skip it.
			if(!controller.rules.CheckCardValidity( playedCards[playedCards.Count-1], hand[playOrder[i]]))
			{
				continue;
			}
			// If a suitable card is found, play it.
			else if(hand[playOrder[i]].Suit != "Joker")
			{
				Debug.Log("AI Playing card");
				RemoveCard( hand[playOrder[i]] );
				return;
			}
		}

		// If no suitable cards are found, play any wild cards.
		foreach(PlayingCard card in hand)
		{
			if(card.Suit == "Joker")
			{
				Debug.Log("AI Playing card");
				RemoveCard( card );
				return;
			}
		}
			
		// None of the cards passed the rules so pickup (if the deck isn't empty).
		if(deck.Count > 0)
		{
			AddCard( deck[0],true );
			deck.RemoveAt(0);
		}
		else
		{
			Debug.Log("AI Player knocking!");
			timeOffset = Time.time + 0.3f;
			delay = true;
		}

	}
}
