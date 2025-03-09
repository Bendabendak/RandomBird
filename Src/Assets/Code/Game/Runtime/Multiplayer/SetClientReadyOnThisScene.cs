using Mirror;

namespace Game
{
    public class SetClientReadyOnThisScene : SadJam.Component
    {
        protected override void OnStartAndEnable()
        {
            base.OnStartAndEnable();

            Game.NetworkManager.OnClientStarted -= OnClientStarted;
            Game.NetworkManager.OnClientStarted += OnClientStarted;

            if (NetworkClient.active)
            {
                OnClientStarted();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Game.NetworkManager.OnClientStarted -= OnClientStarted;

            SetClientNotReady();
        }

        private void OnClientStarted()
        {
            SetClientReady();
        }

        protected virtual void SetClientNotReady()
        {
            Game.NetworkManager.SetNotReadyToEnterTheGame();
        }

        protected virtual void SetClientReady()
        {
            Game.NetworkManager.SetReadyToEnterTheGame();
        }
    }
}
