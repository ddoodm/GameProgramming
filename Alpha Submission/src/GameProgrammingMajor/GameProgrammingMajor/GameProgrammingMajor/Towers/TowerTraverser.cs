﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProgrammingMajor
{
    class TowerTraverser
    {
        public Vector2 position;
        List<Vector2> path;
        StaticModel markerModel, moverModel;

        float levelWidth, levelHeight;

        float time;
        float defaultDelay = 1f;
        float delay = 1f;

        public void addPath(List<Vector2> path)
        {
            this.path = path;
            //time = 0;
        }

        public void addModel(StaticModel markerModel)
        {
            this.markerModel = markerModel;
        }

        public void addMoverModel(StaticModel moverModel)
        {
            this.moverModel = moverModel;
        }

        public void Update(UpdateParams updateParams, TowerManager level)
        {
            levelWidth = levelHeight = 20f * level.blocks.GetLength(0);

            if (path.Count > 0)
            {

                time += (float)updateParams.gameTime.ElapsedGameTime.TotalSeconds;
                if (time >= delay)
                {
                    position = path[0];
                    time = 0;
                    TowerType towerType = level.getTowerTypeAt((int)path[0].X / 20, (int)path[0].Y / 20);
                    switch (towerType)
                    {
                        case TowerType.PATH: delay = 0.5f;//road
                            break;
                        case TowerType.GRASS: delay = 1;//grass
                            break;
                        case TowerType.TAR: delay = 1.5f;//tar
                            break;
                    }
                    path.RemoveAt(0);
                }

            }
            else delay = defaultDelay;

            // Update models
            moverModel.update(updateParams);
            markerModel.update(updateParams);
        }

        public void Draw(DrawParams drawParams)
        {
            DrawPath(drawParams);

            moverModel.world = Matrix.CreateTranslation(
                new Vector3(position.X * 2 - levelWidth/2 - 80, 0, position.Y * 2 - levelHeight/2 - 80));
            moverModel.draw(drawParams);
        }

        private void DrawPath(DrawParams drawParams)
        {
            foreach (Vector2 v in path)
            {
                markerModel.world = Matrix.CreateScale(3f) * Matrix.CreateTranslation(
                    new Vector3(v.X * 2 - levelWidth/2 - 80, 10, v.Y * 2 - levelHeight/2 - 80));
                markerModel.draw(drawParams);
            }
        }
    }
}
