using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPlayer
{
	bool TurnOver{ get; set; }
	bool HandEmpty{ get; }

	void AddCard(PlayingCard card,bool end);
	void StateChange();
	void UpdateState();
}
