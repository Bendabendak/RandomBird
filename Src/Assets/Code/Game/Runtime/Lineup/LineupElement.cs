using System;
using UnityEngine;

namespace Game
{
    public class LineupElement : MonoBehaviour
    {
        public bool Linedup { get; set; } = false;

        protected virtual void OnDisable()
        {
            Linedup = false;
        }
    }
}
