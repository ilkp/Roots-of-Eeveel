using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script just to hold all of the keybinds

public class Keybindings : MonoBehaviour
{
    public static Keybindings Instance { get; private set; }
    public KeyCode interaction;
    public KeyCode altInteraction;
    public KeyCode secondaryInteraction;
    public KeyCode altSecondaryInteraction;
    public KeyCode crouch;
    public KeyCode altCrouch;
    public KeyCode flashLight;

    private void Awake()
    {
        Time.timeScale = 1;
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
}
