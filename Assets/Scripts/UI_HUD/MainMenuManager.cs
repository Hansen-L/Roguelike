using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	public GameObject tutorialCanvas;
	public Animator transitionAnimator;

	private bool tutorialOpen = false;

	private void Update()
	{
		// If we are on the tutorial screen, load the game if we press any key.
		if (tutorialOpen)
			if (Input.anyKey)
				StartGame();
	}


	public void StartGame() // Play transition animation, wait until it is done, and then load next scene.
	{
		transitionAnimator.SetTrigger("transition");
		Invoke("LoadNextScene", 1f);
	}
	private void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void StartTutorial()
	{
		tutorialCanvas.SetActive(true);
		tutorialOpen = true;
	}

}
