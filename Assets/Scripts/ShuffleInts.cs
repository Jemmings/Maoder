using UnityEngine;
using System.Collections;

public class ShuffleInts
{
	public int[] Shuffle(int arraySize)
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

}
