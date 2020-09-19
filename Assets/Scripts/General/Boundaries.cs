using UnityEngine;

public class Boundaries : MonoBehaviour 
{
	public float cameraBoundary_x = 3.87f;
	public float cameraBoundary_y = 4.86f;

	public float playerBoundary_x = 14.83f;
	public float playerBoundary_y = 9.04f;

	public float sheepBoundary_x = 15.4f;
	public float sheepBoundary_top_y = 9.4f; // Boundary on the top of screen
	public float sheepBoundary_bot_y = 9.7f; // Boundary on the bottom


	void Start()
	{
		SetEdgeCollider();
	}

	void SetEdgeCollider()
	{
		float x = sheepBoundary_x;
		float top_y = sheepBoundary_top_y;
		float bot_y = sheepBoundary_bot_y;

		// Setting the edge collider points
		EdgeCollider2D collider = this.GetComponent<EdgeCollider2D>();
		Vector2[] colliderpointsTemp = collider.points;
		colliderpointsTemp[0] = new Vector2(x, top_y);
		colliderpointsTemp[1] = new Vector2(-x, top_y);
		colliderpointsTemp[2] = new Vector2(-x, -bot_y);
		colliderpointsTemp[3] = new Vector2(x, -bot_y);
		colliderpointsTemp[4] = new Vector2(x, top_y);

		collider.points = colliderpointsTemp;
	}
}
