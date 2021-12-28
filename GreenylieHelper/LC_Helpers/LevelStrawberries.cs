using System;
using System.Collections;
using Celeste;
using Monocle;
using MonoMod.Utils;

namespace Celeste.Mod.GreenylieHelper.LC_Helpers
{
    public class LevelStrawberries
    {

        public int count;
        public int spent;

        public LevelStrawberries()
        {
            
        }
        public int Count()
        {
            this.count = 0;

            Session session = (Engine.Scene as Level).Session;
            
            foreach (EntityID i in session.Strawberries)
            {
                this.count++;
            }
            Logger.Log("GreenylieHelper: LevelStrawberries.Count()", this.count.ToString());

            return this.count;

        }
    }
}
