using UnityEngine;
using System.Collections;
using System;

public abstract class RuleBase
{
	protected string ruleDescription;
	protected int cardValue;
	protected string cardSuit;

	public string RuleDescription
	{
		get
		{
			return ruleDescription;
		}
	}

	public int CardValue
	{
		get
		{
			return cardValue;
		}
	}

	public string CardSuit
	{
		get
		{
			return cardSuit;
		}
	}

	public abstract void RandomiseRuleValues();
	public Func<PlayingCard,PlayingCard,bool> CheckRule;
}
