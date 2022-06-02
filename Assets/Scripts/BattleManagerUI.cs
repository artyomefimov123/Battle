using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManagerUI : MonoBehaviour
{
    public bool mode; 

    public GameObject targetingMarker;
    public GameObject battleMenu;


    int _currentPickIndex;
    float MarkerOffset => battleManagerInstance.currentBattler.markerOffset;


    public BattleManager battleManagerInstance;


    void Awake()
    {
        battleManagerInstance.OnTurnBegin.AddListener(OnTurnStarted);
    }

    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        if (mode)
        {
            targetingMarker.transform.position = battleManagerInstance.EnemyTeam[_currentPickIndex].transform.position + Vector3.up * MarkerOffset;

            if (Input.GetKeyDown(KeyCode.A))
            {
                _currentPickIndex--;
                if (_currentPickIndex < 0) _currentPickIndex = 3;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                _currentPickIndex = (_currentPickIndex + 1) % 4;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                targetingMarker.SetActive(false);
                battleManagerInstance.Player_PickTarget(_currentPickIndex);
                mode = false;
            }
            
        }
    }

    void OnTurnStarted()
    {
        targetingMarker.SetActive(true);
        battleMenu.SetActive(!battleManagerInstance.currentBattler.isEnemy);
        targetingMarker.transform.position = battleManagerInstance.currentBattler.transform.position + Vector3.up * MarkerOffset;
    }

    public void SwitchToTargetingMode()
    {
        mode = true;
        targetingMarker.SetActive(true);
        battleMenu.SetActive(false);

        _currentPickIndex = 0;
    }

    public void SkipTurn()
    {
        battleMenu.SetActive(false);
        battleManagerInstance.SkipTurn();
    }
}
