using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProgrammingMajor
{
    /// <summary>
    /// The AI manager class for the Tank object
    /// </summary>
    public class Tank : Entity
    {
        protected TankModel model;

        protected ProjectileManager projectileManager;
        public Kinematic turretTarget;

        protected TowerManager level;

        protected TowerTraverser traverser;

        public Tower targetTower;

        protected int id;

        public Tank(Game game, Vector3 position, World world, TowerManager level, TowerTraverser traverser, int id)
            : base(game)
        {
            this.level = level;
            this.traverser = traverser;
            this.id = id;

            model = new TankModel();
            kinematic.position = position;

            initialize(game, world, level);
        }

        public Tank(Game game, World world, TowerManager towerManager, TowerTraverser traverser, int id)
            : this(game, Vector3.Zero, world, towerManager, traverser, id)
        {
            
        }

        public virtual void initialize(Game game, World world, TowerManager level)
        {
            npc = new NPC(game, this, "FSM\\tank_basic.xml");

            projectileManager = new ProjectileManager(game, world, level, level.tankTree);
            projectileManager.maxAmmo = 6;
        }

        public bool Equals(Tank rhs)
        {
            return rhs.id == id;
        }

        public virtual void load(ContentManager content)
        {
            model.Load(content, "tank");
            projectileManager.loadContent(content);
        }

        public override void update(UpdateParams updateParams)
        {
            updateTurret();
            updateWheels();

            // Initialize world matrix and scale the model down
            world = Matrix.Identity * Matrix.CreateScale(0.03f);

            // Update world matrix
            world *= Matrix.CreateRotationY(kinematic.orientation);
            world *= Matrix.CreateTranslation(kinematic.position);

            projectileManager.update(updateParams);
        }

        public override void kill(UpdateParams updateParams)
        {
            traverser.kill();

            // Tell the TowerManager for stat generation
            level.addDeathAt(kinematic.position);
            updateParams.hud.setDeathMap(level.generateDeathMap());

            base.kill(updateParams);
        }

        public override void destroy()
        {
            traverser.kill();
            base.destroy();
        }

        public bool colliding(Tank other)
        {
            foreach(ModelMesh thisMesh in model.model.Meshes)
            {
                // Get bounding sphere for this object
                BoundingSphere thisBs = getBoundingSphere();

                // Check for collision with every mesh in the other model
                foreach (ModelMesh otherMesh in other.model.model.Meshes)
                {
                    BoundingSphere otherBs = other.getBoundingSphere();

                    // Perform check
                    if (thisBs.Intersects(otherBs))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get bounding sphere for this object
        /// </summary>
        public BoundingSphere getBoundingSphere()
        {
            return new BoundingSphere(kinematic.position, 10f);
        }

        public override bool collidesWith(BoundingSphere boundingSphere)
        {
            return getBoundingSphere().Intersects(boundingSphere);
        }

        public override bool collidesWith(BoundingBox boundingBox)
        {
            return (boundingBox.Contains(getBoundingSphere()) == ContainmentType.Contains) || getBoundingSphere().Intersects(boundingBox);
        }

        private void updateTurret()
        {
            // Do not update if there is no target
            if (turretTarget == null)
                return;

            // Get direction to target
            Vector3 targetDirection = turretTarget.position - kinematic.position;

            // Get angle to target
            float theta = (float)Math.Atan2(targetDirection.X, targetDirection.Z);

            // Subtract the tank's body rotation
            theta -= kinematic.orientation;

            // Rotate turret
            model.TurretRotation = theta;
        }

        private void updateWheels()
        {
            model.WheelRotation += kinematic.velocity.Length() / 1000f;

            /* model.SteerRotation =
                (float)Math.Atan2(npc.steering.linear.X, npc.steering.linear.Y)
                + MathHelper.PiOver2; */
        }

        public bool atTarget()
        {
            if (turretTarget == null)
                return false;

            BoundingSphere range = new BoundingSphere(this.kinematic.position, 70f);
            return range.Contains(turretTarget.position) == ContainmentType.Contains;
        }

        public bool awayFromAttacker()
        {
            // TODO: Implement "awayFromAttacker".
            return !nearTurret();
        }

        public bool noAmmo()
        {
            return projectileManager.outOfAmmo();
        }

        public bool lowAmmo()
        {
            return (projectileManager.maxAmmo - projectileManager.shotsFired) < 10;
        }

        public bool nearTurret()
        {
            return attacker != null
                && new BoundingSphere(this.kinematic.position, 400f).Contains(attacker.kinematic.position) == ContainmentType.Contains;
        }

        public bool noAttacker()
        {
            return attacker.dead;
        }

        public void fsm_attackTarget(UpdateParams updateParams)
        {
            // Get direction to target
            Vector3 target = turretTarget.position + Vector3.Up * 10f;
            Vector3 targetDirection = Vector3.Normalize(target - kinematic.position);

            shootAt(updateParams, targetDirection);
        }

        public void fsm_attackTurret(UpdateParams updateParams)
        {
            if (attacker == null)
                return;

            traverser.removePath();

            Vector3 attackerPosition = attacker.kinematic.position;
            Vector3 target = attackerPosition + Vector3.Up * 10f;
            Vector3 targetDirection = Vector3.Normalize(target - kinematic.position);

            shootAt(updateParams, targetDirection);
        }

        public void fsm_evadeAttacker(UpdateParams updateParams)
        {
            
        }

        public void fsm_goToTarget(UpdateParams updateParams)
        {
            if(traverser.pathLength() == 0)
                traverser.pathfindToTeapot();
        }

        private void shootAt(UpdateParams updateParams, Vector3 direction)
        {
            projectileManager.shoot(updateParams, this.kinematic.position + Vector3.Up * 12f, direction, this);
        }

        public override void draw(DrawParams drawParams)
        {
            model.Draw(world, drawParams.camera.view, drawParams.camera.projection);
            projectileManager.draw(drawParams);
        }
    }

    public class TankAggressive : Tank
    {
        public TankAggressive(Game game, Vector3 position, World world, TowerManager level, TowerTraverser traverser, int id)
            : base(game, position, world, level, traverser, id)
        { }

        public TankAggressive(Game game, World world, TowerManager towerManager, TowerTraverser traverser, int id)
            : this(game, Vector3.Zero, world, towerManager, traverser, id)
        { }

        public override void initialize(Game game, World world, TowerManager level)
        {
            npc = new NPC(game, this, "FSM\\tank_aggressive.xml");

            projectileManager = new ProjectileManager(game, world, level, level.tankTree);
            projectileManager.maxAmmo = 12;
            projectileManager.cooldownWait = 50f;
        }

        public override void load(ContentManager content)
        {
            model.Load(content, "red_tank");
            projectileManager.loadContent(content);
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
        public Model model;

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
        protected Matrix[] boneTransforms;

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

        public Vector3 canonPosition
        {
            get { return model.Bones["canon_geo"].Transform.Translation
                 + model.Bones["turret_geo"].Transform.Translation;
            }
        }

        /// <summary>
        /// Loads the tank model.
        /// </summary>
        public void Load(ContentManager content, string modelPath)
        {
            // Load the tank model from the ContentManager.
            model = content.Load<Model>(modelPath);

            // Look up shortcut references to the bones we are going to animate.
            leftBackWheelBone = model.Bones["l_back_wheel_geo"];
            rightBackWheelBone = model.Bones["r_back_wheel_geo"];
            leftFrontWheelBone = model.Bones["l_front_wheel_geo"];
            rightFrontWheelBone = model.Bones["r_front_wheel_geo"];
            leftSteerBone = model.Bones["l_steer_geo"];
            rightSteerBone = model.Bones["r_steer_geo"];
            turretBone = model.Bones["turret_geo"];
            cannonBone = model.Bones["canon_geo"];
            hatchBone = model.Bones["hatch_geo"];

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
            boneTransforms = new Matrix[model.Bones.Count];
        }

        /// <summary>
        /// Draws the tank model, using the current animation settings.
        /// </summary>
        public void Draw(Matrix world, Matrix view, Matrix projection)
        {
            // Set the world matrix as the root transform of the model.
            model.Root.Transform = world;

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
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            drawSolid(world, view, projection);
            //drawShadow(view, projection, sun, plane);
        }

        protected virtual void drawSolid(Matrix world, Matrix view, Matrix projection)
        {
            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
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
    }

    public class PlasmaTankModel : TankModel
    {
        private Effect plasma;

        public PlasmaTankModel(ContentManager content)
        {
            plasma = content.Load<Effect>("Shaders\\plasma");
        }

        protected override void drawSolid(Matrix world, Matrix view, Matrix projection)
        {
            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = plasma;

                    part.Effect.Parameters["World"].SetValue(mesh.ParentBone.Transform * world);
                    part.Effect.Parameters["View"].SetValue(view);
                    part.Effect.Parameters["Projection"].SetValue(projection);
                }
                mesh.Draw();
            }
        }
    }
}
