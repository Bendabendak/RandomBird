using TypeReferences;

namespace Game
{
    [ClassTypeAddress("Executor/Game/Map/SwitchToLeft")]
    public class Map_SwitchToLeft : Map_Switch
    {
        protected override void DynamicExecutor_OnExecute()
        {
            SwitchToLeft();
        }
    }
}
