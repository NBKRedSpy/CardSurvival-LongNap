using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace CardSurvival_LongNap
{


    [HarmonyPatch(typeof(GameManager), "Awake")]
    public static class AddSleepAction_GameMangerAwake
    {
        private static bool Inited = false;

        public static void Postfix()
        {

            try
            {
                //Note - updating here since the TimeOptions appear to not exist during game load.
                //Checking for init since this is called every time a game is loaded.
                if (Inited == false)
                {

                    Plugin.LocalizationInfo = new LocalizationInfo();
                    Plugin.LocalizationInfo.Load(Path.Combine(Plugin.ModFolder, "lang"));

                    //Get the "Sleep" action.
                    DismantleCardAction sleep = GameManager.Instance.TimeOptions.Actions
                        .Single(x => x.ActionName.LocalizationKey == "TimeSkipOptions_Actions[2].ActionName");

                    MethodInfo memberwiseCloneInfo = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);


                    //Make a clone of the sleep object
                    DismantleCardAction clone = (DismantleCardAction)memberwiseCloneInfo.Invoke(sleep, Array.Empty<object>());

                    clone.ActionName = (LocalizedString)memberwiseCloneInfo.Invoke(clone.ActionDescription, Array.Empty<object>());
                    clone.ActionName.DefaultText = "Long Nap";
                    clone.ActionName.LocalizationKey = "LongNap";
                    clone.ActionName.SetLocalization();

                    //It seems that DayTime cost is the important one?  It is serialized while TotalDaytimeCost is not.
                    //DayTimeCost It is also when creating a new dismantle action.

                    FieldInfo daytimeCostField = AccessTools.Field(typeof(CardAction), "DaytimeCost");

                    daytimeCostField.SetValue(clone, 24);

                    clone.StatModifications = (StatModifier[])clone.StatModifications.Clone();

                    StatModifier target;

                    //SleepClock
                    target = clone.StatModifications.Single(x => x.Stat.UniqueID == "ca25b2c02ece6674bae6aaba4a6b8c10");
                    target.ValueModifier = new UnityEngine.Vector2(23, 23);

                    //SleepRisk
                    target = clone.StatModifications.Single(x => x.Stat.UniqueID == "bbf02bb0a49d7d94a8efe6cee8cf18fe");
                    target.ValueModifier = new UnityEngine.Vector2(23, 23);


                    var actions = new List<DismantleCardAction>(GameManager.Instance.TimeOptions.Actions);

                    //Insert after the nap entry.
                    actions.Insert(actions.IndexOf(sleep) + 1, clone);

                    GameManager.Instance.TimeOptions.Actions = actions.ToArray();
                    
                    Inited = true;
                }
            }
            catch (Exception ex)
            {
                Inited = true;
                Plugin.Log.LogError($"Error adding the new sleep object {ex}");
            }

        }

    }
}
