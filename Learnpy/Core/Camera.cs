using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnpy.Core
{
    public class Camera
    {
        public Viewport view;
        public Vector2 centre;
        public Matrix transform;
        public float zoom;
        public float speed;
        public float rotation;

        public Camera(Viewport viewport)
        {
            speed = 2f;
            zoom = 1f;
            this.view = viewport;
        }

        public void Update()
        {
            transform = Matrix.CreateScale(new Vector3(zoom, zoom, 0))
                * Matrix.CreateTranslation(-centre.X, -centre.Y, 0)
                *Matrix.CreateRotationZ(rotation)
                * Matrix.CreateTranslation(new Vector3(view.Width * 0.5f, view.Height * 0.5f, 0f));

            if (Input.HeldKey(Keys.NumPad4))
                centre.X -= speed;
            if (Input.HeldKey(Keys.NumPad6))
                centre.X += speed;
            if (Input.HeldKey(Keys.NumPad8))
                centre.Y -= speed;
            if (Input.HeldKey(Keys.NumPad2))
                centre.Y += speed;
            if (Input.HeldKey(Keys.OemPlus))
                zoom += 0.02f;
            if (Input.HeldKey(Keys.OemMinus))
                zoom -= 0.02f;
            if (Input.HeldKey(Keys.D9))
                rotation += 0.02f;
            if (Input.HeldKey(Keys.D0))
                rotation -= 0.02f;

            if (zoom < 0.1f)
                zoom = 0.1f;
        }

        public Vector2 Position => centre - new Vector2(view.Width, view.Height) * 0.5f;
    }
}
