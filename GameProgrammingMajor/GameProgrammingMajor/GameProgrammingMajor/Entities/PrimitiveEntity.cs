using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProgrammingMajor
{
    /// <summary>
    /// An Entity that wraps a Primitive
    /// </summary>
    /// <typeparam name="Vertex">The XNA.Graphics Vertex Format Structure to give this primitive.</typeparam>
    public class PrimitiveEntity<Vertex> : Entity where Vertex : struct
    {
        public Primitive<Vertex> primitive;

        public PrimitiveEntity(Game game, Primitive<Vertex> primitive, Vector3 position, float rotation)
            : base(game, position, rotation)
        {
            // Check that the primitive is built before creating the entity
            if (!primitive.isBuilt)
                primitive.build();

            this.primitive = primitive;
        }

        public PrimitiveEntity(Game game, Primitive<Vertex> primitive)
            : this(game, primitive, Vector3.Zero, 0)
        {
            
        }

        public override bool collidesWith(StaticModel model)
        {
            return base.collidesWith(model);
        }

        public override void load(ContentManager content)
        {

        }

        public override void update(UpdateParams updateParams)
        {
            primitive.update(updateParams);

            // Initialize world matrix
            world = Matrix.Identity;

            // Update world matrix
            world *= Matrix.CreateRotationY(kinematic.orientation);
            world *= Matrix.CreateTranslation(kinematic.position);

            // Provide world matrix to the primitive
            primitive.world = world;
        }

        public override void draw(DrawParams drawParams)
        {
            primitive.draw(drawParams);
        }

    }
}
