using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineToShadow : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();

    }

    private void Update()
    {
        _lineRenderer.SetPosition(0, GameManager.GetMainPlayer().transform.position);
        _lineRenderer.SetPosition(1, GameManager.GetShadowPlayer().transform.position);
    }

}
