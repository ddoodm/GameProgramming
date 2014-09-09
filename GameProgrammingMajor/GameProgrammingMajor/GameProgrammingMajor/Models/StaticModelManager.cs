using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Stores and manages a list of static geometry.
    /// </summary>
    public class StaticModelManager
    {
        /// <summary>
        /// The list of static models to draw and update. 
        /// </summary>
        public List<StaticModel> models;

        /// <summary>
        /// Add a model to the Static Model Manager
        /// </summary>
        /// <param name="model">The static model to add.</param>
        /// <returns>A handle to the static model that was added.</returns>
        public StaticModel add(StaticModel model)
        {
            models.Add(model);
            return model;
        }

        public StaticModelManager()
        {
            models = new List<StaticModel>();
        }

        public void update(EntityUpdateParams updateParams)
        {
            foreach (StaticModel model in models)
                model.update(updateParams);
        }

        public void draw(EntityDrawParams drawParams)
        {
            foreach (StaticModel model in models)
                model.draw(drawParams);
        }
    }
}
