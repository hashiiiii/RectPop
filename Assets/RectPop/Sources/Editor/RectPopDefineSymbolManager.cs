using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace RectPop.Editor
{
    [InitializeOnLoad]
    public class RectPopDefineSymbolManager
    {
        private const string SymbolUniRx = "RECTPOP_UNIRX";
        private const string SymbolR3 = "RECTPOP_R3";
        private const string AssemblyUniRx = "UniRx";
        private const string AssemblyR3 = "R3";
        private const string Delimiter = ";";
        private const string RectPopAsmdefPath = "Assets/RectPop/Sources/Runtime/Rectpop.asmdef";

        static RectPopDefineSymbolManager()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
            var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(Delimiter).ToList();

            var uniRxFound = IsAssemblyExists(AssemblyUniRx);
            var r3Found = IsAssemblyExists(AssemblyR3);

            UpdateSymbol(defines, uniRxFound, SymbolUniRx);
            UpdateSymbol(defines, r3Found, SymbolR3);

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(Delimiter, defines));

            UpdateAsmdefReferences(uniRxFound);

            return;

            static bool IsAssemblyExists(string assemblyNameKeyword)
            {
                return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Contains(assemblyNameKeyword));
            }

            static void UpdateSymbol(List<string> defines, bool found, string symbol)
            {
                if (found)
                {
                    if (!defines.Contains(symbol))
                    {
                        defines.Add(symbol);
                    }
                }
                else
                {
                    if (defines.Contains(symbol))
                    {
                        defines.Remove(symbol);
                    }
                }
            }

            static void UpdateAsmdefReferences(bool uniRxFound)
            {
                if (!File.Exists(RectPopAsmdefPath))
                {
                    Debug.LogError($"Asmdef file not found: {RectPopAsmdefPath}");
                    return;
                }

                // read and parse the asmdef file
                var asmdefJson = File.ReadAllText(RectPopAsmdefPath);
                var asmdefData = JsonUtility.FromJson<AsmdefData>(asmdefJson);

                // ensure references list is initialized
                asmdefData.references ??= new List<string>();

                var updated = false;

                if (uniRxFound)
                {
                    if (!asmdefData.references.Contains(AssemblyUniRx))
                    {
                        asmdefData.references.Add(AssemblyUniRx);
                        updated = true;
                        Debug.Log("Added UniRx reference to RectPop.asmdef.");
                    }
                }
                else
                {
                    if (asmdefData.references.Remove(AssemblyUniRx))
                    {
                        updated = true;
                        Debug.Log("Removed UniRx reference from RectPop.asmdef.");
                    }
                }

                if (!updated) return;

                // save changes if the asmdef was updated
                var updatedJson = JsonUtility.ToJson(asmdefData, true);
                File.WriteAllText(RectPopAsmdefPath, updatedJson);
                AssetDatabase.Refresh();
            }
        }

        [Serializable]
        private class AsmdefData
        {
            public string name;
            public List<string> references;
            public List<string> includePlatforms;
            public List<string> excludePlatforms;
        }
    }
}