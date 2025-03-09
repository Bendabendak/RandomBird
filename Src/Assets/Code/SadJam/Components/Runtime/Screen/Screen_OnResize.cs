using System;
using System.Collections;
using TypeReferences;
using UnityEngine;

namespace SadJam.Components 
{
    [ClassTypeAddress("Executor/Screen/OnResize")]
    [CustomStaticExecutor("HguR4zV_uECIGvfaSsAtiQ")]
    public class Screen_OnResize : StaticExecutor
    {
        [NonSerialized]
        private Vector2 lastRes;

        protected override void Start()
        {
            base.Start();

            lastRes = new(Screen.width, Screen.height);

            StartCoroutine(CheckResCor());
        }

        private IEnumerator CheckResCor() 
        {
            while (true) 
            {
                if (lastRes.x != Screen.width || lastRes.y != Screen.height)
                {
                    lastRes = new Vector2(Screen.width, Screen.height);

                    Execute(Time.deltaTime);
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}
