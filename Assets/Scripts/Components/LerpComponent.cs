using UnityEngine;
using System.Collections;

public class LerpComponent : MonoBehaviour
{
	// Overload method: no startPos, no rotation, no spehericalLerp, no easing.
	public IEnumerator LerpMove2D(GameObject go, Vector2 dest, float lerpTime, bool offscreen )
	{
		StartCoroutine( LerpMove2D(go, go.transform.position, dest, go.transform.rotation, go.transform.rotation, lerpTime, false, LerpEasing.linear, offscreen) );
		yield return null;
	}

	// Overload method: no rotation.
	public IEnumerator LerpMove2D(GameObject go, Vector2 startPos, Vector2 dest, bool sphericalLerp, float lerpTime, LerpEasing easingType, bool offscreen)
	{
		StartCoroutine( LerpMove2D(go, startPos, dest, go.transform.rotation, go.transform.rotation, lerpTime, sphericalLerp, easingType, offscreen) );
		yield return null;
	}

	// Overload method: no rotation, no easing.
	public IEnumerator LerpMove2D(GameObject go, Vector2 startPos, Vector2 dest, bool sphericalLerp, float lerpTime, bool offscreen)
	{
		StartCoroutine( LerpMove2D(go, startPos, dest, go.transform.rotation, go.transform.rotation, lerpTime, sphericalLerp, LerpEasing.linear, offscreen) );
		yield return null;
	}

	// Overload method: no easing, no spherialLerp.
	public IEnumerator LerpMove2D(GameObject go, Vector2 startPos, Vector2 dest, Quaternion startRot, Quaternion destRot, float lerpTime, bool offscreen)
	{
		StartCoroutine( LerpMove2D(go, startPos, dest, startRot, destRot, lerpTime, false, LerpEasing.linear, offscreen) );
		yield return null;
	}

	// Main Method, not overloaded.
	public IEnumerator LerpMove2D(GameObject go, Vector2 startPos, Vector2 dest, Quaternion startRot, Quaternion destRot, float lerpTime, bool sphericalLerp, LerpEasing easingType, bool offscreen, int numberOfCards = 1)
	{
		for(var i = 0; i < numberOfCards; i++)
		{
			float elapsedTime = 0;

			while (elapsedTime < lerpTime)
			{
				elapsedTime += Time.deltaTime;

				if (elapsedTime > lerpTime)
				{
					elapsedTime = lerpTime;
				}

				// Basic linear easing.
				float perc = elapsedTime / lerpTime;

				if(easingType == LerpEasing.easeIn)
				{
					// Ease in.
					perc = Mathf.Sin(perc * Mathf.PI * 0.5f);
				}
				else if(easingType == LerpEasing.easeOut)
				{
					// Ease out.
					perc = 1f - Mathf.Cos(perc * Mathf.PI * 0.5f);
				}

				if(sphericalLerp)
				{
					go.transform.position = Vector3.Slerp((Vector3)startPos, (Vector3)dest, perc);
				}
				else
				{
					go.transform.position = Vector2.Lerp(startPos, dest, perc);
				}

				go.transform.rotation = Quaternion.Lerp(startRot, destRot, perc);

				yield return null;
			}

			// Make sure the GameObject gets to it's destinated location and rotation.
			go.transform.position = dest;
			go.transform.rotation = destRot;
		}

		if(offscreen)
		{
			go.transform.position = new Vector2(-10,-10);
		}
	}

	public enum LerpEasing
	{
		linear,
		easeIn,
		easeOut
	}
}