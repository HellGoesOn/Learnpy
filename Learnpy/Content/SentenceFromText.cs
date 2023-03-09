using Learnpy.Content.Components;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Learnpy.Content.Scenes;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using static Learnpy.Collision;

namespace Learnpy.Content
{
    public static class SentenceFromText
    {
        internal static string[] levelFileNames;

        public static void Init()
        {
            string s = Directory.GetCurrentDirectory();
            levelFileNames = null;
            levelFileNames = Directory.GetFiles(s + $@"\Content\{GameOptions.Language}\Lessons", "*.txt");
        }

        public static void Load(World world, int lvlId)
        {
            LearnGame learnGame = EntryPoint.Instance;
            learnGame.ResetWorld();
            string fileText = File.ReadAllText(levelFileNames[lvlId]);
            string[] theEntireThing = fileText.Split(new string[] { "♪" + Environment.NewLine }, 2, StringSplitOptions.None);

            if (theEntireThing.Length < 2)
            {
                throw new Exception("Wrong lesson format provided");
            }

            string[] sentences = theEntireThing[1].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            Random rand = new Random();
            foreach (string sentence in sentences)
            {
                string[] words = sentence.Split('♪');

                foreach (string word in words)
                {
                    PieceType p = word != words[words.Length - 1] ? PieceType.Middle : PieceType.End;
                    var e = world.Create();
                    var pos = new Vector2(200 + rand.Next(500), 56 + rand.Next(400));
                    e.Add(new TransformComponent(pos));
                    e.Add(new TextureComponent("PuzzlePiece"));
                    e.Add(new BoxComponent(new AABB(pos + new Vector2(64, 32), new Vector2(64, 32))));
                    e.Add(new PuzzleComponent(p) {StoredText = word });
                    e.Add(new MoveableComponent());
                    e.Add(new DragComponent());
                }
            }

            CompletionSystem completionSystem = world.GetSystem<CompletionSystem>();
            completionSystem.CurrentTarget = theEntireThing[1].Replace("♪", "");
            completionSystem.Task = theEntireThing[0];
        }
    }
}
