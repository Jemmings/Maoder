using UnityEngine;
using System.Collections;

public interface IRuleset
{
	void CreateRuleset( int[] ruleIDs );
	bool CheckCardValidity(PlayingCard previousCard,PlayingCard proposedCard);
}
