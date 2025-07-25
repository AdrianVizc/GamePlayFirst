using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Animator animator;
    [SerializeField] private float comboAdder = 250f; // Adds +x points for every additional key needed for trick
    [HideInInspector] public float currentTrickScore;

    private ComboNode root = new ComboNode();
    private ComboNode currentNode;
    [HideInInspector] public float comboTimer;
    private float comboMaxTime = 0.5f;
    private KeyCode? lastKeyPressed = null;

    private PlayerGrind rail;
    private PlayerWall wall;
    private ScoreCombo scoreCombo;
    private bool onRail;
    private bool isWallRunning;
    private Dictionary<KeyCode, String> keyDict;

    // Note: There will be a small delay on FrontFlip and BackFlip because W and S are potential starters for combos while D and A are not. There is a small
    // window of time to check if there is consecutive inputs or just a singular input
    void Start()
    {
        rail = GetComponent<PlayerGrind>();
        wall = GetComponent<PlayerWall>();
        scoreCombo = GetComponent<ScoreCombo>();
        keyDict = new Dictionary<KeyCode, String>();
        keyDict[KeyCode.UpArrow] = "FrontFlip";
        keyDict[KeyCode.DownArrow] = "BackFlip";
        keyDict[KeyCode.LeftArrow] = "BasicTrick1";
        keyDict[KeyCode.RightArrow] = "BasicTrick2";

        currentTrickScore = 0;

        currentNode = root;
        // Combos
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
        onRail = rail.onRail;
        isWallRunning = wall.isWallRunning;

        if (onRail || isWallRunning)
        {
            foreach (KeyCode key in new[] { KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow })
            {
                if (Input.GetKeyDown(key))
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
                            Debug.Log("Played animation: " + nextNode.animationTrigger);
                            currentTrickScore += nextNode.points;
                            if (nextNode.children.Count == 0)
                            {
                                ResetCombo();
                            }
                            //Debug.Log(currentTrickScore);
                        }
                        // Else wait for more input or timeout
                    }
                    else
                    {
                        // Invalid combo path — reset
                        currentTrickScore = 0;
                        ResetCombo();
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
        
    }
    // Adds combos to the trie
    private void AddCombo(List<KeyCode> sequence, string animationTrigger)
    {
        ComboNode node = root;
        foreach (KeyCode key in sequence)
        {
            if (!node.children.ContainsKey(key))
            {
                node.children[key] = new ComboNode();
            }
            //Debug.Log($"Added key to combo: {key} : {node.animationTrigger}");
            node = node.children[key];
            node.animationTrigger = keyDict[key];
        }
        node.animationTrigger = animationTrigger;
        node.points = sequence.Count * comboAdder;
        //Debug.Log($"Added key to combo: {node} : {node.animationTrigger}");
    }

    private void ResetCombo()
    {
        scoreCombo.score += currentTrickScore;
        currentTrickScore = 0;

        currentNode = root;
        comboTimer = 0f;
    }
}
