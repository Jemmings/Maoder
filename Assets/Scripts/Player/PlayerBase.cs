﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class PlayerBase : IPlayer
{
	protected GameController controller;
	protected List<PlayingCard> hand = new List<PlayingCard>();
	protected List<PlayingCard> playedCards = new List<PlayingCard>();
	protected List<PlayingCard> deck = new List<PlayingCard>();
	protected bool turnOver = true;
	protected bool delay = false;
	protected bool endTurn = false;
	protected float timeOffset = 0;

	private GameObject faceDownCard;
	private Vector2 handPosition;
	private Vector2 handDirection;
	private Quaternion handRotation;

	public bool TurnOver
	{
		get
		{
			return turnOver;
		}
		set
		{
			turnOver = value;
		}
	}

	public PlayerBase(GameController controller, List<PlayingCard> deck,List<PlayingCard> playedCards,Vector2 handPosition,Vector2 handDirection,Quaternion handRotation)
	{
		this.controller = controller;
		this.deck = deck;
		this.playedCards = playedCards;
		this.handPosition = handPosition;
		this.handDirection = handDirection;
		this.handRotation = handRotation;

		faceDownCard = MonoBehaviour.Instantiate(Resources.Load("Face Down Card"),new Vector2(-10,-10),Quaternion.identity) as GameObject;
	}

	public void AddCard(int numberOfCards,bool end)
	{
		controller.StartCoroutine( controller.lerp.LerpMove2D(faceDownCard,controller.deckPosition,handPosition,Quaternion.Euler(Vector3.zero),handRotation,0.3f,false,LerpComponent.LerpEasing.linear,true,numberOfCards) );
		timeOffset = Time.time + (0.3f * numberOfCards);
		delay = true;
		endTurn = end;
	}

	public void AddCard(PlayingCard card, bool end)
	{
		hand.Add( card );
		controller.StartCoroutine( controller.lerp.LerpMove2D(faceDownCard,controller.deckPosition,handPosition,Quaternion.Euler(Vector3.zero),handRotation,0.3f,true) );
		timeOffset = Time.time + 0.3f;
		delay = true;
		endTurn = end;
	}

	public void RemoveCard(PlayingCard card)
	{
		hand.Remove( card );
		playedCards.Add( card );
		SortPlayedCardsOrder();
		controller.StartCoroutine( controller.lerp.LerpMove2D(card.CardGO,handPosition,controller.turnCardPosition,handRotation,Quaternion.Euler(Vector3.zero),0.3f,false) );
		timeOffset = Time.time + 0.3f;
		delay = true;
	}

	void SortHandCardOrder()
	{
		float cardDist = Mathf.Clamp(3f/hand.Count,0.2f,0.5f);

		for(var i = 0; i < hand.Count; i++)
		{
			hand[i].CardGO.transform.position = handPosition + ((handDirection * cardDist) * i);
			hand[i].CardGO.transform.rotation = handRotation;
			hand[i].CardGO.GetComponent<SpriteRenderer>().sortingOrder = hand.Count - i;
		}
	}

	void SortPlayedCardsOrder()
	{
		if(playedCards.Count > 2)
		{
			for(var i = 0; i < playedCards.Count-2; i++)
			{
				playedCards[i].CardGO.transform.position = new Vector2(-10,-10);
			}

			playedCards[playedCards.Count-2].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
		else if(playedCards.Count > 1)
		{
			playedCards[playedCards.Count-2].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}

		//Debug.Log("Played Cards Top: "+ playedCards[playedCards.Count-1].Value +" "+playedCards[playedCards.Count-1].Suit);
		playedCards[playedCards.Count-1].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 2;
	}

	public abstract void StateChange();

	public virtual void UpdateState()
	{
		if(timeOffset < Time.time && delay)
		{
			SortHandCardOrder();
			delay = false;
			if(endTurn)
			{
				turnOver = true;
			}
		}
	}

}
