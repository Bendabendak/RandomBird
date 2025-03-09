using TypeReferences;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Map/SwitchToRight")]
    public class Map_SwitchToRight : Map_Switch
    {
        protected override void DynamicExecutor_OnExecute()
        {
            SwitchToRight();
        }
    }
}
