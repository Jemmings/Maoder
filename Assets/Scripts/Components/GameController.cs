using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
	public GameObject FaceDownCard;
	public GameObject TurnIndicator;
	public GameObject DeckSprites;
	// Components.
	[HideInInspector]public LerpComponent lerp;
	public BasicRuleset rules;

	// Cards.
	private PlayingCard[] playingCards = new PlayingCard[54];
	private List<PlayingCard> playedCards = new List<PlayingCard>();
	private List<PlayingCard> deck = new List<PlayingCard>();

	// Players.
	IPlayer leftAIPlayer;
	IPlayer topAIPlayer;
	IPlayer rightAIPlayer;
	IPlayer realPlayer;
	IPlayer currentPlayer;
	ETurnState turnState = new ETurnState();

	// Locations.
	private Vector2 offscreen = new Vector2(-10,-10);
	[HideInInspector]public Vector2 deckPosition = new Vector2(0.75f,0);
	[HideInInspector]public Vector2 turnCardPosition = new Vector2(-0.75f,0);
	private Vector2 leftStartingPoint = new Vector2(-4.5f,-1.5f);
	private Vector2 topStartingPoint = new Vector2(-1.5f,4);
	private Vector2 rightStartingPoint = new Vector2(4.5f,1.5f);
	private Vector2 bottomStartingPoint = new Vector2(1.5f,-3.5f);

	// Checks.
	private bool gameStarted = false;
	private bool movingIndicator = false;
	private bool checkingDeck = false;

	void Awake()
	{
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

		// Create the one-off GameObjects.
		FaceDownCard = Instantiate(FaceDownCard,offscreen,Quaternion.identity) as GameObject;
		TurnIndicator = Instantiate(TurnIndicator) as GameObject;
		TurnIndicator.SetActive( false );

		// Create the players.
		leftAIPlayer = new AiPlayer(this,deck,playedCards,leftStartingPoint,Vector2.up,Quaternion.Euler(new Vector3(0,0,90)));
		topAIPlayer = new AiPlayer(this,deck,playedCards,topStartingPoint,Vector2.right,Quaternion.Euler(new Vector3(0,0,0)));
		rightAIPlayer = new AiPlayer(this,deck,playedCards,rightStartingPoint,-Vector2.up,Quaternion.Euler(new Vector3(0,0,90)));
		realPlayer = new RealPlayer(this,deck,playedCards,bottomStartingPoint,-Vector2.right,Quaternion.Euler(new Vector3(0,0,0)));

	}
		
	void Start() 
	{
		GameObject cardHolder = new GameObject("Card Holder");
		foreach(PlayingCard card in playingCards)
		{
			card.CardGO.transform.parent = cardHolder.transform;
			card.CardGO.transform.name = string.Format("{0} of {1}",card.Value,card.Suit);
		}

		rules = new BasicRuleset();
		ShuffleCards();
	}
	

	void Update()
	{
		currentPlayer.UpdateState();

		if(gameStarted && currentPlayer.TurnOver && !movingIndicator)
		{
			if((int)turnState == 3)
			{
				turnState = (ETurnState)0;
			}
			else
			{
				int nextTurn = (int)turnState + 1;
				turnState = (ETurnState)nextTurn;
			}
			movingIndicator = true;
			StartCoroutine( ChangePlayers(1) );
		}
	}


	void ShuffleCards()
	{
		// Remove all cards from present deck.
		deck.Clear();

		ShuffleInts shuffle = new ShuffleInts();
		int[] shuffleOrder = shuffle.Shuffle(54);

		for(var i = 0; i < shuffleOrder.Length; i++)
		{
			deck.Add(playingCards[shuffleOrder[i]]);
		}

		StartCoroutine( DealCards() );
	}

	IEnumerator DealCards()
	{
		for(var i = 0; i < 7; i++)
		{
			for(var j = 0; j < 4; j++)
			{
				turnState = (ETurnState)j;
				StartCoroutine( ChangePlayers(-1) );
				yield return new WaitUntil(() => !currentPlayer.TurnOver);
				currentPlayer.AddCard(deck[0],true);
				deck.RemoveAt(0);
				yield return new WaitUntil(() => currentPlayer.TurnOver);
			}
		}

		//Turn the top card over
		playedCards.Add( deck[0] );
		StartCoroutine( lerp.LerpMove2D(deck[0].CardGO,deckPosition,turnCardPosition,false,0.5f,false) );
		deck.RemoveAt(0);
		yield return new WaitForSeconds(1f);
		StartGame();
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
			StartCoroutine( lerp.LerpMove2D(deck[0].CardGO,deckPosition,turnCardPosition,false,0.5f,false) );
			deck.RemoveAt(0);
		}
		else if(deck.Count == 0 && playedCards.Count <= 1)
		{
			DeckSprites.SetActive( false );
		}

		checkingDeck = false;

	}

	void StartGame()
	{
		TurnIndicator.SetActive( true );
		gameStarted = true;
		currentPlayer.TurnOver = false;
		currentPlayer.StateChange();
	}
		
	IEnumerator ChangePlayers(float lerpTime)
	{
		Vector3 startPos = Vector3.zero;
		Vector3 destPos = Vector3.zero;
		Quaternion startRot = TurnIndicator.transform.rotation;
		Quaternion destRot = Quaternion.Euler( TurnIndicator.transform.rotation.eulerAngles + new Vector3(0,0,-90) );

		if(turnState == ETurnState.LeftPlayer)
		{
			currentPlayer = leftAIPlayer;
			startPos = TurnIndicator.transform.position;
			destPos = new Vector3(-3,0,0);
		}
		else if(turnState == ETurnState.TopPlayer)
		{
			currentPlayer = topAIPlayer;
			startPos = TurnIndicator.transform.position;
			destPos = new Vector3(0,2,0);
		}
		else if(turnState == ETurnState.RightPlayer)
		{
			currentPlayer = rightAIPlayer;
			startPos = TurnIndicator.transform.position;
			destPos = new Vector3(3,0,0);
		}
		else if(turnState == ETurnState.RealPlayer)
		{
			currentPlayer = realPlayer;
			startPos = TurnIndicator.transform.position;
			destPos = new Vector3(0,-2,0);
		}

		float elapsedTime = 0;
		while (elapsedTime < lerpTime)
		{
			elapsedTime += Time.deltaTime;

			if(elapsedTime > lerpTime)
			{
				elapsedTime = lerpTime;
				break;
			}

			float perc = elapsedTime / lerpTime;
			TurnIndicator.transform.position = Vector3.Slerp(startPos, destPos, perc);
			TurnIndicator.transform.rotation = Quaternion.Lerp(startRot, destRot, perc);
			yield return null;
		}
		TurnIndicator.transform.position = destPos;
		TurnIndicator.transform.rotation = destRot;


		currentPlayer.TurnOver = false;
		if(gameStarted)
		{
			// Check deck condition before another turn begins.
			checkingDeck = true;
			CheckDeckCount();
			yield return new WaitUntil(() => !checkingDeck);

			currentPlayer.StateChange();
		}
		movingIndicator = false;
	}

	private enum ETurnState
	{
		LeftPlayer,
		TopPlayer,
		RightPlayer,
		RealPlayer
	}
}
