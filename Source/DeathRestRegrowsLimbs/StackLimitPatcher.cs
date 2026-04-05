using System.Linq;
using Verse;
using RimWorld;

namespace DeathRestRegrowsLimbs
{
    [StaticConstructorOnStartup]
    public static class StackLimitPatcher
    {
        static StackLimitPatcher()
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs
                .Where(d => d.thingCategories != null &&
                            d.thingCategories.Contains(ThingCategoryDefOf.MeatRaw)))
            {
                def.stackLimit = 500;
            }
        }
    }
}
