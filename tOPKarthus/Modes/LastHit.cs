﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace tOPKarthus.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on lasthit mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            var allMinions = ObjectManager.Get<Obj_AI_Base>().Where(t => Q.IsInRange(t) && t.IsValidTarget() && t.IsMinion && t.IsEnemy && t.Health < SpellManager.QMultitargetDamage(t)).OrderBy(t => t.Health);

            if (allMinions == null || allMinions.Count() == 0)
            {
                return;
            }

            var QLocation = Prediction.Position.PredictCircularMissileAoe(allMinions.ToArray(), Q.Range, Q.Radius, Q.CastDelay, Q.Speed, Player.Instance.Position);

            if (QLocation == null)
            {
                return;
            }

            var bestPred = QLocation[0];

            if (bestPred == null)
                return;

            foreach (var pred in QLocation)
            {
                if (pred.CastPosition.CountEnemiesInRange(200) == 1)
                {
                    if (Q.IsReady() && Q.IsInRange(pred.CastPosition))
                    {
                        Q.Cast(pred.CastPosition);
                        return;
                    }
                }
            }
            if (Q.IsReady() && Q.IsInRange(bestPred.CastPosition))
            {
                Q.Cast(bestPred.CastPosition);
                return;
            }
        }
    }
}
