﻿using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Irehon.Editor
{
    public class ProjectEditorUtilities
    {
        [MenuItem("Project/Show Assets Dependency")]
        private static void ShowAssetsDependenciesWindow()
        {
            AssetNodeBuilder dependenciesNodeBuilder = new AssetNodeBuilder();

            AssetNode node = dependenciesNodeBuilder.GetAssetNodes(GetDependenciesPath());
            
            NodeViewerWindow window = NodeViewerWindow.CreateWindow(node, "Assets dependencies");
            
            window.Show();
        }

        [MenuItem("Project/Show Unused Assets")]
        private static void ShowUnusedAssetsWindow()
        {
            AssetNodeBuilder dependenciesNodeBuilder = new AssetNodeBuilder();

            AssetNode rootNode = dependenciesNodeBuilder.GetAssetNodes(GetUnusedPaths());

            AssetNode assetsNode = (AssetNode)rootNode.FindNode("Assets");

            NodeViewerWindow window = NodeViewerWindow.CreateWindow(assetsNode, "Unused assets");
            
            window.Show();
        }

        private static string[] GetUnusedPaths()
        {
            string[] allAssetsPaths = AssetDatabase.GetAllAssetPaths();
            string[] dependenciesPaths = GetDependenciesPath();

            return allAssetsPaths.Except(dependenciesPaths).ToArray();
        }
        
        private static string[] GetDependenciesPath()
        {
            var gameScenes = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets/Scenes" });
            
            string[] scenesAssetId = new string[gameScenes.Length];

            for (int i = 0; i < gameScenes.Length; i++)
                scenesAssetId[i] = AssetDatabase.GUIDToAssetPath(gameScenes[i]);;

            return AssetDatabase.GetDependencies(scenesAssetId);
        }
    }
}