using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    public class TreePatch
    {
        StaticModel[] trees;

        public TreePatch(Game game, Rectangle region, Terrain terrain, int count)
        {
            trees = new StaticModel[count];
            Model treeModel = game.Content.Load<Model>("Models\\tree01_model");
            Random r = new Random();

            for (int i = 0; i < count; i++)
            {
                // Place randomly
                Vector3 position = new Vector3(
                    r.Next(region.Left, region.Right),
                    0,
                    r.Next(region.Top, region.Bottom));

                // Align to terrain
                position = new Vector3(position.X, terrain.getYAt(position), position.Z);

                trees[i] = new StaticModel(game, treeModel, Matrix.CreateTranslation(position));
            }
        }

        public void draw(DrawParams drawParams)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            drawParams.graphicsDevice.RasterizerState = rs;

            foreach (StaticModel tree in trees)
            {
                tree.draw(drawParams);
            }

            rs = new RasterizerState();
            drawParams.graphicsDevice.RasterizerState = rs;
        }
    }
}
