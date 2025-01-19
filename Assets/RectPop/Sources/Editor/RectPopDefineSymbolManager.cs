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
        private const string AssetFolderName = "Assets";
        private const string RootFolderName = "RectPop";
        private const string PostPathForAsmdef = "Sources/Runtime/Rectpop.asmdef";
        private const string PackageName = "jp.hashiiiii.rectpop";

        static RectPopDefineSymbolManager()
        {
            var allBuildTargetGroups = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(group => group != BuildTargetGroup.Unknown && IsValidBuildTargetGroup(group));

            var uniRxFound = IsAssemblyExists(AssemblyUniRx);
            var r3Found = IsAssemblyExists(AssemblyR3);

            foreach (var targetGroup in allBuildTargetGroups)
            {
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                var defines = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget).Split(Delimiter).ToList();

                UpdateSymbol(defines, uniRxFound, SymbolUniRx);
                UpdateSymbol(defines, r3Found, SymbolR3);

                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, string.Join(Delimiter, defines));
            }

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
                var rectPopAsmdefPath = GetRectPopAsmdefPath();
                if (string.IsNullOrEmpty(rectPopAsmdefPath))
                {
                    Debug.LogError("RectPop asmdef path is null or empty");
                    return;
                }

                if (!File.Exists(rectPopAsmdefPath))
                {
                    Debug.LogError($"Asmdef file not found: {rectPopAsmdefPath}");
                    return;
                }

                // read and parse the asmdef file
                var asmdefJson = File.ReadAllText(rectPopAsmdefPath);
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
                File.WriteAllText(rectPopAsmdefPath, updatedJson);
                AssetDatabase.Refresh();
            }

            static string GetRectPopAsmdefPath()
            {
                var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
                var packageInfo = packages.FirstOrDefault(p => p.name == PackageName);

                if (packageInfo != null)
                {
                    return Path.Combine(packageInfo.resolvedPath, PostPathForAsmdef);
                }

                var path = $"{AssetFolderName}/{RootFolderName}/{PostPathForAsmdef}";
                return File.Exists(path) ? path : null;
            }

            static bool IsValidBuildTargetGroup(BuildTargetGroup group)
            {
                var type = typeof(BuildTargetGroup);
                var field = type.GetField(group.ToString());
                if (field == null) return false;

                var isObsolete = Attribute.IsDefined(field, typeof(ObsoleteAttribute));
                return !isObsolete && group != BuildTargetGroup.Unknown;
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
