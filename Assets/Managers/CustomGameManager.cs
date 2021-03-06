using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class CustomGameManager : MonoBehaviour
{
    float timestamp = 0;
    float cooldown = 60f;
    bool first = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GameOver();
    }


    public void FixedUpdate()
    {
        if (PlayerManager.gameStarted)
        {
            if (first)
            {
                timestamp = Time.time;
                first = false;
            }

            if (timestamp + cooldown < Time.time)
            {
                timestamp = Time.time;
                cooldown = Random.Range(80, 140);
                PlayerManager.Capricious();
            }
        }
    }

    public void GameOver()
    {
        Invoke("Restart", 1f);
        foreach (GameObject player in PlayerManager.PlayerObjects)
        {
            if (player != null)
            {
                Destroy(player);
            }
        }
        PlayerManager.Reset();
    }

    public void Restart()
    {
        LevelManager.TempleFloor = 1;
        SceneManager.LoadScene(1);
        Destroy(this.gameObject);
    }

    public void Victory()
    {
        foreach (GameObject player in PlayerManager.PlayerObjects)
        {
            if (player != null)
            {
                Destroy(player);
            }
        }
        Invoke("Win", 2f);
    }


    public void Win()
    {
        var time = Time.time - PlayerManager.time;
        var record = PlayerPrefs.GetFloat("score");

        if (time < 10f)
        {
            GuiScript.instance.Talk(new Message(aText: "YOU CHEATED, CONGRATULATIONS!"));
        }
        else if (record < 0.1f)
        {
            GuiScript.instance.Talk(new Message(aText: "YOU ARE THE FIRST TO WIN ON THIS COMPUTER!\nTIME: " + time + "."));
            PlayerPrefs.SetFloat("score", time);
        }
        else if (time >= record)
        {
            GuiScript.instance.Talk(new Message(aText: "YOU HAVE WON!\nTIME: " + time + ".\nRECORD: "));
        }
        else
        {
            GuiScript.instance.Talk(new Message(aText: "YOU HAVE WON AND SET A NEW RECORD!\nTIME: " + time + ".\nLAST RECORD: " + record + "."));
            PlayerPrefs.SetFloat("score", time);
        }

        PlayerManager.Reset();
        Invoke("Restart", 30f);
    }

    private void OnApplicationQuit()
    {
        for (int i = 0; i < 3; i++)
            GamePad.SetVibration((PlayerIndex)i, 0, 0);
    }
}