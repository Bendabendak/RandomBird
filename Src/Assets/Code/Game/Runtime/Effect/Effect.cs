using SadJam;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Effect", menuName = "Game/Effect/Create")]
    public class Effect : GameConfig, IGameConfig_Effect
    {
        [Serializable]
        public class Blending
        {
            public GameConfig Target;
            public GameConfig BlendOnEnable;
            public GameConfig BlendOnDisable;

            [Space]
            public bool ResetBeforeEnabled = false;
        }

        [BlendableField("Enabled"), SerializeField]
        private bool _enabled = true;
        [BlendableProperty("Enabled")]
        public bool Enabled { get; set; }

        [BlendableField("Icon"), SerializeField]
        private Sprite _icon;
        [BlendableProperty("Icon")]
        public Sprite Icon { get; set; }

        [BlendableField("Duration"), Space, SerializeField]
        private float _duration;
        [BlendableProperty("Duration")]
        public float Duration { get; set; }

        [BlendableField("DurationUnscaled"), SerializeField]
        private bool _durationUnscaled = false;
        [BlendableProperty("DurationUnscaled")]
        public bool DurationUnscaled { get; set; }

        [BlendableField("AllowMultipleEffects"), Space, SerializeField]
        private bool _allowMultipleEffects = false;
        [BlendableProperty("AllowMultipleEffects")]
        public bool AllowMultipleEffects { get; set; }

        [BlendableField("EnableEffectWhenAlreadyActivated"), SerializeField]
        private bool _enableEffectWhenAlreadyActivated = true;
        [BlendableProperty("EnableEffectWhenAlreadyActivated")]
        public bool EnableEffectWhenAlreadyActivated { get; set; }

        [BlendableField("SoloEffect"), SerializeField]
        private bool _soloEffect = false;
        [BlendableProperty("SoloEffect")]
        public bool SoloEffect { get; set; }

        [BlendableField("Blendings"), Space, SerializeField]
        private List<Blending> _blendings;
        [BlendableProperty("Blendings")]
        public List<Blending> Blendings { get; set; }

        private static List<Effect> _effects = new();
        public static ReadOnlyCollection<Effect> Effects => _effects.AsReadOnly();

        protected override void OnEnable()
        {
            base.OnEnable();

            _effects.Add(this);
            _exceptThis = new(1) { this };
        }

        [NonSerialized]
        private List<GameObject> _activeHolders = new();
        [NonSerialized]
        private List<Effect> _exceptThis;
        public virtual bool ActivateEffect(GameObject effectHolder)
        {
            bool contains = false;
            foreach(GameObject g in _activeHolders)
            {
                if (g == effectHolder)
                {
                    contains = true;
                    break;
                }
            }

            if (!AllowMultipleEffects && contains)
            {
                return false;
            }

            _activeHolders.Add(effectHolder);

            if (SoloEffect)
            {
                DisableAllEffects(_exceptThis);
            }

            if (!contains || EnableEffectWhenAlreadyActivated)
            {
                foreach (Blending blending in Blendings)
                {
                    if (blending.ResetBeforeEnabled)
                    {
                        blending.Target.Blend(blending.BlendOnDisable);
                    }
                }

                foreach (Blending blending in Blendings)
                {
                    blending.Target.Blend(blending.BlendOnEnable);
                }
            }

            return true;
        }

        public virtual void RemoveEffect(GameObject effectHolder)
        {
            _activeHolders.Remove(effectHolder);

            bool contains = false;
            foreach (GameObject g in _activeHolders)
            {
                if (g == effectHolder)
                {
                    contains = true;
                    break;
                }
            }

            if (contains)
            {
                return;
            }

            foreach (Blending blending in Blendings)
            {
                blending.Target.Blend(blending.BlendOnDisable);
            }

            if (SoloEffect)
            {
                EnableAllEffects(_exceptThis);
            }
        }

        public void DisableAllEffects(IEnumerable<Effect> except = null)
        {
            if (except == null)
            {
                foreach (Effect e in _effects)
                {
                    if (e == null) continue;

                    e.Enabled = false;
                }
            }
            else
            {
                foreach (Effect e in _effects)
                {
                    if (e == null) continue;

                    bool skip = false;
                    foreach (Effect d in except)
                    {
                        if (d == e)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Enabled = false;
                }
            }
        }

        public void EnableAllEffects(IEnumerable<Effect> except = null)
        {
            if (except == null)
            {
                foreach (Effect e in _effects)
                {
                    if (e == null) continue;

                    e.Enabled = true;
                }
            }
            else
            {
                foreach (Effect e in _effects)
                {
                    if (e == null) continue;

                    bool skip = false;
                    foreach (Effect d in except)
                    {
                        if (d == e)
                        {
                            skip = true;
                            break;
                        }
                    }

                    if (skip) continue;

                    e.Enabled = true;
                }
            }
        }
    }
}
