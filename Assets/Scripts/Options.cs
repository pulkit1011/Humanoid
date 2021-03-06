﻿using CrazyMinnow.SALSA;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [SerializeField] GameObject options;
    [SerializeField] Salsa salsa;
    [SerializeField] AudioSource normalAudioSource;
    [SerializeField] TMP_InputField delayText;

    FileBrowserForAudioFile fileBrowser;
    float delayTime = 0.2f;

    private void Start()
    {
        fileBrowser = GetComponent<FileBrowserForAudioFile>();
    }
    public void OnOptionClick()
    {
        options.SetActive(!options.activeSelf);
    }
    public void OnPlayClick()
    {
        salsa.audioSrc.Play();
        StartCoroutine(PlayDelay());
    }
    public void OnContinuousPlayClick()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        FindObjectOfType<PlayAudioContinuously>().PlayContinuously(delayTime);
    }
    public void OnPauseClick()
    {
        normalAudioSource.Pause();
        salsa.audioSrc.Pause();
    }
    public void OnContinuousStopClick()
    {
        Time.timeScale = 0;
        normalAudioSource.Pause();
        salsa.audioSrc.Pause();
    }
    public void OnStopClick()
    {
        normalAudioSource.Stop();
        salsa.audioSrc.Stop();
    }
    public void OnUploadClick()
    {
        fileBrowser.LoadFile();
    }
    public void OnExitClick()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    public void OnDelaySubmitButtonClick()
    {
        float seconds;
        bool isNumeric = float.TryParse(delayText.text, out seconds);
       
        if(isNumeric)
        {
            delayTime = seconds;
            OnStopClick();
            OnPlayClick();
        }
    }

    public void OnExitButtonClick()
    {
        Application.Quit();
    }

   IEnumerator PlayDelay()
    {

        yield return new WaitForSeconds(delayTime);
        
        if(salsa.audioSrc.isPlaying)
        {
            normalAudioSource.Play();
        }

    }
  
}
