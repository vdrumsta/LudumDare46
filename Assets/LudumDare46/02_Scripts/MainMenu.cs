using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private bool _displayingHelp;
    private GameObject _helpObject;

    public void DisplayHelp(GameObject helpImageObject)
    {
        _helpObject = helpImageObject;
        _helpObject.SetActive(true);
        StartCoroutine(DelayReadyToHide(1));
    }

    public void Update()
    {
        if (_displayingHelp && Input.anyKeyDown)
        {
            _helpObject.SetActive(false);
            _displayingHelp = false;
        }
    }

    IEnumerator DelayReadyToHide(float seconds)
    {
        yield return new WaitForSeconds(1);
        _displayingHelp = true;
    }
}
