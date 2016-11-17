using UnityEngine;
using System.Collections;

public class PickUpCards : PenaltyBase 
{
	public PickUpCards()
	{
		RandomiseRuleValues();
	}

	public override void RandomiseRuleValues()
	{
		int functionPick = UnityEngine.Random.Range(0,2);

		if(functionPick == 0)
		{
			cardValue = UnityEngine.Random.Range(1,14);
			cardSuit = "None";
			penaltyAmount = Random.Range(1,3);
			penaltyDescription = "If a " + GetCardName(cardValue) + " is played, the next player picks up " + penaltyAmount.ToString() + " card(s).";
			CheckPenalty = ValueBasedPickup;
		}
		else
		{
			string[] suits = new string[4]{"Hearts","Clubs","Diamonds","Spades"};
			cardSuit = suits[ UnityEngine.Random.Range(0,4) ];
			cardValue = 0;
			penaltyAmount = 1;
			penaltyDescription = "If " + cardSuit + " are played, the next player picks up 1 card.";
			CheckPenalty = SuitBasedPickup;
		}
	}

	int ValueBasedPickup(PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return 0;
		}
		else if(proposedCard.Value == cardValue )
		{
			return penaltyAmount;
		}
		else
		{
			return 0;
		}
	}

	int SuitBasedPickup(PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return 0;
		}
		else if(proposedCard.Suit == cardSuit )
		{
			return penaltyAmount;
		}
		else
		{
			return 0;
		}
	}
		
	string GetCardName(int value)
	{
		switch(value)
		{
		case 1:
			return "Ace";
		case 2:
			return "2";
		case 3:
			return "3";
		case 4:
			return "4";
		case 5:
			return "5";
		case 6:
			return "6";
		case 7:
			return "7";
		case 8:
			return "8";
		case 9:
			return "9";
		case 10:
			return "10";
		case 11:
			return "Jack";
		case 12:
			return "Queen";
		case 13:
			return "King";
		default:
			return "Ace";
		}
	}
}
