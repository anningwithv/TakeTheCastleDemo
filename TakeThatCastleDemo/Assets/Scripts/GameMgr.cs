using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Qarth;
using UnityEngine.UI;

public class GameMgr : TMonoSingleton<GameMgr>
{
    [SerializeField] private Button m_SpawnBt;

    private void Start()
    {
        m_SpawnBt.onClick.AddListener(SoldierSpawnMgr.S.SpawnSoldier);
        gameObject.AddComponent<InputMgr>();
        gameObject.AddComponent<SoldierSpawnMgr>();
    }
}
