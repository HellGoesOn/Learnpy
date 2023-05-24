using Learnpy.Content.Components;
using Learnpy.Content.Scenes.Transitions;
using Learnpy.Content.Systems;
using Learnpy.Core.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Learnpy.Content.Scenes
{
    public partial class LearnGame : Game
    {
        public string ConnectionString = @"Data Source = DESKTOP-JB2KS99\SQLEXPRESS;Initial Catalog = LearnpyDB; Integrated Security = True;";

        // 0 - typing in login, 1 - typing in pass, 2 - auth attempt
        public int loginProgress;

        public void DoLogin()
        {
            World w = Worlds[GameState.LoginScreen];

            w.AddSystem<DrawSystem>();
            w.AddSystem<TextInputSystem>();

            w.camera.centre = new Vector2(680, 384);

            int[] ids = new int[] { 1, 3 };

            var backDrop = w.Create();
            backDrop.Add(new TransformComponent());
            backDrop.Add(new TextureComponent("Pixel"));
            backDrop.Add(new DrawDataComponent(Vector2.Zero, new Vector2(1360, 768), 1f, Color.Black));
            backDrop.Add(new TextInputComponent("", true));
            backDrop.Add(new AnimationComponent() {
                Action = () =>
                {
                    ref TextInputComponent tic = ref backDrop.Get<TextInputComponent>();

                    ref TextComponent t = ref backDrop.Get<TextComponent>();

                    // no login boot;
                    if (Input.PressedKey(Keys.F9)) {
                        sceneTransitions.Add(new SlideTransition(GameState.LoginScreen, GameState.MainMenu));
                    }

                    t.Get(ids[loginProgress]).Text = string.IsNullOrWhiteSpace(tic.Text) ? "" : tic.Text;
                    t.Get(ids[loginProgress]).Origin = Assets.DefaultFont.MeasureString(tic.Text) * 0.5f;

                    if (Input.PressedKey(Keys.Enter)) {
                        Input.StopTextInput(out var rip);
                        if (loginProgress < ids.Length - 1) {
                            Input.StartTextInput("");
                            loginProgress++;
                        } else {
                            DoAuth(t.Get(ids[0]).Text, t.Get(ids[1]).Text);
                        }
                        tic.Text = "";
                    }

                    if(Input.PressedKey(Keys.Back)) {
                        if(t.Get(ids[loginProgress]).Text == "" && loginProgress > 0) {
                            loginProgress--;
                        }
                    }
                }
            });

            TextContext[] texts = new TextContext[] {
                new TextContext("Введите логин:", new Vector2(680, 344), Assets.DefaultFont) {
                    Color = Color.White,
                    Origin = Assets.DefaultFont.MeasureString("Введите логин:") * 0.5f,
                    ShadowColor = Color.Black * 0f
                },
                new TextContext("", new Vector2(680, 364)) {
                    Color = Color.White,
                    ShadowColor = Color.Black * 0f
                },
                new TextContext("Введите пароль:", new Vector2(680, 384), Assets.DefaultFont) {
                    Color = Color.White,
                    Origin = Assets.DefaultFont.MeasureString("Введите пароль:") * 0.5f,
                    ShadowColor = Color.Black * 0f
                },
                new TextContext("", new Vector2(680, 404)) {
                    Color = Color.White,
                    ShadowColor = Color.Black * 0f
                },
                new TextContext("...", new Vector2(680, 304)) {
                    Color = Color.White,
                    ShadowColor = Color.Black * 0f
                },
            };

            w.GetSystem<TextInputSystem>().isEditingText = true;
            Input.StartTextInput("");

            backDrop.Add(new TextComponent(texts));
        }

        public void DoAuth(string login, string pass)
        {
            World w = Worlds[GameState.LoginScreen];
            SqlConnection connection = new SqlConnection(ConnectionString);

            try {
                connection.Open();
                var cmdText = $"select count(*) from [Users] where Login = '{login}' and Password = '{pass}'";
                var cmdText2 = $"select * from [Users] where Login = '{login}' and Password = '{pass}'";
                var cmd = new SqlCommand(cmdText, connection);
                var reader = cmd.ExecuteReader();
                reader.Read();
                if(reader.GetInt32(0) > 0) {
                    var getUser = Database.Read(cmdText2);
                    getUser.Read();
                    int id = getUser.GetInt32(0);
                    var name = getUser.GetString(4);
                    var groupReader = Database.Read($"select [Groups].Name from [Groups] inner join [Users] on [Users].GroupId = [Groups].Id where Users.Id = '{id}'");
                    groupReader.Read();
                    Database.User = new User() {
                        Id = id,
                        Name = name,
                        Group = groupReader.GetString(0)
                    };
                    sceneTransitions.Add(new SlideTransition(GameState.LoginScreen, GameState.MainMenu));
                } else {
                    ref TextComponent txt = ref w.Entities[0].Get<TextComponent>();
                    ref TextContext t = ref txt.Get(4);
                    t.Text = "Ошибка авторизации. Пользователь не найден.";
                    t.Color = Color.Yellow;
                    t.Origin = Assets.DefaultFont.MeasureString(t.Text) * 0.5f;
                    Input.StartTextInput("");
                }
                connection.Close();
            }
            catch (Exception e) {
                ref TextComponent txt = ref w.Entities[0].Get<TextComponent>();
                ref TextContext t = ref txt.Get(4);
                t.Text = "Ошибка авторизации. Нет доступа к серверу.";
                t.Color = Color.Red;
                t.Origin = Assets.DefaultFont.MeasureString(t.Text) * 0.5f;
                Input.StartTextInput("");
                Console.WriteLine(e);
            }
        }
    }
}
