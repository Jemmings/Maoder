using UnityEngine;
using System.Collections;

public class ValueRestrictionPerSuit : RuleBase
{
	public ValueRestrictionPerSuit ()
	{
		RandomiseRuleValues();
	}

	public override void RandomiseRuleValues()
	{
		string[] suits = new string[4]{"Hearts","Clubs","Diamonds","Spades"};
		cardSuit = suits[ UnityEngine.Random.Range(0,4) ];
		cardValue = 0;

		int functionPick = UnityEngine.Random.Range(0,2);

		if(functionPick == 0)
		{
			// Only higher cards.
			CheckRule = HigherAfterSuit;
			ruleDescription = "If " + cardSuit + " are played, the next card must be a higher number or a face card.";
		}
		else
		{
			// Only lower cards.
			CheckRule = LowerAfterSuit;
			ruleDescription = "If " + cardSuit + " are played, the next card must be a lower number or a face card.";
		}
	}

	bool HigherAfterSuit(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker" || proposedCard.Value > 10)
		{
			return true;
		}
		else if(previousCard.Suit == cardSuit && proposedCard.Value < previousCard.Value)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	bool LowerAfterSuit(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker" || proposedCard.Value > 10)
		{
			return true;
		}
		else if(previousCard.Suit == cardSuit && proposedCard.Value > previousCard.Value)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
}
