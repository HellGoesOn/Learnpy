using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core;
using Learnpy.Core.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public class SceneManager
    {
        public static void SwitchScene(GameState scene, IContext context = null)
        {
            if (scene != EntryPoint.Instance.GameState) {
                EntryPoint.Instance.GameState = scene;
                World w = EntryPoint.Instance.Worlds[scene];
                switch (scene) {
                    case GameState.MainMenu:
                        w.WipeWorld();
                        SoundEngine.StartMusic("MainTheme", true);
                        EntryPoint.Instance.InitializeMainMenu();
                        break;
                    case GameState.Playground:
                        EntryPoint.Instance.ResetWorld();
                        SentenceFromText.Load(EntryPoint.Instance.MainWorld, w.GetSystem<CompletionSystem>().LevelTarget - 1);
                        break;
                    case GameState.Cyberspace:
                        w.WipeWorld();
                        EntryPoint.Instance.InitializeCyberspace();
                        break;
                    case GameState.Combat:
                        w.WipeWorld();
                        SoundEngine.StartMusic("KickBack", true);
                        EntryPoint.Instance.BeginCombat(context as CombatContext);
                        break;
                }
            }
        }
    }
}
