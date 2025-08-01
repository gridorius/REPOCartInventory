using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace CartInventory.Extensions;

public static class ListExtension
{
    public static List<CodeInstruction> SetField(this List<CodeInstruction> list, Type type, string method,
        Type setType, string fieldName)
    {
        list.Add(new CodeInstruction(OpCodes.Ldloc_1));
        list.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(type, method)));
        list.Add(new CodeInstruction(OpCodes.Stfld, AccessTools.Field(setType, fieldName)));
        return list;
    }
}