using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AudioManager;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform startingPosObj;
    [SerializeField] private Transform endingPosObj;

    [Space]

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float waitTime = 2f;
    [SerializeField] private int scoreLoss = 1000;

    private Vector3 startingPos;
    private Vector3 endingPos;

    private Animator animator;
    private Animator playerAnimator;

    private const string PENGUIN_DEATH_1 = "PenguinDead1";
    private const string PENGUIN_DEATH_2 = "PenguinDead2";
    private const string PENGUIN_DEATH_3 = "PenguinDead3";

    private bool isBumping;
    private bool zeroVel;
    private Movement movement;
    private Rigidbody rb;
    private float bumpForce = 9f;

    private void Start()
    {
        startingPosObj.gameObject.SetActive(false);
        endingPosObj.gameObject.SetActive(false);

        startingPos = startingPosObj.position;
        endingPos = endingPosObj.position;

        animator = GetComponentInChildren<Animator>();

        StartCoroutine(MoveLoop());

        isBumping = false;
        zeroVel = false;
    }

    private void FixedUpdate()
    {
        if (isBumping && zeroVel)
        {
            movement.currentSpeed = 0;
            isBumping = false;
            zeroVel = false;
            movement.enabled = true;
            AudioManager.instance.UnPauseSkateSound();
        }
        if (rb != null && rb.velocity.z == 0)
        {
            zeroVel = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            // Bump
            movement = other.gameObject.GetComponent<Movement>();
            rb = other.gameObject.GetComponent<Rigidbody>();
            playerAnimator = other.gameObject.GetComponentInChildren<Animator>();

            AudioManager.instance.PlayEnvironmentSound("Bonk");
            AudioManager.instance.PauseSkateSound();

            movement.enabled = false;
            Vector3 bumpDir = -rb.transform.forward;
            rb.velocity *= 0.2f;
            rb.velocity = bumpDir * bumpForce;

            isBumping = true;

            switch (Random.Range(1, 4))
            {
                case 1:
                    AudioManager.instance.PlayEnvironmentSound(PENGUIN_DEATH_1);
                    break;
                case 2:
                    AudioManager.instance.PlayEnvironmentSound(PENGUIN_DEATH_2);
                    break;
                case 3:
                    AudioManager.instance.PlayEnvironmentSound(PENGUIN_DEATH_3);
                    break;

            }

            ScoreCombo.Instance.totalScore -= scoreLoss;
            InGameCanvas.instance.UpdatePenguinNegativeScore(scoreLoss);

            playerAnimator.SetTrigger("getHurt");
            animator.SetTrigger("OnHit");

            StartCoroutine(DisableAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }
    private IEnumerator MoveLoop()
    {
        while (true)
        {
            yield return StartCoroutine(MoveTo(endingPos));

            transform.Rotate(0f, 180f, 0f);

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(MoveTo(startingPos));

            transform.Rotate(0f, 180f, 0f);

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
