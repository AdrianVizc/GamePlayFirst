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

    private const string PENGUIN_DEATH_1 = "PenguinDead1";
    private const string PENGUIN_DEATH_2 = "PenguinDead2";
    private const string PENGUIN_DEATH_3 = "PenguinDead3";

    private void Start()
    {
        startingPosObj.gameObject.SetActive(false);
        endingPosObj.gameObject.SetActive(false);

        startingPos = startingPosObj.position;
        endingPos = endingPosObj.position;

        StartCoroutine(MoveLoop());

        animator = GetComponentInChildren<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
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

            ScoreCombo.Instance.score -= scoreLoss;

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
