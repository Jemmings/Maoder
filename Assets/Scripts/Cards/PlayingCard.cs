using UnityEngine;
using System.Collections;

public class PlayingCard
{
	private int value;
	private string suit;
	private bool inDeck;
	private bool previousCard;
	private GameObject cardGO;

	public PlayingCard(int value, string suit)
	{
		this.value = value;
		this.suit = suit;
		this.inDeck = true;
		previousCard = false;

		cardGO = MonoBehaviour.Instantiate(Resources.Load(suit +"/"+ value.ToString()),new Vector2(-10,-10),Quaternion.identity) as GameObject;
	}


	public int Value
	{
		get{ return value; }
	}
	public string Suit
	{
		get{ return suit; }
	}
	public GameObject CardGO
	{
		get{ return cardGO; }
	}
	public bool InDeck
	{
		get; set;
	}

	public bool PreviousCard
	{
		get; set;
	}

}
