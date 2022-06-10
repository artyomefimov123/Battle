using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public UnityEvent OnTurnBegin = new UnityEvent();
    public bool playerMadeTurn;
    public List<Battler> battlers;
    public Battler currentBattler, targetBattler;
    public GameObject GameOver;

    [SerializeField] float timeToGetToAttackPoints;
    [SerializeField] Transform playerAttackPoint;
    [SerializeField] Transform enemyAttackPoint;
    Vector3[] oldPositions = new Vector3[2];
    public GameObject Blur;
    public Camera camera;
    public List<Battler> EnemyTeam;
    public List<Battler> PlayerTeam;
    public bool PlayerTurn => !currentBattler.isEnemy;

    bool _skippedTurn;
    int _currentBattlerIndex = 1;

    private void Start()
    {
        foreach (Battler b in battlers)
        {
            b.turnOrder = Random.Range(0, 16);
        }
        EnemyTeam = battlers.Where(battler => battler.isEnemy).ToList();
        PlayerTeam = battlers.Where(battler => !battler.isEnemy).ToList();
        battlers = battlers.OrderBy(battler => battler.turnOrder).ToList();

        currentBattler = battlers[0];
        StartCoroutine(MakeTurn());
    }

    IEnumerator MakeTurn()
    {
        while (true)
        {

            OnTurnBegin.Invoke();
            if (currentBattler.isEnemy)
            {
                yield return new WaitForSeconds(1);

                yield return new WaitForSeconds(1 + Random.Range(0.5f, 1.5f)); 
                targetBattler = currentBattler.AI_MakeDecision(PlayerTeam);
                if (targetBattler == null)
                    SkipTurn();
            }
            else
            {
                playerMadeTurn = false;
                yield return new WaitUntil(() => playerMadeTurn);
            }
            if (_skippedTurn)
            {
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                yield return StartCoroutine(Attack());
            }

            currentBattler = battlers[_currentBattlerIndex++ % battlers.Count];
            _skippedTurn = false;
            yield return null;
        }
    }

    public void Player_PickTarget(int index)
    {
        targetBattler = EnemyTeam[index];
        playerMadeTurn = true;
    }

    public void SkipTurn()
    {
        _skippedTurn = true;
        if (PlayerTurn)
            playerMadeTurn = true;
    }

    IEnumerator Attack()
    {

        oldPositions[0] = currentBattler.transform.position;
        oldPositions[1] = targetBattler.transform.position;
        
        camera.orthographicSize = 10f;
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.walkAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.walkAnim, true);
        }
        Blur.SetActive(true);
        float progress = 0;
        while (progress < timeToGetToAttackPoints)
        {
            if (PlayerTurn)
            {
                currentBattler.transform.position = Vector3.Lerp(oldPositions[0], playerAttackPoint.position, progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(oldPositions[1], enemyAttackPoint.position, progress / timeToGetToAttackPoints);
            }
            else
            {
                currentBattler.transform.position = Vector3.Lerp(oldPositions[0], enemyAttackPoint.position, progress / timeToGetToAttackPoints);
                targetBattler.transform.position = Vector3.Lerp(oldPositions[1], playerAttackPoint.position, progress / timeToGetToAttackPoints);
            }
            progress += Time.deltaTime;
            yield return null;
        }

        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.idleAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.idleAnim, true);
        }

        yield return new WaitForSeconds(0.5f);

        currentBattler.animationState.SetAnimation(0, currentBattler.attackAnim, false);
        currentBattler.animationState.AddAnimation(0, currentBattler.idleAnim, true, 0);
        yield return new WaitForSeconds(0.5f);
        targetBattler.animationState.SetAnimation(0, targetBattler.hitAnim, false);
        targetBattler.animationState.AddAnimation(0, targetBattler.idleAnim, true, 0);
        yield return new WaitForSeconds(0.5f);
        targetBattler.ChangeHealth(Random.Range(30, 50));
        //targetBattler.ChangeHealth(100);


        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.walkAnim, true);
        }
        if (targetBattler.usesSpine && targetBattler != null)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.walkAnim, true);
        }

        progress = 0;
        while (progress < timeToGetToAttackPoints)
        {
            if (PlayerTurn)
            {
                if (targetBattler != null)
                {
                    currentBattler.transform.position = Vector3.Lerp(playerAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                    targetBattler.transform.position = Vector3.Lerp(enemyAttackPoint.position, oldPositions[1], progress / timeToGetToAttackPoints);
                }
                else
                {
                    currentBattler.transform.position = Vector3.Lerp(playerAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                }

            }
            else
            {
                if (targetBattler != null)
                {
                    currentBattler.transform.position = Vector3.Lerp(enemyAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                    targetBattler.transform.position = Vector3.Lerp(playerAttackPoint.position, oldPositions[1], progress / timeToGetToAttackPoints);
                }
                else
                {
                    currentBattler.transform.position = Vector3.Lerp(enemyAttackPoint.position, oldPositions[0], progress / timeToGetToAttackPoints);
                }

            }
            progress += Time.deltaTime;
            yield return null;
        }
        if (currentBattler.usesSpine)
        {
            currentBattler.animationState.SetAnimation(0, currentBattler.idleAnim, true);
        }
        if (targetBattler.usesSpine)
        {
            targetBattler.animationState.SetAnimation(0, targetBattler.idleAnim, true);
        }
        Blur.SetActive(false);
        camera.orthographicSize = 20f;
        End();
    }
    public void End()
    {
        foreach (var enemy in EnemyTeam.ToList())
        {
            if (enemy._currentHealth <= 0)
            {

                battlers.Remove(enemy);
                EnemyTeam.Remove(enemy);
                targetBattler = null;
            }
        }
        foreach (var player in PlayerTeam.ToList())
        {
            if (player._currentHealth <= 0)
            {
                battlers.Remove(player);
                PlayerTeam.Remove(player);
                targetBattler = null;
            }
        }
        if (PlayerTeam.Count == 0 || EnemyTeam.Count == 0)
        {
            GameOver.SetActive(true);
        }
    }

}
