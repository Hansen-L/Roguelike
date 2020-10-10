using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// This singleton class keeps track of some scene-wide objects 

	private static GameManager Instance;

	public GameObject mainPlayer;
	public GameObject shadowPlayer;
	public Rigidbody2D mainPlayerRb;
	public Rigidbody2D shadowPlayerRb;

	void Awake()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(gameObject); }

		DontDestroyOnLoad(gameObject);
	}

	public static GameObject GetMainPlayer()
	{
		return Instance.mainPlayer;
	}

	public static GameObject GetShadowPlayer()
	{
		return Instance.shadowPlayer;
	}

	public static Rigidbody2D GetMainPlayerRb()
	{
		return Instance.mainPlayerRb;
	}

	public static Rigidbody2D GetShadowPlayerRb()
	{
		return Instance.shadowPlayerRb;
	}
}
