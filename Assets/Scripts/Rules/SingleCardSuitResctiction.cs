using UnityEngine;
using System.Collections;
using System;

public class SingleCardSuitResctiction : RuleBase
{
	
	public SingleCardSuitResctiction()
	{
		RandomiseRuleValues();
	}

	public override void RandomiseRuleValues()
	{
		cardValue = UnityEngine.Random.Range(1,14);

		string[] suits = new string[4]{"Hearts","Clubs","Diamonds","Spades"};
		cardSuit = suits[ UnityEngine.Random.Range(0,4) ];

		CheckRule = OnlyPlayChosenSuit;
		ruleDescription = "If a " + GetCardName(cardValue) + " is played, you can only play " + cardSuit + " directly after.";
	}

	bool OnlyPlayChosenSuit(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if(previousCard.Value == cardValue && proposedCard.Suit != cardSuit)
		{
			return false;
		}
		else
		{
			return true;
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
