using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Setup : MonoBehaviour
{
	public GameObject DeckSprites;
	public GameObject FaceDownCard;
	public GameObject TurnIndicator;
	public LayerMask PlayingCardLayer;

	// Components.
	private LerpComponent lerp;

	// World positions.
	private Vector2 offscreen = new Vector2(-10,-10);
	private Vector2 deckPosition = new Vector2(0.75f,0);
	private Vector2 turnCardPosition = new Vector2(-0.75f,0);
	private Vector2 leftStartingPoint = new Vector2(-4.5f,-1.5f);
	private Vector2 topStartingPoint = new Vector2(-1.5f,4);
	private Vector2 rightStartingPoint = new Vector2(4.5f,1.5f);
	private Vector2 bottomStartingPoint = new Vector2(1.5f,-3.5f);

	// Hands.
	private PlayingCard[] playingCards = new PlayingCard[54];
	private List<PlayingCard> playedCards = new List<PlayingCard>();
	private List<PlayingCard> deck = new List<PlayingCard>();
	private List<PlayingCard> leftHand = new List<PlayingCard>();
	private List<PlayingCard> topHand = new List<PlayingCard>();
	private List<PlayingCard> rightHand = new List<PlayingCard>();
	private List<PlayingCard> playerHand = new List<PlayingCard>();

	private BasicRuleset rules;
	private int[] shuffleOrder = new int[54];
	private int startAmount = 7;
	private int cardsInUse = 0;
	private Turn currentTurn = new Turn();
	private bool turnChange = false;
	private bool playerChosen = false;
	private bool dealingOutCards = false;
	private bool placingPlayedCards = false;
	private bool roundFinished = false;

	// The card lifting collider and position.
	private Transform colliderHitTransform;
	private Vector3 colliderStartPoint;

	private enum Turn
	{
		LeftPlayer,
		TopPlayer,
		RightPlayer,
		BottomPlayer
	}

	void Awake()
	{
		Debug.Log("Right Project");
		// GetComponents.
		lerp = GetComponent<LerpComponent>();

		// Create playing cards.
		string[] cardSuits = new string[4]{"Spades","Hearts","Clubs","Diamonds"};
		int suitNum = 0;
		int arrayPos = 0;
		for(var i = 1; i < 14; i++)
		{
			playingCards[arrayPos] = new PlayingCard(i,cardSuits[suitNum]);
			arrayPos++;

			if(i == 13 && arrayPos < 51)
			{
				i = 0;
				suitNum++;
			}
		}
		// Add the Jokers.
		playingCards[arrayPos] = new PlayingCard(1,"Joker");
		arrayPos++;
		playingCards[arrayPos] = new PlayingCard(2,"Joker");

		shuffleOrder = Shuffle(shuffleOrder.Length);
		FaceDownCard = Instantiate(FaceDownCard,offscreen,Quaternion.identity) as GameObject;
		TurnIndicator = Instantiate(TurnIndicator,new Vector3(0,-2,0),Quaternion.identity) as GameObject;
	}
	
	void Start()
	{
		GameObject cardHolder = new GameObject("Card Holder");
		foreach(PlayingCard card in playingCards)
		{
			card.CardGO.transform.name = card.Value.ToString() +" "+ card.Suit;
			card.CardGO.transform.parent = cardHolder.transform;
		}
	
		StartCoroutine( DealCards() );
		rules = new BasicRuleset();

		//lerp.LerpMove(this,Vector2.zero,Vector2.zero,Quaternion.identity,Quaternion.identity,1f);
	}

	void Update()
	{

		if(roundFinished)
		{
			// Show the rules screen.
			Debug.Log("Round Won!");
			return;
		}

		if(turnChange)
		{
			Debug.Log("Changing Turn");
			turnChange = false;
			ChangePlayers();
		}

		if(currentTurn == Turn.BottomPlayer && !playerChosen)
		{
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(mousePos,Vector2.zero,100f,PlayingCardLayer);

			if(hit.collider != null && hit.collider.transform != colliderHitTransform && hit.collider.tag == "Playing Card")
			{
				if(colliderHitTransform != null)
				{
					colliderHitTransform.position = colliderStartPoint;
				}

				colliderHitTransform = hit.collider.transform;
				colliderStartPoint = hit.collider.transform.position;
				hit.collider.transform.position = hit.collider.transform.position + new Vector3(0,0.5f,0);
			}
			else if(hit.collider == null)
			{
				if(colliderHitTransform != null)
				{
					colliderHitTransform.position = colliderStartPoint;
				}

				colliderHitTransform = null;
			}

			// If player chooses an action.
			if(Input.GetMouseButtonDown(0) && hit.collider != null)
			{
				if(hit.collider.tag == "Playing Card")
				{
					playerChosen = true;
					StartCoroutine( PlayerChoosesCard(hit.collider.gameObject) );
				}
				else if(hit.collider.tag == "Deck")
				{
					playerChosen = true;
					StartCoroutine( PlayerPicksUp() );
				}

				colliderHitTransform = null;
			}
		}

	}

	IEnumerator PlayerChoosesCard(GameObject chosenCard)
	{
		PlayingCard proposedCard = null;

		foreach(PlayingCard card in playerHand)
		{
			// Turn off colliders when turn is over.
			card.CardGO.GetComponent<BoxCollider2D>().enabled = false;

			if(chosenCard == card.CardGO)
			{
				proposedCard = card;
			}
		}

		if(!rules.CheckCardValidity( playedCards[playedCards.Count-1], proposedCard))
		{
			Debug.Log("2 CARD PENALTY");
			//2 card Penalty.
			dealingOutCards = true;
			StartCoroutine( DealLerp(2,true) );
		}
		else
		{
			Debug.Log("PLAY CARD NO PENALTY");
			// Add card to playedCard list.
			playerHand.Remove( proposedCard );
			playedCards.Add( proposedCard );
			PlacePlayedCard(proposedCard.CardGO.transform.position,false);
			CheckDeckCount();
			// Check if the player hand is now empty.
			roundFinished = (playerHand.Count == 0) ? true: false;
			// Check if the played card is a penalty card.
			if(rules.CheckCardValidity(proposedCard) > 0)
			{
				Debug.Log("IF");
				dealingOutCards = true;
				StartCoroutine( DealLerp(rules.CheckCardValidity(proposedCard)) );
				yield return new WaitUntil(() => !dealingOutCards);
				//turnChange = true;
			}
			else
			{
				Debug.Log("ELSE");
				//turnChange = true;
			}

			StartCoroutine( IndicatorLerp() );
			Debug.Log("FUNCTION END");
		}
	}

	IEnumerator PlayerPicksUp()
	{
		// Turn off colliders when turn is over.
		foreach(PlayingCard card in playerHand)
		{
			card.CardGO.GetComponent<BoxCollider2D>().enabled = false;
		}

		if(deck.Count > 0)
		{
			Debug.Log("Picking up Card");
			playerHand.Add( deck[0] );
			placingPlayedCards = true;
			StartCoroutine( CardLerp(FaceDownCard,deckPosition,bottomStartingPoint,FindCardRotation(bottomStartingPoint),0.5f,true) );
			yield return new WaitUntil(() => !placingPlayedCards);
			deck[0].InDeck = false;
			deck.RemoveAt( 0 );
			CheckDeckCount();
		}
		else
		{
			Debug.Log("Deck is empty!" + "/nPlayer knocks!");
			StartCoroutine( IndicatorLerp() );
		}
	}

	int[] Shuffle(int arraySize)
	{
		int[] newShuffleArray = new int[arraySize];
		for(var i = 0; i < arraySize; i++)
		{
			newShuffleArray[i] = i;
		}

		for(var i = 0; i < arraySize; i++)
		{
			int randomLoc = Random.Range(0,arraySize);
			int temp  = newShuffleArray[i];
			newShuffleArray[i] = newShuffleArray[randomLoc];
			newShuffleArray[randomLoc] = temp;
		}

		return newShuffleArray;
	}

	IEnumerator DealCards()
	{
		for(var i = 0; i < startAmount; i++)
		{
			// Left side cards.
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.position = leftStartingPoint;
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
			playingCards[shuffleOrder[cardsInUse]].InDeck = false;
			leftHand.Add( playingCards[shuffleOrder[cardsInUse]] );
			cardsInUse++;
			SortCardOrder(leftHand,leftStartingPoint);
			yield return new WaitForSeconds(0.1f);

			// Top side cards.
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.position = topStartingPoint;
			playingCards[shuffleOrder[cardsInUse]].InDeck = false;
			topHand.Add( playingCards[shuffleOrder[cardsInUse]] );
			cardsInUse++;
			SortCardOrder(topHand,topStartingPoint);
			yield return new WaitForSeconds(0.1f);

			// Right side cards.
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.position = rightStartingPoint;
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
			playingCards[shuffleOrder[cardsInUse]].InDeck = false;
			rightHand.Add( playingCards[shuffleOrder[cardsInUse]] );
			cardsInUse++;
			SortCardOrder(rightHand,rightStartingPoint);
			yield return new WaitForSeconds(0.1f);

			// Player side cards.
			playingCards[shuffleOrder[cardsInUse]].CardGO.transform.position = bottomStartingPoint;
			playingCards[shuffleOrder[cardsInUse]].InDeck = false;
			playerHand.Add( playingCards[shuffleOrder[cardsInUse]] );
			cardsInUse++;
			SortCardOrder(playerHand,bottomStartingPoint);
			yield return new WaitForSeconds(0.1f);
		}

		// Add the remaining cards to the deck.
		for(var i = 0; i < 54; i++)
		{
			if(playingCards[shuffleOrder[i]].InDeck)
			{
				deck.Add( playingCards[shuffleOrder[i]] );
			}
		}

		// Turn over the first card.
		playedCards.Add( deck[0] );
		deck[0].InDeck = false;
		deck.Remove( deck[0] );

		currentTurn = Turn.BottomPlayer;
		PlacePlayedCard(deckPosition,true);
	}

	void SortCardOrder(List<PlayingCard> currentHand, Vector3 startPos)
	{
		float cardDist = Mathf.Clamp(3f/currentHand.Count,0.2f,0.5f);
		Vector3 cardDirection = FindCardDirection(startPos,cardDist);

		for(var i = 0; i < currentHand.Count; i++)
		{
			currentHand[i].CardGO.transform.position = startPos + (cardDirection * i);
			currentHand[i].CardGO.transform.rotation = FindCardRotation((Vector2)startPos);
			currentHand[i].CardGO.GetComponent<SpriteRenderer>().sortingOrder = currentHand.Count - i;
		}
	}

	void PlacePlayedCard(Vector2 cardOrigin, bool endTurn)
	{
		if(playedCards.Count > 2)
		{
			for(var i = 0; i < playedCards.Count-2; i++)
			{
				playedCards[i].CardGO.transform.position = offscreen;
			}

			playedCards[playedCards.Count-2].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
		else if(playedCards.Count > 1)
		{
			playedCards[playedCards.Count-2].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}

		Debug.Log("Played Cards Top: "+ playedCards[playedCards.Count-1].Value +" "+playedCards[playedCards.Count-1].Suit);

		playedCards[playedCards.Count-1].CardGO.GetComponent<SpriteRenderer>().sortingOrder = 2;
		StartCoroutine( CardLerp( playedCards[playedCards.Count-1].CardGO,cardOrigin,turnCardPosition,Quaternion.Euler(Vector3.zero),0.5f,endTurn) );

	}

	Vector3 FindCardDirection(Vector2 cardPos, float dist)
	{
		if(cardPos.x > 4 || cardPos.x < -4)
		{
			return (cardPos.x > 4) ? new Vector3(0,-dist,0): new Vector3(0,dist,0);
		}
		else
		{
			return (cardPos.y > 3) ? new Vector3(dist,0,0): new Vector3(-dist,0,0);
		}
	}

	Quaternion FindCardRotation(Vector2 cardPos)
	{
		if(cardPos.x > 4 || cardPos.x < -4)
		{
			return Quaternion.Euler(0,0,90);
		}
		else
		{
			return Quaternion.Euler( Vector3.zero );
		}
	}

	//------Play-Mechanics---
	IEnumerator PlayerAction(List<PlayingCard> hand, Vector3 startPos)
	{
		int[] playOrder = Shuffle(hand.Count);

		for(var i = 0; i < playOrder.Length; i++)
		{
			if(!rules.CheckCardValidity( playedCards[playedCards.Count-1], hand[playOrder[i]]))
			{
				continue;
			}
			else if(hand[playOrder[i]].Suit != "Joker")
			{
				// Play the card.
				Debug.Log("Playing card");
				PlayingCard playCard = hand[playOrder[i]];
				hand.Remove( playCard );
				playedCards.Add( playCard );
				placingPlayedCards = true;
				PlacePlayedCard(playCard.CardGO.transform.position,false);
				CheckDeckCount();
				yield return new WaitUntil(() => !placingPlayedCards);
				// Check if the player hand is now empty.
				roundFinished = (hand.Count == 0) ? true: false;
				// Check if the played card is a penalty card.
				if(rules.CheckCardValidity(playCard) > 0)
				{
					dealingOutCards = true;
					StartCoroutine( DealLerp(rules.CheckCardValidity(playCard)) );
					yield return new WaitUntil(() => !dealingOutCards);
				}
				StartCoroutine( IndicatorLerp() );
				yield break;
			}
			else
			{
				continue;
			}
		}

		// Play the wild card before picking up.
		foreach(PlayingCard playCard in hand)
		{
			if(playCard.Suit == "Joker")
			{
				Debug.Log("Playing card");
				hand.Remove( playCard );
				playedCards.Add( playCard );
				PlacePlayedCard(playCard.CardGO.transform.position,true);
				CheckDeckCount();
				roundFinished = (hand.Count == 0) ? true: false;
				yield break;
			}
		}

		// None of the cards passed the rules so pickup.
		if(deck.Count > 0)
		{
			hand.Add( deck[0] );
			StartCoroutine( CardLerp(FaceDownCard,deckPosition,startPos,FindCardRotation(startPos),0.5f,true) );
			deck[0].CardGO.transform.rotation = hand[0].CardGO.transform.rotation;
			deck[0].InDeck = false;
			deck.RemoveAt( 0 );
			CheckDeckCount();

			yield break;
		}
		else
		{
			Debug.Log("AI Player knocking!");
			turnChange = true;
		}

		yield return null;
	}

	void CheckDeckCount()
	{
		if(deck.Count == 0 && playedCards.Count >= 2)
		{
			Debug.Log("---Swapping Deck Over---");

			DeckSprites.SetActive( true );
			foreach(PlayingCard card in playedCards)
			{
				deck.Add(card);
				card.CardGO.transform.position = offscreen;
				card.CardGO.GetComponent<SpriteRenderer>().sortingOrder = 0;
			}

			deck.Reverse();

			playedCards.Clear();
			playedCards.Add( deck[0] );


			deck[0].InDeck = false;
			deck.Remove( deck[0] );

			PlacePlayedCard(deckPosition,true);
		}
		else if(deck.Count == 0 && playedCards.Count <= 1)
		{
			DeckSprites.SetActive( false );
		}
		else
		{
			return;
		}
			
	}

	void ChangePlayers()
	{
		if(currentTurn == Turn.BottomPlayer)
		{
			currentTurn = Turn.LeftPlayer;
			StartCoroutine( PlayerAction(leftHand,leftStartingPoint) );
		}
		else if(currentTurn == Turn.LeftPlayer)
		{
			currentTurn = Turn.TopPlayer;
			StartCoroutine( PlayerAction(topHand,topStartingPoint) );
		}
		else if(currentTurn == Turn.TopPlayer)
		{
			currentTurn = Turn.RightPlayer;
			StartCoroutine( PlayerAction(rightHand,rightStartingPoint) );
		}
		else if(currentTurn == Turn.RightPlayer)
		{
			playerChosen = false;
			currentTurn = Turn.BottomPlayer;
			Debug.Log("Player's Turn");
			// Turn on colliders for card lifting.
			for(var i = 0; i < playerHand.Count; i++)
			{
				playerHand[i].CardGO.transform.position = new Vector3(playerHand[i].CardGO.transform.position.x,playerHand[i].CardGO.transform.position.y, (playerHand.Count - i) * -0.1f);
				playerHand[i].CardGO.GetComponent<BoxCollider2D>().enabled = true;
			}
		}

		Debug.Log("Changing Players: " + currentTurn.ToString());
	}

	IEnumerator CardLerp(GameObject go, Vector2 dest, Quaternion destRot, float lerpTime, bool endTurn)
	{
		Vector2 startingPos = go.transform.position;
		StartCoroutine( CardLerp(go,startingPos,dest,destRot,lerpTime,endTurn) );
		yield return null;
	}

	IEnumerator CardLerp(GameObject go, Vector2 dest, float lerpTime)
	{
		Vector2 startingPos = go.transform.position;
		Quaternion destRot = go.transform.rotation;
		StartCoroutine( CardLerp(go,startingPos,dest,destRot,lerpTime,false) );
		yield return null;
	}

	IEnumerator CardLerp(GameObject go, Vector2 startPos, Vector2 dest, Quaternion destRot, float lerpTime, bool endTurn)
	{
		float elapsedTime = 0;
		Quaternion startRot = go.transform.rotation;

		while (elapsedTime < lerpTime)
		{
			// !ncrement timer once per frame.
			elapsedTime += Time.deltaTime;
			if (elapsedTime > lerpTime)
			{
				elapsedTime = lerpTime;
			}

			// No easing. Always use this either on it's own or in combination with the easings below.
			float perc = elapsedTime / lerpTime;
			// perc = Mathf.Sin(perc * Mathf.PI * 0.5f);//Ease in
			// perc = 1f - Mathf.Cos(perc * Mathf.PI * 0.5f);//Ease out

			go.transform.position = Vector2.Lerp(startPos, dest, perc);
			go.transform.rotation = Quaternion.Lerp(startRot,destRot,perc);

			yield return null;
		}
			
		go.transform.position = dest;
		go.transform.rotation = destRot;
		FaceDownCard.transform.position = offscreen;

		// Sort hands workaround.
		SortCardOrder( leftHand,leftStartingPoint );
		SortCardOrder( topHand,topStartingPoint );
		SortCardOrder( rightHand,rightStartingPoint );
		SortCardOrder( playerHand,bottomStartingPoint );

		yield return new WaitForSeconds(1f);
		yield return new WaitUntil(() => !dealingOutCards);
		placingPlayedCards = false;

		if(endTurn)
		{
			Debug.Log("ENDING TURN");
			StartCoroutine( IndicatorLerp() );
		}
	}

	IEnumerator IndicatorLerp()
	{
		float elapsedTime = 0;
		float lerpTime = 1f;
		Vector3 startPos = TurnIndicator.transform.position;
		Vector3 destPos;
		if(currentTurn == Turn.RightPlayer)
		{
			destPos = new Vector3(0,-2,0);
		}
		else if(currentTurn == Turn.BottomPlayer)
		{
			destPos = new Vector3(-3,0,0);
		}
		else if(currentTurn == Turn.LeftPlayer)
		{
			destPos = new Vector3(0,2,0);
		}
		else
		{
			destPos = new Vector3(3,0,0);
		}

		Quaternion startRot = TurnIndicator.transform.rotation;
		Quaternion destRot = Quaternion.Euler( TurnIndicator.transform.rotation.eulerAngles + new Vector3(0,0,-90) );

		while (elapsedTime < lerpTime)
		{
			// !ncrement timer once per frame.
			elapsedTime += Time.deltaTime;
			if (elapsedTime > lerpTime)
			{
				elapsedTime = lerpTime;
			}

			// No easing. Always use this either on it's own or in combination with the easings below.
			float perc = elapsedTime / lerpTime;
			// perc = Mathf.Sin(perc * Mathf.PI * 0.5f);//Ease in
			// perc = 1f - Mathf.Cos(perc * Mathf.PI * 0.5f);//Ease out

			TurnIndicator.transform.position = Vector3.Slerp(startPos, destPos, perc);
			TurnIndicator.transform.rotation = Quaternion.Lerp(startRot,destRot,perc);

			yield return null;
		}

		TurnIndicator.transform.position = destPos;
		TurnIndicator.transform.rotation = destRot;

		turnChange = true;
	}

	IEnumerator DealLerp(int numberOfCards, bool toSelf = false)
	{
		if(deck.Count < numberOfCards && deck.Count > 0)
		{
			numberOfCards = deck.Count;
		}
		else if(deck.Count == 0)
		{
			dealingOutCards = false;
			yield break;
		}

		Vector3 dest;
		Quaternion destRot;
		List<PlayingCard> destHand;

		if(currentTurn == Turn.RightPlayer || toSelf)
		{
			dest = new Vector3(0,-2,0);
			destRot = Quaternion.Euler( new Vector3(0,0,0) );
			destHand = playerHand;
		}
		else if(currentTurn == Turn.BottomPlayer)
		{
			dest = new Vector3(-3,0,0);
			destRot = Quaternion.Euler( new Vector3(0,0,90) );
			destHand = leftHand;
		}
		else if(currentTurn == Turn.LeftPlayer)
		{
			dest = new Vector3(0,2,0);
			destRot = Quaternion.Euler( new Vector3(0,0,0) );
			destHand = topHand;
		}
		else
		{
			dest = new Vector3(3,0,0);
			destRot = Quaternion.Euler( new Vector3(0,0,90) );
			destHand = rightHand;
		}

		// Add/remove the cards.
		for(var i = 0; i < numberOfCards; i++)
		{
			destHand.Add( deck[0] );
			deck.RemoveAt(0);
		}

		for(var i = 0; i < numberOfCards; i++)
		{
			CheckDeckCount();
			Vector3 startPos = deckPosition;
			float lerpTime = 0.5f;
			float elapsedTime = 0;
			Quaternion startRot = Quaternion.Euler( Vector3.zero );

			while (elapsedTime < lerpTime)
			{
				// Increment timer once per frame.
				elapsedTime += Time.deltaTime;
				if (elapsedTime > lerpTime)
				{
					elapsedTime = lerpTime;
				}
				// No easing. Always use this either on it's own or in combination with the easings below.
				float perc = elapsedTime / lerpTime;
				// perc = Mathf.Sin(perc * Mathf.PI * 0.5f);//Ease in
				// perc = 1f - Mathf.Cos(perc * Mathf.PI * 0.5f);//Ease out

				FaceDownCard.transform.position = Vector3.Lerp(startPos, dest, perc);
				FaceDownCard.transform.rotation = Quaternion.Lerp(startRot,destRot,perc);

				yield return null;
			}

			FaceDownCard.transform.position = dest;
			FaceDownCard.transform.rotation = destRot;

			// Sort hands workaround.
			SortCardOrder( leftHand,leftStartingPoint );
			SortCardOrder( topHand,topStartingPoint );
			SortCardOrder( rightHand,rightStartingPoint );
			SortCardOrder( playerHand,bottomStartingPoint );

			FaceDownCard.transform.position = offscreen;
		}

		dealingOutCards = false;

		if(toSelf)
		{
			turnChange = true;
		}
	}
}

