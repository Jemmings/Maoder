using UnityEngine;
using System.Collections;
using System;

public class NoMatchingValues : RuleBase
{
	public NoMatchingValues()
	{
		RandomiseRuleValues();
	}

	public override void RandomiseRuleValues()
	{
		cardValue = UnityEngine.Random.Range(1,14);
		cardSuit = "None";
		CheckRule = NoMatchingNumbers;
		ruleDescription = "You can't play a card of the same value as the previous card.";
	}

	bool NoMatchingNumbers(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if(previousCard.Value != proposedCard.Value)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}
