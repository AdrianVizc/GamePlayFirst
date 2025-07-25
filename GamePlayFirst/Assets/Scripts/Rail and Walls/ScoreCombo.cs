using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCombo : MonoBehaviour
{
    [HideInInspector] public float score; // Player's total score
    [HideInInspector] public float currMultiplier; // Player's current multiplier level (1x, 1.25x, etc.)

    [SerializeField] private int pointValue = 100; // Use to increase each point value
    [SerializeField] private float multiplier = 0.25f; // Use to increase multiplier level
    [SerializeField] private float scoreTimeInterval = 0.25f; // Gain points every 0.25 seconds

    private PlayerGrind rail;
    private PlayerWall wall;
    private Movement movement;
    private Tricks tricks;
    private float totalTime;

    private void Start()
    {
        rail = GetComponent<PlayerGrind>();
        wall = GetComponent<PlayerWall>();
        movement = GetComponent<Movement>();
        tricks = GetComponent<Tricks>();

        totalTime = 0f;
        currMultiplier = 1 - multiplier;
    }

    private void Update()
    {
        if( rail.onRail || wall.isWallRunning)
        {
            // Add to total time when on rail or wall
            totalTime += Time.deltaTime;
        }
        else if( movement.isGrounded)
        {
            // Reset time and consecutive jumps if player touches the ground
            totalTime = 0;
            currMultiplier = 1 - multiplier;
        }

        // Add to score after time elapsed is greater than the score interval time
        if (totalTime > scoreTimeInterval)
        {
            totalTime = 0;
            score += (pointValue * currMultiplier);

            // Uncomment code for debugging purposes only to see values:
            Debug.Log("Score: " + score + "\nCurrent Multiplier: " + currMultiplier);
            Debug.Log("Combo Score Bonus: " + tricks.currentTrickScore);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Every time player collides with a rail or wall, add to multiplier
        if(collision.gameObject.CompareTag("rail") ||
            collision.gameObject.CompareTag("wall"))
        {
            currMultiplier += multiplier;
        }
    }
}
