using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicRuleset : IRuleset
{
	//RuleBase singleCardSuitRestriction, noMatchingValues, valueRestrictionPerSuit,mustAlternateValues;
	//PenaltyBase pickUpCards;
	List<RuleBase> rulesList = new List<RuleBase>();
	List<PenaltyBase> penaltyList = new List<PenaltyBase>();

	public BasicRuleset()
	{
		// Rules.
		//singleCardSuitRestriction = new SingleCardSuitResctiction();
		//noMatchingValues = new NoMatchingValues();
		//valueRestrictionPerSuit = new ValueRestrictionPerSuit();
		//mustAlternateValues = new MustAlternateValues();

		// Penalties.
		//pickUpCards = new PickUpCards();


		CreateRuleset( Shuffle(4) );
		CreatePenaltySet(2);
	}


	public void CreateRuleset(int[] ruleIDs)
	{
		for(var i = 0; i < ruleIDs.Length; i++)
		{
			if(ruleIDs[i] == 0)
			{
				rulesList.Add( new SingleCardSuitResctiction() );
			}
			else if(ruleIDs[i] == 1)
			{
				rulesList.Add( new NoMatchingValues() );
			}
			else if(ruleIDs[i] == 2)
			{
				rulesList.Add( new ValueRestrictionPerSuit() );
			}
			else
			{
				rulesList.Add( new MustAlternateValues() );
			}
		}

		Debug.Log("!---RULES---!");
		foreach(RuleBase rule in rulesList)
		{
			Debug.Log(rule.RuleDescription);
		}
	}

	public void CreatePenaltySet( int penaltyIDs)
	{
		for(var i = 0; i < penaltyIDs; i++)
		{			
			penaltyList.Add( new PickUpCards() );
			if(i == 1 && penaltyList[0].PenaltyDescription == penaltyList[1].PenaltyDescription)
			{
				penaltyList.Clear();
				CreatePenaltySet(penaltyIDs);
			}
		}

		Debug.Log("!---PENALTIES---!");
		foreach(PenaltyBase penalty in penaltyList)
		{
			Debug.Log(penalty.PenaltyDescription);
		}
	}

	public int CheckCardValidity(PlayingCard proposedCard)
	{
		foreach(PenaltyBase penalty in penaltyList)
		{
			if(penalty.CheckPenalty(proposedCard) > 0 )
			{
				Debug.Log(penalty.PenaltyDescription);
				return penalty.PenaltyAmount;
			}
		}

		return 0;
	}


	// True means the card doesn't break 'the rules'.
	public bool CheckCardValidity(PlayingCard previousCard,PlayingCard proposedCard)
	{
		foreach(RuleBase rule in rulesList)
		{
			if(!rule.CheckRule(previousCard,proposedCard))
			{
				Debug.Log( rule.RuleDescription );
				return false;
			}
		}

		return true;
	}


	int[] Shuffle(int numberOfRules)
	{
		int[] order = new int[numberOfRules];
		for(var i = 0; i < numberOfRules; i++)
		{
			order[i] = i;
		}

		for(var i = 0; i < numberOfRules; i++)
		{
			int randomLoc = Random.Range(0,numberOfRules);
			int temp  = order[i];
			order[i] = order[randomLoc];
			order[randomLoc] = temp;
		}

		return new int[3]{order[0],order[1],order[2]};
	}

}
