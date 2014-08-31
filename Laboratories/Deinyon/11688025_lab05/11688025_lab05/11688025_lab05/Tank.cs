using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace _11688025_lab05
{
    /// <summary>
    /// The AI manager class for the Tank object
    /// </summary>
    public class Tank : Entity
    {
        private const float WP_RADIUS = 50f, ROT_THRESH = 0.1f;

        private TankModel model = new TankModel();

        public float acceleration = 0.006f, maxSpeed = .2f, rotSpeed = 0.0001f, maxRotSpeed = 0.0002f;

        public Vector2 position { get; protected set; }
        public Vector2 direction { get; protected set; }
        public Vector2 target { get; protected set; }
        public Vector2 velocity { get; protected set; }
        public float rotation { get; protected set; }
        public float rotVelocity { get; protected set; }
        public float steerVelocity { get; protected set; }
        public Vector3 turretDirection { get; protected set; }

        public Tank(Game game, Vector2 position)
            : base(game)
        {
            this.position = position;
        }

        public Tank(Game game)
            : this(game, Vector2.Zero)
        {
            
        }

        public void load(ContentManager content)
        {
            model.Load(content);
        }

        public void update(GameTime gameTime, WaypointManager waypoint, MouseState mouse)
        {
            // Compute time delta
            float timeDelta = gameTime.ElapsedGameTime.Milliseconds;

            // Initialize world matrix and scale the model down
            world = Matrix.Identity * Matrix.CreateScale(0.075f);

            // Actor target
            target = new Vector2(waypoint.position.X, waypoint.position.Z);

            // Find direction to target from position
            direction = Vector2.Normalize(target - position);

            // Do not update if the position is close enough to the target
            if ((target - position).Length() > WP_RADIUS)
            {
                // Velocity vector has a length (magnitude) of 'acceleration'
                if (velocity.Length() < maxSpeed)
                    velocity += direction * acceleration;
                else
                    velocity = direction * maxSpeed;

                // Perform rotation computations
                updateRotation(timeDelta, waypoint);
            }

            // Decrement velocity once we have reached the target radius
            else velocity *= 0.95f;

            // Rotate wheels semsibly
            model.WheelRotation += velocity.Length() / 25f * timeDelta;

            // Rotate the turrent to follow the mouse
            updateTurret(waypoint);

            // Approach the target and apply time displacement
            position += velocity * timeDelta;

            // Update world matrix
            world *= Matrix.CreateRotationY(rotation);
            world *= Matrix.CreateTranslation(new Vector3(position.X, 0, position.Y));
        }

        private void updateTurret(WaypointManager waypoint)
        {
            turretDirection = new Vector3(
                waypoint.livePosition.X - position.X,
                0,
                waypoint.livePosition.Z - position.Y);

            float turretTarget =
                (float)Math.Atan2(turretDirection.X, turretDirection.Z) - rotation;

            model.TurretRotation = turretTarget;

            // Fire a projectile, just for fun (I wish XNA supported vector swizzling!)
            if(Mouse.GetState().RightButton == ButtonState.Pressed)
                ((Game1)game).projectileManager.shoot(
                    new Vector3(position.X, 25f, position.Y),
                    Vector3.Normalize(turretDirection));
        }

        private void updateRotation(float timeDelta, WaypointManager waypoint)
        {
            // Obtain the angle between the reference and the target using the two-dimensional Arctangent function
            float targetRotation = (float)Math.Atan2(direction.X, direction.Y);

            // Obtain the difference between the current rotation and the target
            float rotDelta = rotation - targetRotation;

            // Steer towards the waypoint
            model.SteerRotation = -rotDelta;

            // If the rotation delta is greater than 180', add / subtract 360' to rotate in a sensible direction
            if (Math.Abs(rotDelta) > Math.PI)
            {
                if (rotDelta > 0)   rotDelta -= MathHelper.TwoPi;
                else                rotDelta += MathHelper.TwoPi;
            }

            // Obtain rotation direction
            float rotDirection = (rotDelta < 0) ? 1 : -1;

            // Reduce velocity if within threshold
            if (Math.Abs(rotDelta) < ROT_THRESH)
                rotVelocity *= 0.001f * timeDelta;
            else
                rotVelocity = rotDirection * maxRotSpeed * timeDelta;

            rotation = (rotation + rotVelocity * timeDelta) % MathHelper.TwoPi;

            //rotation = (rotation + (rotDelta < 0 ? rotSpeed : -rotSpeed)) % MathHelper.TwoPi;
        }

        public void draw(Camera camera, Light sun, Plane plane)
        {
            model.Draw(world, camera.view, camera.projection, sun, plane);
        }
    }

    /// <summary>
    /// Copied from the Tank class of the Microsoft (MSDN) Basic Animation sample project:
    /// http://xbox.create.msdn.com/en-US/education/catalog/sample/simple_animation
    /// 
    /// The class is a wrapper for the "Tank" model
    /// </summary>
    public class TankModel
    {
        // The XNA framework Model object that we are going to display.
        Model tankModel;

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

        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content)
        {
            // Load the tank model from the ContentManager.
            tankModel = content.Load<Model>("tank");

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

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix world, Matrix view, Matrix projection, Light sun, Plane plane)
        {
            // Set the world matrix as the root transform of the model.
            tankModel.Root.Transform = world;

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

            drawSolid(view, projection);
            //drawShadow(view, projection, sun, plane);
        }

        private void drawSolid(Matrix view, Matrix projection)
        {
            // Draw the model.
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.LightingEnabled = true;
                    effect.Alpha = 1.0f;
                    effect.DiffuseColor = Vector3.One;

                    effect.World = boneTransforms[mesh.ParentBone.Index];
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }

                mesh.Draw();
            }
        }

        private void drawShadow(Matrix view, Matrix projection, Light sun, Plane plane)
        {
            // Compute shadow matrix
            Matrix shadow = Matrix.CreateShadow(sun.position, plane);
            shadow *= Matrix.CreateTranslation(new Vector3(0, 1, 0));

            // Draw each mesh in model
            foreach (ModelMesh mesh in tankModel.Meshes)
            {
                // Provide MVP matrices to each shader of each mesh
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //shader.EnableDefaultLighting();
                    effect.LightingEnabled = false;
                    effect.Alpha = 0.4f;
                    effect.DiffuseColor = Vector3.Zero;
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = /*tankModel.Root.Transform*/ (mesh.ParentBone.Transform*tankModel.Root.Transform) * shadow;
                }

                mesh.Draw();
            }
        }
    }
}
