using UnityEngine;
using System.Collections;

public class OrbitTest : MonoBehaviour
{
	public GameObject planet;
	public GameObject Sun;

	private float radius = 1;

	void FixedUpdate()
	{
		Sun.transform.position = Sun.transform.position + (Vector3.right * Time.deltaTime);


		//float x = centerX + radius * Mathf.Cos(angle);
		//float x = centerY + radius * Mathf.Sin(angle);

		float x = Sun.transform.position.x + radius * Mathf.Cos(Time.time);
		float y = Sun.transform.position.y + radius * Mathf.Sin(Time.time);

		planet.transform.position = new Vector2(x,y);
	}

	IEnumerator Rotate()
	{
		float r = 10.0f;
		for (var theta=0.0f; theta < 2 * Mathf.PI; theta += 0.01f)
		{
			float x = r * Mathf.Cos(theta),
			y = r * Mathf.Sin(theta);
			planet.transform.position = new Vector2(x, y);
			yield return null;
		}
	}


}