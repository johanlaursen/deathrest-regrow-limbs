using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace DeathRestRegrowsLimbs
{
    public class Hediff_RegrowLimb : HediffWithComps
    {

        public override void Tick()
        {
            base.Tick();

            this.Severity += 1f / 180000f; // 3 days to heal

            if (this.Severity >= 1f)
            {
                this.pawn.health.RemoveHediff(this);
                if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    Messages.Message("Regrew missing limb: " + this.Part.Label, pawn, MessageTypeDefOf.PositiveEvent, historical: false);
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }


    /*public class CompProperties_AbilityRegrowLimb : CompProperties_AbilityEffect
    {
        public bool applyToSelf;

        public bool applyToTarget;

        public CompProperties_AbilityRegrowLimb()
            {
            compClass = typeof(CompAbilityEffect_RegrowLimb);
        }
    }


    public class CompAbilityEffect_RegrowLimb : CompAbilityEffect
    {
        public new CompProperties_AbilityRegrowLimb Props => (CompProperties_AbilityRegrowLimb)props;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);

            if (target.Thing is Pawn pawn)
            {
                // Continuously check and regrow missing parts until no more valid parts are found
                while (true)
                {
                    // Retrieve all current missing parts that can be regrown
                    List<Hediff_MissingPart> missingParts = pawn.health.hediffSet
                        .GetMissingPartsCommonAncestors()
                        .Where(h => h != null && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(h.Part))
                        .ToList(); // Convert to List to avoid deferred execution

                    // If no missing parts are found, break the loop
                    if (!missingParts.Any())
                        break;

                    // Process each missing part
                    foreach (var part in missingParts)
                    {
                        // Creating a new regrowth Hediff for each missing part
                        Hediff regrowLimb = HediffMaker.MakeHediff(HediffDef.Named("Hediff_RegrowLimb"), pawn, part.Part);
                        regrowLimb.Severity = 0.01f; // Set the initial severity, adjust as necessary

                        // Remove the old missing part hediff and add the new regrowth hediff
                        pawn.health.RemoveHediff(part);
                        pawn.health.AddHediff(regrowLimb);

                        Log.Message("Added regrowth hediff to part: " + part.Part.def + " (" + part.Part.Label + ")");
                    }

                    // Ensure updates take effect before the next iteration
                    pawn.health.hediffSet.DirtyCache();
                }
       



            *//*   Log.Message("Applying CompAbilityEffect_RegrowLimb to " + target.Pawn);
               IEnumerable<Hediff_MissingPart> hediffs = target.Pawn.health.hediffSet.GetMissingPartsCommonAncestors();
               Log.Message("Found " + hediffs.Count() + " missing parts.");

               Func<Hediff_MissingPart, bool> predicate = (Hediff_MissingPart injury) => (injury != null && !target.Pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(injury.Part));
               IEnumerable<Hediff_MissingPart> injuryList = hediffs.Where(predicate);
               Log.Message("Found " + injuryList.Count() + " valid injuries for regrowth.");*/

            /*if (injuryList.Count() != 0)
            {
                Hediff hediff_injury = injuryList.RandomElement();
                Log.Message("Selected part: " + hediff_injury.Part.def + " for regrowth.");

                Hediff regrowLimb = HediffMaker.MakeHediff(HediffDef.Named("Hediff_RegrowLimb"), target.Pawn, hediff_injury.Part);
                regrowLimb.Severity = 0.01f;
                target.Pawn.health.RemoveHediff(hediff_injury);
                target.Pawn.health.AddHediff(regrowLimb);
                Log.Message("Added regrowth hediff to part: " + hediff_injury.Part.def);

            }
            else
            {
                Log.Message("No valid parts found for regrowth.");

            }*//*
        }
            else
            {
                Log.Message("CompAbilityEffect_RegrowLimb: Target is not a Pawn. " + target.Thing);

            }
        }
    }*/
    public class HediffCompProperties_DeathrestRegrowingLimbs : HediffCompProperties
    {
        public float severityIncrease = 0.01f;  // Default value, can be set in XML

        public HediffCompProperties_DeathrestRegrowingLimbs()
        {
            this.compClass = typeof(HediffComp_DeathrestRegrowingLimbs);
        }
    }

    public class HediffComp_DeathrestRegrowingLimbs : HediffComp
    {
        // This class is responsible for adding regrowing limb hediff when a vampire is deathresting
        public HediffCompProperties_DeathrestRegrowingLimbs Props
        {
            get { return (HediffCompProperties_DeathrestRegrowingLimbs)this.props; }
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            Pawn pawn = base.Pawn;
            // First get all missingParts that don't have bionics and don't have any missing parent parts
            base.CompPostTick(ref severityAdjustment);
            // Retrieve all applicable missing parts first
            var missingParts = pawn.health.hediffSet.GetMissingPartsCommonAncestors()
                                .Where(h => h != null && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(h.Part))
                                .ToList();

            // Prepare a list to collect new Hediffs to be added
            var newHediffs = new List<Hediff>();

            foreach (var hediff in missingParts)
            {
                // Only start regrowing new limb when no other limb is regrowing
                if (!pawn.health.hediffSet.hediffs.Any(hd => hd.def.defName == "Hediff_RegrowLimb"))
                {
                    Hediff regrowLimb = HediffMaker.MakeHediff(HediffDef.Named("Hediff_RegrowLimb"), pawn, hediff.Part);
                    regrowLimb.Severity = 0.01f; // Initial severity
                    newHediffs.Add(regrowLimb);

                    // Mark the old missing part hediff for removal
                    pawn.health.RemoveHediff(hediff);
                }
            }

            // Add the new hediffs after the iteration is complete
            foreach (var newHediff in newHediffs)
            {
                pawn.health.AddHediff(newHediff);
            }

            // Adjust severities in a separate operation
            foreach (var hediff in pawn.health.hediffSet.hediffs.ToList()) // ToList to avoid modification issues
            {
                if (hediff.def.defName == "Hediff_RegrowLimb")
                {
                    hediff.Severity += Props.severityIncrease;
                }
            }
        }
    }
}
