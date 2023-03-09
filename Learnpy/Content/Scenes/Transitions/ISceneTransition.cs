using Learnpy.Content.Scenes;
using System;

namespace Learnpy.Content.Scenes.Transitions
{
    public interface ISceneTransition
    {
        public void Draw(LearnGame game);
        public void Update(LearnGame game);
        public bool IsFinished();
        public bool SceneChanged();
        public Action OnTransitionEnd();
        public Action OnSceneChanged();
    }
}
