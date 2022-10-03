using Learnpy.Core;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy
{
    public class Assets
    {
        private readonly static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();

        public static SpriteFont DefaultFont { get; private set; }
        public static SpriteFont DefaultFontSmall { get; private set; }

        public static SpriteFont DefaultFontBig { get; private set; }

        public static Texture2D LoadTexture(string id, string path)
        {
            FileStream str = new FileStream("Assets/Art/" + path + ".png", FileMode.Open);
            Texture2D loadedTexture = Texture2D.FromStream(EntryPoint.Instance.GraphicsDevice, str);

            str.Dispose();

            if (loadedTexture != null)
            {
                _textures.Add(id, loadedTexture);
                return loadedTexture;
            }
            throw new Exception("There has been an issue with loading asset: " + path);
        }


        public static void LoadAssets()
        {
            string[] textures = Directory.GetFiles("Assets/Art/");

            foreach (string texture in textures)
            {
                var texturePath = Path.GetFileNameWithoutExtension(texture);
                LoadTexture(texturePath, texturePath);
            }

            var fontDefault = TtfFontBaker.Bake(File.ReadAllBytes("Assets/Fonts/DefFont.ttf"),
                20,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.Cyrillic,
                    CharacterRange.LatinExtendedA
                });

            DefaultFont = fontDefault.CreateSpriteFont(EntryPoint.Instance.GraphicsDevice);

            var fontDefaultBig = TtfFontBaker.Bake(File.ReadAllBytes("Assets/Fonts/DefFont.ttf"),
                60,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.Cyrillic,
                    CharacterRange.LatinExtendedA
                });

            DefaultFontBig = fontDefaultBig.CreateSpriteFont(EntryPoint.Instance.GraphicsDevice);

            var fontDefaultSmall = TtfFontBaker.Bake(File.ReadAllBytes("Assets/Fonts/DefFont.ttf"),
                14,
                1024,
                1024,
                new[]
                {
                    CharacterRange.BasicLatin,
                    CharacterRange.Latin1Supplement,
                    CharacterRange.Cyrillic,
                    CharacterRange.LatinExtendedA
                });

            DefaultFontSmall = fontDefaultSmall.CreateSpriteFont(EntryPoint.Instance.GraphicsDevice);
        }

        public static void Unload()
        {
            foreach (var texture in _textures)
            {
                texture.Value.Dispose();
            }
        }

        public static Texture2D GetTexture(string id) => _textures[id];
    }
}
