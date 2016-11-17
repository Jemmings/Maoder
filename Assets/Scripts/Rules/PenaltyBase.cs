using UnityEngine;
using System.Collections;
using System;

public abstract class PenaltyBase
{
	protected string penaltyDescription;
	protected int penaltyAmount;
	protected int cardValue;
	protected string cardSuit;

	public string PenaltyDescription
	{
		get
		{
			return penaltyDescription;
		}
	}

	public int PenaltyAmount
	{
		get
		{
			return penaltyAmount;
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
	public Func<PlayingCard,int> CheckPenalty;
}
