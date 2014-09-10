using System.Linq;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
namespace lab4._1
{
    /// <summary>
    /// Helper class for drawing a tank model with animated wheels and turret.
    /// </summary>
    public class Tank:Entity
    {
        

        // The XNA framework Model object that we are going to display.
        Model tankModel;
        public Matrix world;
        Matrix rotation = Matrix.Identity;

        // Shortcut references to the bones that we are going to animate.
        // We could just look these up inside the Draw method, but it is more
        // efficient to do the lookups while loading and cache the results.
        ModelBone leftBackWheelBone;
        ModelBone rightBackWheelBone;
        ModelBone leftFrontWheelBone;
        ModelBone rightFrontWheelBone;
        ModelBone leftSteerBone;
        ModelBone rightSteerBone;
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone hatchBone;


        // Store the original transform matrix for each animating bone.
        Matrix leftBackWheelTransform;
        Matrix rightBackWheelTransform;
        Matrix leftFrontWheelTransform;
        Matrix rightFrontWheelTransform;
        Matrix leftSteerTransform;
        Matrix rightSteerTransform;
        Matrix turretTransform;
        Matrix cannonTransform;
        Matrix hatchTransform;

        
        // Array holding all the bone transform matrices for the entire model.
        // We could just allocate this locally inside the Draw method, but it
        // is more efficient to reuse a single array, as this avoids creating
        // unnecessary garbage.
        Matrix[] boneTransforms;
        // Current animation positions.
        float wheelRotationValue;
        float steerRotationValue;
        float turretRotationValue;
        float cannonRotationValue;
        float hatchRotationValue;
        /// <summary>
        /// Gets or sets the wheel rotation amount.
        /// </summary>
        public float WheelRotation
        {
            get { return wheelRotationValue; }
            set { wheelRotationValue = value; }
        }
        /// <summary>
        /// Gets or sets the steering rotation amount.
        /// </summary>
        public float SteerRotation
        {
            get { return steerRotationValue; }
            set { steerRotationValue = value; }
        }
        /// <summary>
        /// Gets or sets the turret rotation amount.
        /// </summary>
        public float TurretRotation
        {
            get { return turretRotationValue; }
            set { turretRotationValue = value; }
        }
        /// <summary>
        /// Gets or sets the cannon rotation amount.
        /// </summary>
        public float CannonRotation
        {
            get { return cannonRotationValue; }
            set { cannonRotationValue = value; }
        }
        /// <summary>
        /// Gets or sets the entry hatch rotation amount.
        /// </summary>
        public float HatchRotation
        {
            get { return hatchRotationValue; }
            set { hatchRotationValue = value; }
        }
        public Tank(Matrix world):base()
        {
            this.world = world;
            position = world.Translation;
        }
        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            tankModel = content.Load<Model>(@"Models\Tank\tank");

            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = tankModel.Bones["l_back_wheel_geo"];
            rightBackWheelBone = tankModel.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = tankModel.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = tankModel.Bones["r_front_wheel_geo"];
            leftSteerBone = tankModel.Bones["l_steer_geo"];
            rightSteerBone = tankModel.Bones["r_steer_geo"];
            turretBone = tankModel.Bones["turret_geo"];
            cannonBone = tankModel.Bones["canon_geo"];
            hatchBone = tankModel.Bones["hatch_geo"];

            // Store the original transform matrix for each animating bone.
            leftBackWheelTransform = leftBackWheelBone.Transform;
            rightBackWheelTransform = rightBackWheelBone.Transform;
            leftFrontWheelTransform = leftFrontWheelBone.Transform;
            rightFrontWheelTransform = rightFrontWheelBone.Transform;
            leftSteerTransform = leftSteerBone.Transform;
            rightSteerTransform = rightSteerBone.Transform;
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            hatchTransform = hatchBone.Transform;

            // Allocate the transform matrix array.
            boneTransforms = new Matrix[tankModel.Bones.Count];
        }
        public void Update(GameTime gameTime)
        {
            base.Update();//set ai type during init have it follow target through this
            world.Translation = position;
            rotation = Matrix.CreateRotationY((float)Math.Atan2(velocity.X, velocity.Z));
            
        }
        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            // Set the world matrix as the root transform of the model.
            tankModel.Root.Transform = rotation * world;

            // Calculate matrices based on the current animation position.
            Matrix wheelRotation = Matrix.CreateRotationX(wheelRotationValue);
            Matrix steerRotation = Matrix.CreateRotationY(steerRotationValue);
            Matrix turretRotation = Matrix.CreateRotationY(turretRotationValue);
            Matrix cannonRotation = Matrix.CreateRotationX(cannonRotationValue);
            Matrix hatchRotation = Matrix.CreateRotationX(hatchRotationValue);


            // Apply matrices to the relevant bones.
            leftBackWheelBone.Transform = wheelRotation * leftBackWheelTransform;
            rightBackWheelBone.Transform = wheelRotation * rightBackWheelTransform;
            leftFrontWheelBone.Transform = wheelRotation * leftFrontWheelTransform;
            rightFrontWheelBone.Transform = wheelRotation * rightFrontWheelTransform;
            leftSteerBone.Transform = steerRotation * leftSteerTransform;
            rightSteerBone.Transform = steerRotation * rightSteerTransform;
            turretBone.Transform = turretRotation * turretTransform;
            cannonBone.Transform = cannonRotation * cannonTransform;
            hatchBone.Transform = hatchRotation * hatchTransform;

            // Look up combined bone matrices for the entire model.
            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
            // Draw the model.
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }

        }
    }
}
