﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.Player)
        {
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
			SoundManager sm = SoundManager.instance;
			sm.PlayMusic(1);
			PlayerManager.gameStarted = true;
			PlayerManager.time = Time.time;
        }
    }
}
