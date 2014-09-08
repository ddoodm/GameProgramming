using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _11688025_lab06
{
    public class Light
    {
        public Vector3 position { get; set; }
        public Vector3 ambient = new Vector3(0.35f, 0.45f, 0.50f);
        public Vector3 diffuse = new Vector3(0.95f, 0.56f, 0.30f);
        public Vector3 specular = new Vector3(.98f, .90f, .89f);

        public Vector3 direction
        {
            get { return Vector3.Normalize(-position); }
        }

        public Light(Vector3 position)
        {
            this.position = position;
        }
    }
}
