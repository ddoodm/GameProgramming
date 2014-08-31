using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Lab5
{
    class Tank : Microsoft.Xna.Framework.GameComponent
    {
        Model model;
        public float tankRotation, tempTankRotation;
        float tankSpeed = 50f;
        public Vector3 tankPosition = new Vector3(0f, 0f, 0f);
        ModelMesh FLWheelMesh, FRWheelMesh, BLWheelMesh, BRWheelMesh;
        Matrix frontWheelOffset, backWheelOffset;
        Matrix rot = Matrix.Identity;

        public Tank(Game game, Model model)
            : base(game)
        {
            this.model = model;
        }
        

        private void wheelTurn()
        {
            Matrix wheelRotation = Matrix.CreateRotationX(0.05f);

            frontWheelOffset = Matrix.CreateTranslation(new Vector3(0f, 0.1f, 5.7f));
            backWheelOffset = Matrix.CreateTranslation(new Vector3(0f, -11.6f, 3.2f));


            FLWheelMesh = model.Meshes["l_front_wheel_geo"];
            FRWheelMesh = model.Meshes["r_front_wheel_geo"];
            BLWheelMesh = model.Meshes["l_back_wheel_geo"];
            BRWheelMesh = model.Meshes["r_back_wheel_geo"];


            Matrix FLWheelTransform = FLWheelMesh.ParentBone.Transform;
            Matrix FRWheelTransform = FRWheelMesh.ParentBone.Transform;
            Matrix BLWheelTransform = BLWheelMesh.ParentBone.Transform;
            Matrix BRWheelTransform = BRWheelMesh.ParentBone.Transform;


            FLWheelMesh.ParentBone.Transform = FLWheelTransform * frontWheelOffset * wheelRotation;
            FRWheelMesh.ParentBone.Transform = FRWheelTransform * frontWheelOffset * wheelRotation;
            BLWheelMesh.ParentBone.Transform = BLWheelTransform * backWheelOffset * wheelRotation;
            BRWheelMesh.ParentBone.Transform = BRWheelTransform * backWheelOffset * wheelRotation;
        }



        public void Update(GameTime gameTime, Vector3 targetDirection, float tankDistance)
        {

            if(tempTankRotation != tankRotation)
            {
                tempTankRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.1f);
            }
            if (tempTankRotation <= tankRotation + 0.1 && tempTankRotation >= tankRotation - 0.1)
            {
                tempTankRotation = tankRotation;
            }

            if (tempTankRotation >= MathHelper.TwoPi || tempTankRotation <= -MathHelper.TwoPi)
            {
                tempTankRotation = 0;
            }

            if (tankDistance >= 1 && tempTankRotation == tankRotation)
            {
                wheelTurn();
                tankPosition += targetDirection * tankSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }




            //Matrix.CreateRotationY(tankRotSpeed)
        }


        public void Draw(GraphicsDevice device, Camera camera)
        {

            

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = transforms[mesh.ParentBone.Index] * Matrix.CreateTranslation(new Vector3(0f, 0f, 37f)) * Matrix.CreateScale(new Vector3(.02f, .02f, .02f)) * Matrix.CreateRotationY(tempTankRotation) * Matrix.CreateTranslation(tankPosition);
                    effect.Projection = camera.projection;
                    effect.View = camera.view;

                }
                mesh.Draw();
            }
        }
    }
}
