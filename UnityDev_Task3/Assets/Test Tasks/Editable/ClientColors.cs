using System.Collections.Generic;
using TestTask.NonEditable;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TestTask.Editable
{
    public class ClientColors : MonoBehaviour
    {
        [SerializeField]
        private Image m_colorObjectPrefab;
        [SerializeField]
        private Transform m_parent;

        private ServerColors m_serverColors;
        private bool m_canGenerateColors = false;
        private void Start()
        {
            ClientPacketsHandler.ClientConnectedToServer += OnClientConnectedToServer;
        }
        private void OnClientConnectedToServer()
        {
            m_canGenerateColors = true;
        }
        public void GenerateColors()
        {
            if (m_canGenerateColors)
            {
                m_serverColors = new ServerColors();
                foreach (Transform child in m_parent)
                {
                    Destroy(child.gameObject);
                }
                foreach (var color in m_serverColors.GetServerColors())
                {
                    Image newImage = Instantiate(m_colorObjectPrefab, m_parent);
                    newImage.color = color;
                }
            }
        }
    }
}
