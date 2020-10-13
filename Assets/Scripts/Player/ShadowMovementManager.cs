using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMovementManager : MonoBehaviour
{
	// This class handles managing the movement of the shadow
	// TODO: Should this be a singleton?
	public static ShadowMovementManager Instance;

	private Player shadowPlayerScript;
	private Player mainPlayerScript;
	private Rigidbody2D shadowPlayerRb;
	private Rigidbody2D mainPlayerRb;

	private Queue<Vector2> mainPlayerPositionQueue = new Queue<Vector2>();

	private void Awake()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(gameObject); }

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		shadowPlayerScript = GameManager.GetShadowPlayer().GetComponent<Player>();
		mainPlayerScript = GameManager.GetMainPlayer().GetComponent<Player>();
		shadowPlayerRb = GameManager.GetShadowPlayerRb();
		mainPlayerRb = GameManager.GetMainPlayerRb();
	}

	private void FixedUpdate()
	{
		mainPlayerPositionQueue.Enqueue(mainPlayerRb.position);
		StartCoroutine(ShadowMovement());
	}

	private IEnumerator ShadowMovement() // Shadow movement
	{
		//yield return new WaitForSeconds(Player.shadowDelay);
		for (int i=0; i < Player.ShadowDelay/Time.fixedDeltaTime; i++)
			yield return new WaitForFixedUpdate();

		Vector2 mainPlayerPosition = mainPlayerPositionQueue.Dequeue();
		if (shadowPlayerScript.shadowCanMove)
		{
			Vector2 prevPosition = shadowPlayerRb.position;
			Vector2 deltaPosition = mainPlayerPosition - shadowPlayerRb.position;
			Vector2 newPosition = shadowPlayerRb.position + Vector2.ClampMagnitude(deltaPosition, Player.MaxSpeed * Player.ShadowSpeedRatio * Time.fixedDeltaTime);
			shadowPlayerRb.velocity = (newPosition - prevPosition) / Time.fixedDeltaTime;
		}
		else
			shadowPlayerRb.velocity = new Vector2(0f, 0f);
	}

	public static void ResetPositionQueue()
	// Clears the position queue, then adds an equivalent amount of shadow's current position in to match the previous queue size.
	{
		int queueSize = Instance.mainPlayerPositionQueue.Count;
		Instance.mainPlayerPositionQueue.Clear();

		for (int i = 0; i < queueSize; i++)
		{
			Instance.mainPlayerPositionQueue.Enqueue(Instance.shadowPlayerRb.position);
		}
	}
}
