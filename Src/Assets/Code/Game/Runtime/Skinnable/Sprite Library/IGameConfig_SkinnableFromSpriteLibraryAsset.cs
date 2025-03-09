using SadJam;
using UnityEngine.U2D.Animation;

namespace Game
{
    public interface IGameConfig_SkinnableFromSpriteLibraryAsset : IGameConfig
    {
        public SpriteLibraryAsset Skin { get; }
    }
}
