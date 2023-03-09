using Learnpy.Content.Scenes;

namespace Learnpy.Content.Scenes.Transitions
{
    public interface ISceneTransition
    {
        public void Draw(LearnGame game);
        public void Update(LearnGame game);
        public bool IsFinished();
    }
}
