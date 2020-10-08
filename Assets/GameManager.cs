using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	// This singleton class keeps track of some scene-wide objects 

	public static GameManager Instance;

	public GameObject mainPlayer;
	public GameObject shadowPlayer;

	void Awake()
	{
		if (Instance == null) { Instance = this; }
		else { Destroy(gameObject); }

		DontDestroyOnLoad(gameObject);
	}
}
