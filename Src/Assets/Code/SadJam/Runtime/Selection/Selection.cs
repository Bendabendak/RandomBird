using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SadJam
{
    [Serializable]
    public class Selection
    {
        [field: SerializeField]
        public List<string> Collection { get; private set; }
        [field: SerializeField]
        public string Selected { get; set; }

        public void ChangeCollection(List<string> collection)
        {
            if(collection == null)
            {
                Collection = null;
                return;
            }

            collection.Sort((x, y) => string.Compare(x, y));
            Collection = collection;
        }

        public Selection(params string[] content)
        {
            Collection = content.ToList();
        }

        public static implicit operator string(Selection input)
        {
            return input.Selected;
        }
    }
}
