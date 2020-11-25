using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteFlashManager : MonoBehaviour
{
    public static WhiteFlashManager Instance;
    public Material whiteMaterial;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

        DontDestroyOnLoad(gameObject);
    }

    // Dummy method to start a coroutine to flash the gameObject white for a set amount of time.
    public static void FlashWhite(GameObject targetObject, float flashDuration = 0.1f)
    {
        Instance.StartCoroutine(Instance.FlashWhiteCoroutine(targetObject, flashDuration));
    }
    // Associated coroutine
    private IEnumerator FlashWhiteCoroutine(GameObject targetObject, float flashDuration)
    {
        // Store original material so we can switch back
        SpriteRenderer renderer = targetObject.GetComponent<SpriteRenderer>();
        if (renderer)
        {
            if (!renderer.material.name.Contains(whiteMaterial.name)) // Checks if we are already using the whiteMaterial
            {
                Material originalMaterial = renderer.material;

                // Change the material to whiteMaterial, wait, and then change back
                renderer.material = whiteMaterial;
                yield return new WaitForSeconds(flashDuration);

                // Check if enemy did not die, reset the material.
                if (targetObject)
                    renderer.material = originalMaterial;

                //// If the boss changed color during the flash.
                //if (originalMaterial.color == renderer.material.color)
            }
        }
        yield return null;
    }
}
