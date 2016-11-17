using UnityEngine;
using System.Collections;

public class MustAlternateValues : RuleBase
{
	public MustAlternateValues()
	{
		RandomiseRuleValues();
	}

	public override void RandomiseRuleValues()
	{
		cardValue = 0;
		cardSuit = "None";

		int functionPick = UnityEngine.Random.Range(0,4);

		if(functionPick == 0)
		{
			CheckRule = AlternateSuits;
			ruleDescription = "Cannot play a card on another card with the same suit.";
		}
		else if(functionPick == 1)
		{
			CheckRule = AlternateOdds;
			ruleDescription = "Cannot play a card with an odd value on another card with an odd value.";
		}
		else if(functionPick == 2)
		{
			CheckRule = AlternateEvens;
			ruleDescription = "Cannot play a card with an even value on another card with an even value.";
		}
		else
		{
			CheckRule = AlternateColours;
			ruleDescription = "Cannot play a card on another card that has the same color suit.";
		}
	}

	bool AlternateSuits(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if(previousCard.Suit == proposedCard.Suit)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	bool AlternateOdds(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if(previousCard.Value <= 10 && proposedCard.Value <= 10 && previousCard.Value % 2 != 0 && proposedCard.Value % 2 != 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	bool AlternateEvens(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if(previousCard.Value <= 10 && proposedCard.Value <= 10 && previousCard.Value % 2 == 0 && proposedCard.Value % 2 == 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	bool AlternateColours(PlayingCard previousCard, PlayingCard proposedCard)
	{
		if(proposedCard.Suit == "Joker")
		{
			return true;
		}
		else if((previousCard.Suit == "Diamonds" || previousCard.Suit == "Hearts") && (proposedCard.Suit == "Clubs" || proposedCard.Suit == "Spades"))
		{
			return true;
		}
		else if((previousCard.Suit == "Clubs" || previousCard.Suit == "Spades") && (proposedCard.Suit == "Diamonds" || proposedCard.Suit == "Hearts"))
		{
			return true;	
		}
		else
		{
			return false;
		}
	}
}
