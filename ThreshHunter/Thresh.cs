﻿using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ThreshHunter.Interfaces;
using ThreshHunter.Utility;
using ThreshHunter.Utility.Helpers;

namespace ThreshHunter
{
    class Thresh
    {
        #region Fields
        public static Menu RootMenu { get; set; }

        public static Orbwalking.Orbwalker Orbwalker;

        public static readonly Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>
        {
            {SpellSlot.Q, new Spell(SpellSlot.Q, 1100f)},
            {SpellSlot.W, new Spell(SpellSlot.Q, 920f)},
            {SpellSlot.E, new Spell(SpellSlot.Q, 400f)},
            {SpellSlot.R, new Spell(SpellSlot.R, 440f)},
        };

       

        public static Obj_AI_Hero HookedUnit { get; set; }

        public static float HookEndTick { get; set; }

        public static List<ITHModule> THModules = new List<ITHModule>() { };

        #endregion

        #region Initializations, Skills, Events
        public static void Init()
        {
            LoadSkills();
            LoadEvents();
        }

        private static void LoadSkills()
        {
            spells[SpellSlot.Q].SetSkillshot(0.500f, 60f, 1900f, true, SkillshotType.SkillshotLine);
        }

        private static void LoadEvents()
        {
            Obj_AI_Base.OnBuffAdd += OnBuffAdd;
            Game.OnUpdate += (args) => { OnUpdate(); };
            Orbwalking.BeforeAttack += OW_BeforeAttack;
            Drawing.OnDraw += OnDraw;
        }

        
        #endregion

        #region Event Delegates
        private static void OnUpdate()
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
        }

        private static void OW_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void OnBuffAdd(Obj_AI_Base sender, Obj_AI_BaseBuffAddEventArgs args)
        {
            if ((sender is Obj_AI_Hero) && sender.IsValidTarget(float.MaxValue, false))
            {
                var h = (Obj_AI_Hero)sender;
                Console.WriteLine("{0} - {1}", h.ChampionName, args.Buff.Name);
            }

            if ((sender is Obj_AI_Hero) && sender.IsValidTarget() && (args.Buff.Name == "threshqfakeknockup" || args.Buff.Name == "ThreshQ"))
            {
                HookedUnit = (Obj_AI_Hero)sender;
                HookEndTick = Utils.TickCount + 1500f;
                LeagueSharp.Common.Utility.DelayAction.Add((1500 + Game.Ping + 250), () =>
                {
                    HookEndTick = 0;
                    HookedUnit = null;
                });
            }
        }

        private static void OnDraw(EventArgs args)
        {
        }
        #endregion

        #region Combo

        private static void Combo()
        {
            var target = HookedUnit ?? TargetSelector.GetTarget(spells[SpellSlot.Q].Range, TargetSelector.DamageType.Magical);
            if (spells[SpellSlot.E].IsEnabledAndReady(Orbwalking.OrbwalkingMode.Combo) && target.IsValidTarget(spells[SpellSlot.E].Range))
            {
                switch (EHelper.GetEMode())
                {
                    case EMode.Pull:
                        EHelper.CastFlayPull(target, Orbwalking.OrbwalkingMode.Combo);
                        break;
                    case EMode.Push:
                        EHelper.CastFlayPush(target, Orbwalking.OrbwalkingMode.Combo);
                        break;
                }
            }
        }
        #endregion

    }
}
