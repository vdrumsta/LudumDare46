using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class PlantGrow : MonoBehaviour
{
    public float maxGrowSize = 3f;
    public GameController gameController;
    private float roundTime = 180f;
    private Stopwatch stopwatch;
    private float startingGrowScale;
    private float maxGrowExtra;

    void Start()
    {
        startingGrowScale = transform.localScale.x;
        maxGrowExtra = maxGrowSize - transform.localScale.x;

        stopwatch = Stopwatch.StartNew();

        if (gameController)
        {
            roundTime = gameController.roundTime;
        }
    }

    void Update()
    {
        float growRatio = (float) stopwatch.Elapsed.TotalSeconds / roundTime;
        float sizeGrowExtra = Mathf.Lerp(0, maxGrowExtra, growRatio);
        float newGrowScale = startingGrowScale + sizeGrowExtra;
        Debug.Log("newGrowScale = " + newGrowScale + "; time = " + stopwatch.Elapsed.TotalSeconds);
        var newGrowScaleVector = new Vector3(newGrowScale, newGrowScale, newGrowScale);
        transform.localScale = newGrowScaleVector;
    }
}
