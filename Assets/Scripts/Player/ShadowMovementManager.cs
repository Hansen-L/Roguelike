using UnityEngine;
using System;
using Utils;
using System.Collections;

public class ShadowMovementManager : MonoBehaviour
{
	// This class handles managing the movement of the shadow

	private Player shadowPlayerScript;
	private Player mainPlayerScript;
	private Rigidbody2D shadowPlayerRb;
	private Rigidbody2D mainPlayerRb;

	public void Start()
	{
		shadowPlayerScript = GameManager.GetShadowPlayer().GetComponent<Player>();
		mainPlayerScript = GameManager.GetMainPlayer().GetComponent<Player>();
		shadowPlayerRb = GameManager.GetShadowPlayer().GetComponent<Rigidbody2D>();
		mainPlayerRb = GameManager.GetMainPlayer().GetComponent<Rigidbody2D>();
	}

	public void Update()
	{
		StartCoroutine(ShadowMovement(mainPlayerRb.position));
	}

	private IEnumerator ShadowMovement(Vector2 mainPlayerPosition) // Shadow movement
	{
		yield return new WaitForSeconds(Player.shadowDelay);
		if (shadowPlayerScript.shadowCanMove)
		{
			Vector2 prevPosition = shadowPlayerRb.position;
			shadowPlayerRb.position = Vector2.Lerp(shadowPlayerRb.position, mainPlayerPosition, 0.05f);
			//_rb.position = Vector2.MoveTowards(_rb.position, mainPlayerPosition, maxSpeed * Time.deltaTime);
			shadowPlayerRb.velocity = (shadowPlayerRb.position - prevPosition) / Time.deltaTime;
		}
	}
}
