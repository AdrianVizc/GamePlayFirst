using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class ScoreCombo : MonoBehaviour
{
    public static ScoreCombo Instance;

    [HideInInspector] public float score; // Player's current score
    [HideInInspector] public float currMultiplier; // Player's current multiplier level (1x, 1.25x, etc.)
    private float totalScore; // Player's total score

    [SerializeField] private int pointValue = 100; // Use to increase each point value
    [SerializeField] private float multiplier = 0.25f; // Use to increase multiplier level
    [SerializeField] private float scoreTimeInterval = 0.25f; // Gain points every 0.25 seconds

    private PlayerGrind rail;
    private PlayerWall wall;
    private Movement movement;
    private Tricks tricks;
    private float totalTime;
    private float combo;
    private bool isCheckingGround;

    private GameObject[] listOfRideables = new GameObject[2];

    private void Start()
    {
        rail = GetComponent<PlayerGrind>();
        wall = GetComponent<PlayerWall>();
        movement = GetComponent<Movement>();
        tricks = GetComponent<Tricks>();

        isCheckingGround = false;
        totalTime = 0f;
        combo = 0f;
        totalScore = 0f;
        currMultiplier = 1 - multiplier;
    }

    private void Awake()
    {
        if (Instance == null) //Code to make script into a singleton
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

            return;
        }
    }

    private void Update()
    {
        if( rail.onRail || wall.isWallRunning)
        {
            // Add to total time when on rail or wall
            totalTime += Time.deltaTime;
        }
        else if(movement.isGrounded && !isCheckingGround)
        {
            // Reset time and consecutive jumps if player touches the ground
            StartCoroutine(CheckGrounded(0.1f));
        }

        // Add to score after time elapsed is greater than the score interval time
        if (totalTime > scoreTimeInterval)
        {
            totalTime = 0;
            score += pointValue; // Passive increase

            // Uncomment code for debugging purposes only to see values:
            // Debug.Log("Score: " + score + "\nCurrent Multiplier: " + currMultiplier);
        }

        // Note: two new variables added
        // #1 "combo" = total tricks + total rails/walls jumped on consecutively until player lands on floor
        // #2 "totalScore" = total score throughout entire run

        InGameCanvas.instance.UpdateMultiplier(currMultiplier);
        InGameCanvas.instance.UpdateTrickPoints(score);
        InGameCanvas.instance.UpdateCombo(combo);
        InGameCanvas.instance.UpdateComboLineColor(combo);
        InGameCanvas.instance.UIPopUp(combo);
    }

    public void UpdateTrickScore()
    {
        score += tricks.currentTrickScore;
        ++combo;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Every time player collides with a rail or wall, add to multiplier
        if(collision.gameObject.CompareTag("rail") ||
            collision.gameObject.CompareTag("wall"))
        {
            // Debug.Log("INDEX 1: " + listOfRideables[0] + "\nINDEX 2: " + listOfRideables[1]);
            // If list of rideables are full and the game object is new, empty the array
            if (listOfRideables[1] != null && (collision.gameObject != listOfRideables[1]))
            {
                System.Array.Clear(listOfRideables, 0, listOfRideables.Length);
            }

            // If list of rideables are empty, set index 1 to game object
            if (listOfRideables[0] == null)
            {
                listOfRideables[0] = collision.gameObject;
                currMultiplier += multiplier;
                ++combo;
            }
            else
            {
                //If index 1 is full, set index 2 to current game object
                listOfRideables[1] = collision.gameObject;

                if (listOfRideables[0] != listOfRideables[1])
                {
                    currMultiplier += multiplier;
                    ++combo;
                    System.Array.Clear(listOfRideables, 0, listOfRideables.Length);
                }
            }
        }
    }

    IEnumerator CheckGrounded(float delay)
    {
        isCheckingGround = true;
        yield return new WaitForSeconds(delay);
        // If player is still on ground after delay
        if(movement.isGrounded && !rail.onRail && !wall.isWallRunning)
        {
            InGameCanvas.instance.DisplayTrickPoints(score);
            totalScore += (score * currMultiplier);
            totalTime = 0;
            score = 0;
            combo = 0;
            currMultiplier = 1 - multiplier;
            //Debug.Log(totalScore);iterator LITE
        }

        isCheckingGround = false;
    }
}
