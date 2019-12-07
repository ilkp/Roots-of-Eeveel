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

    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;

    public Axis vertical;
    public Axis horizontal;

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

    private void Start()
    {
        vertical = new Axis(forward, backward);
        horizontal = new Axis(right, left);
    }
}