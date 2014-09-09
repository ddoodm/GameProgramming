using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameProgrammingMajor
{
    /// <summary>
    /// Stores and manages a list of static geometry.
    /// </summary>
    class StaticModelManager
    {
        /// <summary>
        /// The list of static models to draw and update. 
        /// </summary>
        public List<StaticModel> models;

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
