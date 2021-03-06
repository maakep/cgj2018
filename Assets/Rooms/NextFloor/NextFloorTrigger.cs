﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextFloorTrigger : MonoBehaviour
{
    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == Tags.Player)
        {
            if (!activated)
            {
                activated = true;
                LevelManager.TempleFloor++;
                if (LevelManager.TempleFloor == 4)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    return;
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
