namespace Learnpy.Core.ECS
{
    public interface ISystem
    {
        void Execute(World gameState);

        void Render(LearnGame gameRenderer);
    }
}
