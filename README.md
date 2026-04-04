# Hemogen Regrow Limbs

A RimWorld mod (v1.5) that causes vampires to slowly regrow missing limbs while deathresting.

**Author:** Ragnar  
**Requires:** RimWorld + Ideology DLC

## What it does

### Limb regrowth during deathrest

When a vampire (a pawn with the Deathrest hediff) goes into deathrest, a custom hediff component scans for missing body parts and begins regrowing them one at a time. Regrowth progresses through five stages over roughly 3 in-game days:

1. **Bones** — part is non-functional, minor pain, -2 beauty
2. **Muscles** — part is non-functional, increased pain, -2 beauty
3. **Nerves** — part is 25% functional, peak pain, -2 beauty
4. **Skin** — part is 50% functional, pain easing, -1 beauty
5. **Finishing** — part is fully functional, minor residual pain, -1 beauty

Once regrowth completes, the missing-part hediff is removed and a notification is sent. Only one limb regrows at a time; subsequent missing parts queue up for the next deathrest.

### Stack limit increases

The mod also patches stack limits for many vanilla items to reduce micromanagement:

| Item | Stack limit |
|---|---|
| Resources (base) | 1000 |
| Silver / Gold | 5000 |
| Chemfuel | 1500 |
| Hay | 2000 |
| Components (industrial / spacer) | 500 |
| Medicine (herbal / base) | 250–750 |
| Meals (base) | 100 |
| Wool (base) | 1000 |
| Beer / Wort | 250 |
| Hemogen Packs | 100 |
| Baby Food | 750 |

## How it works (technical)

- `regrowlimbspatch.xml` — adds `HediffCompProperties_DeathrestRegrowingLimbs` to the vanilla `Deathrest` hediff via XML patch. The severity increase per tick is configured here (`0.0004`, roughly 1 limb stage per hour of deathrest).
- `Hediff_RegrowLimb` (C# + XML def) — a ticking hediff applied to the regrowing body part. Severity increases by `1/180000` per tick (3 days total). On reaching severity 1 the hediff removes itself and the limb is restored.
- `StackLimitPatch.xml` — XML patches that replace stack limits on various vanilla `ThingDef`s.

## Source

[Source/DeathRestRegrowsLimbs/](Source/DeathRestRegrowsLimbs/)
