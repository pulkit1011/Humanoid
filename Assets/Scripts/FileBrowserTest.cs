﻿using UnityEngine;
using System.Collections;
using SimpleFileBrowser;
using UnityEngine.Networking;

public class FileBrowserTest : MonoBehaviour
{
	[SerializeField] AudioSource salsaAudioSource;
	[SerializeField] AudioSource normalAudioSource;

	// Warning: paths returned by FileBrowser dialogs do not contain a trailing '\' character
	// Warning: FileBrowser can only show 1 dialog at a time

	void Start()
	{
		
		// Set filters (optional)
		// It is sufficient to set the filters just once (instead of each time before showing the file browser dialog), 
		// if all the dialogs will be using the same filters
		//FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png",".mp3"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"), new FileBrowser.Filter("Audio",".mp3",".wav"));
		FileBrowser.SetFilters(true,new FileBrowser.Filter("Audio", ".mp3", ".wav"));

		// Set default filter that is selected when the dialog is shown (optional)
		// Returns true if the default filter is set successfully
		// In this case, set Images filter as the default filter
		FileBrowser.SetDefaultFilter("Audio");

		// Set excluded file extensions (optional) (by default, .lnk and .tmp extensions are excluded)
		// Note that when you use this function, .lnk and .tmp extensions will no longer be
		// excluded unless you explicitly add them as parameters to the function
		FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");

		// Add a new quick link to the browser (optional) (returns true if quick link is added successfully)
		// It is sufficient to add a quick link just once
		// Name: Users
		// Path: C:\Users
		// Icon: default (folder icon)
		FileBrowser.AddQuickLink("Users", "C:\\Users", null);

		// Show a save file dialog 
		// onSuccess event: not registered (which means this dialog is pretty useless)
		// onCancel event: not registered
		// Save file/folder: file, Allow multiple selection: false
		// Initial path: "C:\", Title: "Save As", submit button text: "Save"
		// FileBrowser.ShowSaveDialog( null, null, false, false, "C:\\", "Save As", "Save" );

		// Show a select folder dialog 
		// onSuccess event: print the selected folder's path
		// onCancel event: print "Canceled"
		// Load file/folder: folder, Allow multiple selection: false
		// Initial path: default (Documents), Title: "Select Folder", submit button text: "Select"
		// FileBrowser.ShowLoadDialog( ( paths ) => { Debug.Log( "Selected: " + paths[0] ); },
		//                            () => { Debug.Log( "Canceled" ); },
		//                            true, false, null, "Select Folder", "Select" );

		// Coroutine example
		// StartCoroutine(ShowLoadDialogCoroutine());
	}

	public void LoadFile()
	{
		StartCoroutine(ShowLoadDialogCoroutine());
	}
	IEnumerator ShowLoadDialogCoroutine()
	{
		// Show a load file dialog and wait for a response from user
		// Load file/folder: file, Allow multiple selection: true
		// Initial path: default (Documents), Title: "Load File", submit button text: "Load"
		yield return FileBrowser.WaitForLoadDialog(false, false, null, "Load File", "Load");

		// Dialog is closed
		// Print whether the user has selected some files/folders or cancelled the operation (FileBrowser.Success)
		Debug.Log(FileBrowser.Success);

		if (FileBrowser.Success)
		{
			// Print paths of the selected files (FileBrowser.Result) (null, if FileBrowser.Success is false)
			for (int i = 0; i < FileBrowser.Result.Length; i++)
				Debug.Log(FileBrowser.Result[i]);

			yield return StartCoroutine(GetAudioClip(FileBrowser.Result[0]));
			// Read the bytes of the first file via FileBrowserHelpers
			// Contrary to File.ReadAllBytes, this function works on Android 10+, as well
			
			//byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result[0]);
		}
	}


	IEnumerator GetAudioClip(string directory)
	{
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + directory, AudioType.WAV))
		{
			yield return www.SendWebRequest();
			while (!www.isDone)
			{
				yield return new WaitForEndOfFrame();
			}

			if (www.isNetworkError)
			{
				Debug.Log(www.error);
			}
			else
			{
				AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
				//AudioClip audioClip = Resources.Load<AudioClip>(FileBrowser.Result[0]);
				normalAudioSource.clip = myClip;
				salsaAudioSource.clip = myClip;
				//transform.GetComponent<AudioSource>().clip = myClip;
				//transform.GetComponent<AudioSource>().Play();
			}
		}
	}
}