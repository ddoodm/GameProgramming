using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace lab4._1
{
    public class Cube
    {
        VertexPositionNormalTexture[] vertices;
        VertexBuffer vertexBuffer;
        BasicEffect effect;
        //Texture2D texture;
        public Matrix translation, rotation, scale;
        Matrix originalTranslation, originalRotation, originalScale;

        Game game;
        Camera camera;
        MouseState prevMouseState;
        public Cube(Game1 game, float scale, Vector3 position)
        {
           this.game = game;
           this.camera = game.camera;
           Initialize(scale, position);
        }
        public void Initialize()
        {
            Initialize(1,Vector3.Zero);
        }
        public void Initialize(float scaleValue, Vector3 translationValue)
        {
            Vector3 righttopback = new Vector3(1, 1, 1);
            Vector3 lefttopback = new Vector3(-1, 1, 1);
            Vector3 righttopfront = new Vector3(1, 1, -1);
            Vector3 lefttopfront = new Vector3(-1, 1, -1);
            Vector3 rightbottomback = new Vector3(1, -1, 1);
            Vector3 leftbottomback = new Vector3(-1, -1, 1);
            Vector3 rightbottomfront = new Vector3(1, -1, -1);
            Vector3 leftbottomfront = new Vector3(-1, -1, -1);

            Vector3 back = new Vector3(0, 0, 1);
            Vector3 top = new Vector3(0, 1, 0);
            Vector3 left = new Vector3(-1, 0, 0);
            Vector3 front = new Vector3(0, 0, -1);
            Vector3 bottom = new Vector3(0, -1,01);
            Vector3 right = new Vector3(1, 0, 0);

            vertices = new VertexPositionNormalTexture[36];
            int i = 0;
            //back
            vertices[i++] = new VertexPositionNormalTexture(lefttopback, back, new Vector2(0, 0));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, back, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomback, back, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, back, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomback, back, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomback, back, new Vector2(0, .5f));
            //top
            vertices[i++] = new VertexPositionNormalTexture(lefttopback, top, new Vector2(0, 1));
            vertices[i++] = new VertexPositionNormalTexture(lefttopfront, top, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, top, new Vector2(.5f, 1));
            vertices[i++] = new VertexPositionNormalTexture(lefttopfront, top, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopfront, top, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, top, new Vector2(.5f, 1));
            //left
            vertices[i++] = new VertexPositionNormalTexture(lefttopback, left, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, left, new Vector2(0, 1));
            vertices[i++] = new VertexPositionNormalTexture(lefttopfront, left, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(lefttopback, left, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomback, left, new Vector2(.5f, 1));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, left, new Vector2(0, 1));
            //front
            vertices[i++] = new VertexPositionNormalTexture(righttopfront, front, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(lefttopfront, front, new Vector2(1, 0));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, front, new Vector2(1, .5f));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomfront, front, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopfront, front, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, front, new Vector2(1, .5f));
            //bottom
            vertices[i++] = new VertexPositionNormalTexture(leftbottomback, bottom, new Vector2(0, 0));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomback, bottom, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, bottom, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomfront, bottom, new Vector2(0, .5f));
            vertices[i++] = new VertexPositionNormalTexture(leftbottomfront, bottom, new Vector2(.5f, 0));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomback, bottom, new Vector2(.5f, .5f));
            //right
            vertices[i++] = new VertexPositionNormalTexture(rightbottomfront, right, new Vector2(1, 1));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, right, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(righttopfront, right, new Vector2(1, .5f));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomback, right, new Vector2(.5f, 1));
            vertices[i++] = new VertexPositionNormalTexture(righttopback, right, new Vector2(.5f, .5f));
            vertices[i++] = new VertexPositionNormalTexture(rightbottomfront, right, new Vector2(1, 1));

            vertexBuffer = new VertexBuffer(game.GraphicsDevice, typeof(VertexPositionTexture), vertices.Length, BufferUsage.None);
            game.GraphicsDevice.SetVertexBuffers(vertexBuffer);
            effect = new BasicEffect(game.GraphicsDevice);
            Texture2D texture = game.Content.Load<Texture2D>(@"Textures/crate(1)");
            effect.Texture = texture; 
            effect.TextureEnabled = true;
            effect.EnableDefaultLighting();
            translation = originalTranslation = Matrix.CreateTranslation(translationValue);
            scale = originalScale = Matrix.CreateScale(scaleValue);
            rotation = originalRotation = Matrix.Identity;
            effect.World = originalScale * originalRotation * originalTranslation;// *rotation; 
            effect.View = camera.view;
            effect.Projection = camera.projection;


        }
        public void Update(GameTime gameTime)
        {
            // Translation 
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                translation *= Matrix.CreateTranslation(-.01f, -.01f, 0);
            if (keyboardState.IsKeyDown(Keys.Right))
                translation *= Matrix.CreateTranslation(.01f, .01f, 0);
            // Rotation 
            if (keyboardState.IsKeyDown(Keys.Up))
                rotation *= Matrix.CreateRotationY(MathHelper.PiOver4 / 60);
            if (keyboardState.IsKeyDown(Keys.Down))
                rotation *= Matrix.CreateRotationY(-MathHelper.PiOver4 / 60);
            if (keyboardState.IsKeyDown(Keys.R))
            {
                translation = originalTranslation;
                scale = originalScale;
                rotation = originalRotation;
            }
            MouseState currMouseState = Mouse.GetState(); 
            if(currMouseState.LeftButton == ButtonState.Pressed
                && (currMouseState.X != prevMouseState.X || currMouseState.Y != prevMouseState.Y)) 
            {
                rotation *= Matrix.CreateRotationY( (MathHelper.PiOver4 / 150)*(currMouseState.X - prevMouseState.X));
                rotation *= Matrix.CreateRotationX((MathHelper.PiOver4 / 150) * (currMouseState.Y - prevMouseState.Y));
            }
            if (currMouseState.ScrollWheelValue > prevMouseState.ScrollWheelValue)
            {
                scale *= Matrix.CreateScale(1.1f);
            }
            if (currMouseState.ScrollWheelValue < prevMouseState.ScrollWheelValue)
            {
                scale *= Matrix.CreateScale(0.9f);
            }
            if (currMouseState.RightButton == ButtonState.Pressed)
            {
                translation.Translation += rotation.Forward*new Vector3(0.1f, 0.1f, 0.1f);
            }
            prevMouseState = currMouseState;

        }
        public void Draw()
        {
            effect.World = scale * rotation * translation;// *rotation; 
            effect.View = camera.view;
            effect.Projection = camera.projection;
            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                int numberTriangles = 12;
                game.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, numberTriangles);
            }
        }
    }
}
