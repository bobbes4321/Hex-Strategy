using System;
using _Strategy.Runtime.Gameloop;
using Reflex.Attributes;
using Teo.AutoReference;
using TMPro;
using UnityEngine;

namespace _Strategy.Runtime.Util
{
    public class TurnTextSetter : MonoBehaviour
    {
        [Inject] private GameManager _gameManager;
        [SerializeField, Get] private TMP_Text _text;

        private void Update()
        {
            _text.SetText(_gameManager.TurnText);
        }
    }
}