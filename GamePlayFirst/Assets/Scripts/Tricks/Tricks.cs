using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboNode
{
    public Dictionary<KeyCode, ComboNode> children = new Dictionary<KeyCode, ComboNode>();
    public string animationTrigger = null; 
}

public class Tricks : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float comboAdder = 250f; // Adds +x points for every additional key needed for trick
    private float currentTrickScore;

    private ComboNode root = new ComboNode();
    private ComboNode currentNode;
    private float comboTimer;
    private float comboMaxTime = 0.5f;
    private KeyCode? lastKeyPressed = null;

    private PlayerGrind rail;
    private PlayerWall wall;
    private ScoreCombo scoreCombo;
    private bool onRail;
    private bool isWallRunning;

    // Note: There will be a small delay on FrontFlip and BackFlip because W and S are potential starters for combos while D and A are not. There is a small
    // window of time to check if there is consecutive inputs or just a singular input
    void Start()
    {
        rail = GetComponent<PlayerGrind>();
        wall = GetComponent<PlayerWall>();
        scoreCombo = GetComponent<ScoreCombo>();

        currentTrickScore = 0;

        currentNode = root;
        // Combos
        AddCombo(new List<KeyCode> { KeyCode.W }, "FrontFlip");
        AddCombo(new List<KeyCode> { KeyCode.S }, "BackFlip");
        AddCombo(new List<KeyCode> { KeyCode.A }, "BasicTrick1");
        AddCombo(new List<KeyCode> { KeyCode.D }, "BasicTrick2");
        AddCombo(new List<KeyCode> { KeyCode.W, KeyCode.W, KeyCode.W }, "SpecialTrick1");
        AddCombo(new List<KeyCode> { KeyCode.W, KeyCode.A, KeyCode.D, KeyCode.S }, "SpecialTrick2");
        AddCombo(new List<KeyCode> { KeyCode.S, KeyCode.S, KeyCode.S }, "SpecialTrick3");
        AddCombo(new List<KeyCode> { KeyCode.W, KeyCode.S, KeyCode.W }, "SpecialTrick4");
    }

    void Update()
    {
        onRail = rail.onRail;
        isWallRunning = wall.isWallRunning;

        if (onRail || isWallRunning)
        {
            foreach (KeyCode key in new[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D })
            {
                if (Input.GetKeyDown(key))
                {
                    if (currentNode.children.TryGetValue(key, out ComboNode nextNode))
                    {
                        currentNode = nextNode;
                        lastKeyPressed = key;
                        comboTimer = comboMaxTime;

                        if (nextNode.animationTrigger != null && nextNode.children.Count == 0)
                        {
                            // Triggers immediately b/c no further branches possible (i.e A and D)
                            animator.SetTrigger(nextNode.animationTrigger);
                            Debug.Log("Played animation: " + nextNode.animationTrigger);
                            ResetCombo();
                        }
                        // Else wait for more input or timeout
                    }
                    else
                    {
                        // Invalid combo path — reset
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
                    if (currentNode.animationTrigger != null)
                    {
                        animator.SetTrigger(currentNode.animationTrigger);
                        Debug.Log("Played animation: " + currentNode.animationTrigger); ;
                    }
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
            node = node.children[key];
        }
        node.animationTrigger = animationTrigger;
    }

    private void ResetCombo()
    {
        scoreCombo.score += currentTrickScore;
        currentTrickScore = 0;

        currentNode = root;
        comboTimer = 0f;
    }
}
