using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework; 
using Microsoft.Xna.Framework.Graphics; 
namespace lab4._1
{
    class BasicModel
    {
        public Model model { get; protected set; }
        protected Matrix world = Matrix.Identity;
        protected Matrix position = Matrix.Identity;
        protected Matrix scale = Matrix.Identity;
        public BasicModel(Model m, Vector3 position, float scale):this(m)
        {
            this.position = Matrix.CreateTranslation(position);
            this.scale = Matrix.CreateScale(scale);
        }
        public BasicModel(Model m){
            model = m;
        }//public BasicModel() { }
        public virtual void Update()
        {
        }
        public void Draw(Camera camera){    
            Matrix[] transforms = new Matrix[model.Bones.Count];    
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes){
                foreach (BasicEffect be in mesh.Effects) { 
                    be.EnableDefaultLighting();
                    be.Projection = camera.projection; 
                    be.View = camera.view; 
                    be.World = GetWorld() * mesh.ParentBone.Transform;
                    //be.TextureEnabled = true;

                }
                mesh.Draw();
            }
        }
        public virtual Matrix GetWorld() { return world*scale*position; } 

    }

}
