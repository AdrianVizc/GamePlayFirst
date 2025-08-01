using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class ComboNode
{
    public Dictionary<KeyCode, ComboNode> children = new Dictionary<KeyCode, ComboNode>();
    public string animationTrigger = null; 
    public float points = 0;
}

public class Tricks : MonoBehaviour
{
    [Header("Trick Animation Settings")]
    private float percentageBuffer = 0.35f;

    [HideInInspector] public float currentTrickScore;
    private Animator animator;

    private ComboNode root = new ComboNode();
    private ComboNode currentNode;
    [HideInInspector] public float comboTimer;
    private float comboMaxTime = 5f;
    private KeyCode? lastKeyPressed = null;

    private PlayerGrind rail;
    private PlayerWall wall;
    private ScoreCombo scoreCombo;
    private bool onRail;
    private bool isWallRunning;
    private Dictionary<KeyCode, String> keyDict;
    [SerializeField] public float FrontFlip = 200f;
    [SerializeField] public float BackFlip = 200f;
    [SerializeField] public float BasicTrick1 = 200f;
    [SerializeField] public float BasicTrick2 = 200f;
    [SerializeField] public float SpecialTrick1 = 500f;
    [SerializeField] public float SpecialTrick2 = 500f;
    [SerializeField] public float SpecialTrick3 = 500f;
    [SerializeField] public float SpecialTrick4 = 500f;
    private bool leftTriggerPressed = false;
    private bool rightTriggerPressed = false;
    private bool firstPress = false;
    [HideInInspector] private string TrickName = "";

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rail = GetComponent<PlayerGrind>();
        wall = GetComponent<PlayerWall>();
        scoreCombo = GetComponent<ScoreCombo>();
        // Dictionary for Keycodes to Tricks
        keyDict = new Dictionary<KeyCode, String>();
        keyDict[KeyCode.UpArrow] = "FrontFlip";
        keyDict[KeyCode.DownArrow] = "BackFlip";
        keyDict[KeyCode.LeftArrow] = "BasicTrick1";
        keyDict[KeyCode.RightArrow] = "BasicTrick2";
        keyDict[KeyCode.Joystick1Button4] = "BasicTrick1";
        keyDict[KeyCode.Joystick1Button5] = "BasicTrick2";
        keyDict[KeyCode.Joystick1Button6] = "FrontFlip";
        keyDict[KeyCode.Joystick1Button7] = "BackFlip";

        currentTrickScore = 0;

        currentNode = root;
        // Controller Combos
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button6 }, "FrontFlip");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button7 }, "BackFlip");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button4 }, "BasicTrick1");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button5 }, "BasicTrick2");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button6, KeyCode.Joystick1Button6, KeyCode.Joystick1Button6 }, "SpecialTrick1");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button6, KeyCode.Joystick1Button4, KeyCode.Joystick1Button5, KeyCode.Joystick1Button7 }, "SpecialTrick2");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button7, KeyCode.Joystick1Button7, KeyCode.Joystick1Button7 }, "SpecialTrick3");
        AddCombo(new List<KeyCode> { KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button6 }, "SpecialTrick4");
        // Keyboard Combos
        AddCombo(new List<KeyCode> { KeyCode.UpArrow }, "FrontFlip");
        AddCombo(new List<KeyCode> { KeyCode.DownArrow }, "BackFlip");
        AddCombo(new List<KeyCode> { KeyCode.LeftArrow }, "BasicTrick1");
        AddCombo(new List<KeyCode> { KeyCode.RightArrow }, "BasicTrick2");
        AddCombo(new List<KeyCode> { KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.UpArrow }, "SpecialTrick1");
        AddCombo(new List<KeyCode> { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.DownArrow }, "SpecialTrick2");
        AddCombo(new List<KeyCode> { KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.DownArrow }, "SpecialTrick3");
        AddCombo(new List<KeyCode> { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.UpArrow }, "SpecialTrick4");
    }

    void Update()
    {
        KeyCode? triggerKey = null;

        onRail = rail.onRail;
        isWallRunning = wall.isWallRunning;
        //AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log("Animation Time On Input: " + animatorState.normalizedTime + "buffer: " + (1 - percentageBuffer));
        if (onRail || isWallRunning)
        {
            // Handles LT and RT (they aren't defined as keycodes so have to be dealt with differently)
            if (Math.Round(Input.GetAxisRaw("RightTrigger")) == 1)
            {
                if (rightTriggerPressed == false)
                {
                    triggerKey = KeyCode.Joystick1Button7;
                    TriggerCombo(triggerKey);
                    rightTriggerPressed = true;
                }
            }
            if (Math.Round(Input.GetAxisRaw("RightTrigger")) == 0)
            {
                rightTriggerPressed = false;
            }
            if (Math.Round(Input.GetAxisRaw("LeftTrigger")) == -1)
            {
                if (leftTriggerPressed == false)
                {
                    triggerKey = KeyCode.Joystick1Button6;
                    TriggerCombo(triggerKey);
                    leftTriggerPressed = true;
                }
            }
            if (Math.Round(Input.GetAxisRaw("LeftTrigger")) == 0)
            {
                leftTriggerPressed = false;
            }


            foreach (KeyCode key in new[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.Joystick1Button4, KeyCode.Joystick1Button5 })
            {

                if (Input.GetKeyDown(key))
                {
                    AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
                    float timeAddition = 0f;

                    if (!firstPress || lastKeyPressed != key)
                    {
                        timeAddition = 1f;
                        firstPress = true;
                        //Debug.Log("LAST KEY DIFFERENT");
                    }
                    // If the current animation finishes, allow key input to go through
                    //Debug.Log("Animation Time On Input: " + animatorState.normalizedTime + "buffer: " + (1 - percentageBuffer));
                    if (animatorState.normalizedTime + timeAddition >= (1 - percentageBuffer))
                    {
                        if (currentNode.children.TryGetValue(key, out ComboNode nextNode))
                        {
                            currentNode = nextNode;
                            lastKeyPressed = key;
                            comboTimer = comboMaxTime;

                            if (nextNode.animationTrigger != null)
                            {
                                // Triggers immediately b/c no further branches possible (i.e A and D)
                                animator.SetTrigger(nextNode.animationTrigger);
                                // TRICK FINISHES HERE
                                TrickName = nextNode.animationTrigger;
                                currentTrickScore = nextNode.points;
                                scoreCombo.UpdateTrickScore();

                                InGameCanvas.instance.UpdateTrickName(TrickName);

                                if (nextNode.children.Count == 0)
                                {
                                    ResetCombo();
                                }

                            }
                            // Else wait for more input or timeout
                        }
                        else
                        {
                            // Invalid combo path — reset
                            currentTrickScore = 0;
                            scoreCombo.UpdateTrickScore();
                            ResetCombo();
                        }
                    }

                    break; // Only handle one key per frame
                }
            }

            // Handle timeout fallback for delayed triggers (e.g., W was a valid single-key combo, but could be part of W W W)
            if (comboTimer > 0)
            {
                comboTimer -= Time.deltaTime;

                if (comboTimer <= 0)
                {

                    // Ran out of time for combo
                    currentTrickScore = 0;
                    ResetCombo();
                }
            }
        }
       
        firstPress = false;
    }
    // Adds combos to the trie
    private void AddCombo(List<KeyCode> sequence, string animationTrigger)
    {
        
        FieldInfo publicField;
        ComboNode node = root;
        foreach (KeyCode key in sequence)
        {
            if (!node.children.ContainsKey(key))
            {
                node.children[key] = new ComboNode();
            }
            
            node = node.children[key];
            node.animationTrigger = keyDict[key];
            publicField = GetType().GetField(node.animationTrigger);
            node.points = (float)publicField.GetValue(this);
    
        }
        node.animationTrigger = animationTrigger;
        publicField = GetType().GetField(animationTrigger);
        node.points = (float)publicField.GetValue(this);
    }

    private void ResetCombo()
    {
        scoreCombo.score += currentTrickScore;
        currentTrickScore = 0;

        currentNode = root;
        comboTimer = 0f;
    }

    private void TriggerCombo(KeyCode? triggerKey)
    {
        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        bool firstPress = false;
        float timeAddition = 0f;

        if (!firstPress || lastKeyPressed != triggerKey)
        {
            timeAddition = 1f;
            firstPress = true;
            //Debug.Log("LAST KEY DIFFERENT");
        }
        if (animatorState.normalizedTime + timeAddition >= (1 - percentageBuffer))
        {
            if (currentNode.children.TryGetValue(triggerKey.Value, out ComboNode nextNode))
            {
                currentNode = nextNode;
                lastKeyPressed = triggerKey;
                comboTimer = comboMaxTime;

                if (nextNode.animationTrigger != null)
                {
                    animator.SetTrigger(nextNode.animationTrigger);
                    //TRICK FINISHES HERE

                    // Debug.Log("Played animation: " + nextNode.animationTrigger);
                    currentTrickScore = nextNode.points;
                    scoreCombo.UpdateTrickScore();
                    InGameCanvas.instance.UpdateTrickName(TrickName);
                    if (nextNode.children.Count == 0)
                    {
                        ResetCombo();
                    }
                }
            }
            else
            {
                currentTrickScore = 0;
                ResetCombo();
            }
        }
        
    }
    
}
