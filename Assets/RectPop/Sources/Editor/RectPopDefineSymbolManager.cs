using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;

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
        }
    }
}