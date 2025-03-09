namespace SadJam.Components
{
    public class GameObject_DontDestroyOnLoad : SadJam.Component
    {
        protected override void AwakeOnce()
        {
            base.AwakeOnce();

            DontDestroyOnLoad(gameObject);
        }
    }
}
