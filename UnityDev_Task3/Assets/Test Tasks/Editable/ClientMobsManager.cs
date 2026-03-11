using System;
using System.Collections.Generic;
using TestTask.NonEditable;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [SerializeField]
        private ServerMock m_server;
        [SerializeField]
        private Image m_monsterImage;
        [SerializeField]
        private Slider m_damageSlider;
        [SerializeField]
        private TMP_Text m_monsterName;
        [SerializeField]
        private Sprite[] m_monsterImages;
        [SerializeField]
        private int m_damageValue;

        private Dictionary<MonsterNames, Sprite> m_monsterDictionary;
        private ServerMobsManager m_serverMobsManager;
        private MonsterData m_currentMonsterData = null;
        private MonsterData m_previousMonsterData = null;
        private bool m_connectedToServer = false;
        private void Awake()
        {
            m_monsterDictionary = new Dictionary<MonsterNames, Sprite>()
            {
                { MonsterNames.Goblin, m_monsterImages[0] },
                { MonsterNames.Troll, m_monsterImages[1] },
                { MonsterNames.Dragon, m_monsterImages[2] },
                { MonsterNames.Skeleton, m_monsterImages[3] },
                { MonsterNames.Orc, m_monsterImages[4] }
            };
        }
        private void Start()
        {
            ClientPacketsHandler.ClientConnectedToServer += OnClientConnectedToServer;
        }
        private void OnClientConnectedToServer()
        {
            m_connectedToServer = true;
            m_serverMobsManager = m_server.ServerMobsManager;
            UpdateMobVisuals();
            ClientPacketsHandler.ClientConnectedToServer -= OnClientConnectedToServer;
        }
        private void UpdateMobVisuals()
        {
            m_serverMobsManager.SpawnMonster();
            m_currentMonsterData = m_serverMobsManager.MonsterData;
            while (m_previousMonsterData != null && m_currentMonsterData.MonsterType == m_previousMonsterData.MonsterType)
            {
                m_serverMobsManager.SpawnMonster();
                m_currentMonsterData = m_serverMobsManager.MonsterData;
            }
            SetMonsterValues();
        }
        private void SetMonsterValues()
        {
            var healthPercent = m_currentMonsterData.MonsterMaxHealth / m_currentMonsterData.MonsterMaxHealth;
            m_damageSlider.maxValue = healthPercent;
            m_damageSlider.value = healthPercent;
            m_monsterName.text = m_currentMonsterData.MonsterName;
            m_monsterImage.sprite = GetMobImage(m_currentMonsterData.MonsterType);
            m_currentMonsterData.MonsterDamaged += OnMonsterDamaged;
            m_currentMonsterData.MonsterDeath += OnMonsterDied;
        }
        public void DealDamage()
        {
            if (m_connectedToServer) { m_currentMonsterData.TakeDamage(m_damageValue); }
        }

        private void OnMonsterDamaged(float num)
        {
            m_damageSlider.value = num;
        }

        private void OnMonsterDied()
        {
            m_previousMonsterData = m_currentMonsterData;
            m_currentMonsterData.MonsterDeath -= OnMonsterDied;
            UpdateMobVisuals();
        }

        private Sprite GetMobImage(MonsterNames type)
        {
            return m_monsterDictionary[type];
        }
    }
}
