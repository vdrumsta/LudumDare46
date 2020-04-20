using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;

public class GameController : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string PlayerStateEvent = "";
    FMOD.Studio.EventInstance playerState;

    public GameObject WinScreen;
    public Image radialRoundImage;
    public float roundTime;

    private Stopwatch _roundTimer;
    public bool _isPaused;
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            if (value)
            {
                _roundTimer.Stop();
            }
            else
            {
                _roundTimer.Start();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerState = FMODUnity.RuntimeManager.CreateInstance(PlayerStateEvent);
        playerState.start();

        _roundTimer = new Stopwatch();
        _roundTimer.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsPaused)
        {
            UpdateRoundTime();
        }
    }

    private void UpdateRoundTime()
    {
        float timeleft = roundTime - (float)_roundTimer.Elapsed.TotalSeconds;
        timeleft = timeleft < 0 ? 0 : timeleft;

        var radialImageRemainingFill = timeleft / roundTime;
        radialRoundImage.fillAmount = radialImageRemainingFill;

        if (timeleft <= 0)
        {
            IsPaused = true;
            _roundTimer.Stop();

            if (WinScreen)
            {
                WinScreen.SetActive(true);
            }
        }
    }
}
