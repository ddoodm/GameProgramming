using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameProgrammingMajor
{
    public class NPCManager
    {
        private Game game;

        public List<NPC> npcs;

        public NPCManager(Game game)
        {
            this.game = game;
            npcs = new List<NPC>();
        }

        /// <summary>
        /// Add an NPC to the NPC Manager
        /// </summary>
        /// <param name="entity">The NPC to add to the manager.</param>
        /// <returns>A handle to the NPC that was added to the manager.</returns>
        public NPC add(NPC npc)
        {
            npcs.Add(npc);
            npc.load(game.Content);
            return npc;
        }

        public virtual void update(UpdateParams updateParams)
        {
            foreach (NPC npc in npcs)
                npc.update(updateParams);
        }

        public virtual void draw(DrawParams drawParams)
        {
            foreach (NPC npc in npcs)
                npc.draw(drawParams);
        }
    }
}
