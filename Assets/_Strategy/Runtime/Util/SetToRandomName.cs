using System.Collections.Generic;
using Doozy.Runtime.Common.Extensions;
using TMPro;
using UnityEngine;

namespace _Strategy.Runtime.Util
{
    public class SetToRandomName : MonoBehaviour
    {
        private static List<string> _names = new() { "Maurice", "Bartholomeus", "Max", "Sandel", "Mattheo", "Steve" };

        private void Awake() => GetComponent<TMP_Text>().SetText(GetRandomName());
        private string GetRandomName() => _names.GetRandomItem();
    }
}