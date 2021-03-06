﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace DZAIO_Reborn.Helpers.Entity
{
    class PlayerMonitor
    {
        private static AttackableUnit LastTarget;

        internal static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            if (target.IsValidTarget() && unit.IsValid<Obj_AI_Hero>() && unit.IsMe)
            {
                LastTarget = target;
            }
        }

        internal static AttackableUnit GetLastTarget()
        {
            return LastTarget;
        }


    }
}
