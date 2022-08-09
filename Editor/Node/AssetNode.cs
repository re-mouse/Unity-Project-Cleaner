﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Irehon.Editor
{
    public class AssetNode : PathNode
    {
        public AssetNode(string name, int depth, bool isFolder) : base(name, depth)
        {
            this.isFolder = isFolder;
        }
        
        private long size;
        private bool isFolder;
        private string cachedReadableSize;
        private float percentTotalSizeFromRoot;
        private Object asset;
        
        public Object GetAsset()
        {
            if (asset == null)
                CacheAsset();
            return asset;
        }

        public long GetTotalSizeBytes()
        {
            if (size == 0)
                CalculateTotalNodeSize();
            return size;
        }

        public string GetReadableTotalSize()
        {
            return cachedReadableSize;
        }

        public void ClearFoldersOnChilds()
        {
            List<AssetNode> childsToRemove = new List<AssetNode>();
            foreach (AssetNode child in childs)
            {
                if (child.IsEndNode() && child.GetFullPath() == "D:/UnityProjects/Irehon/Assets/Models/Training_dummy")
                    Debug.Log(child.GetFullPath());
                if (child.IsEndNode() && child.GetAttributes().HasFlag(FileAttributes.Directory))
                    childsToRemove.Add(child);
                else
                    child.ClearFoldersOnChilds();
            }

            foreach (AssetNode removingChild in childsToRemove)
                childs.Remove(removingChild);
        }

        public long CalculateTotalNodeSize()
        {
            if (IsEndNode() && !isFolder)
            {
                size = GetFileInfo().Length;
            }
            else
            {
                size = 0;
                
                foreach (AssetNode child in childs)
                    size += child.CalculateTotalNodeSize();
            }

            cachedReadableSize = BytesToString(size);

            return size;
        }

        private long GetRootTotalSize()
        {
            Node<string> node = this;
            while (!node.IsRootNode())
                node = node.parent;

            AssetNode rootAssetNode = (AssetNode)node;
            return rootAssetNode.size;
        }

        private void CacheAsset()
        {
            if (!IsEndNode())
                return;
            
            asset = EditorUtility.FindAsset(GetRelativePath(), typeof(Object));
        }

        private FileInfo GetFileInfo()
        {
            return new FileInfo(GetFullPath());
        }

        private FileAttributes GetAttributes()
        {
            return File.GetAttributes(GetFullPath());
        }

        private string GetFullPath()
        {
            StringBuilder path = new StringBuilder();
            
            Node<string> currentNode = this;
            
            int i = 0;

            if (currentNode.parent != null && !currentNode.parent.IsRootNode())
            {
                while (currentNode != null && currentNode.parent != null)
                {
                    string name = currentNode.GetData();

                    if (i != 0)
                        name += "/";

                    path.Insert(0, name);
                    currentNode = currentNode.parent;
                    i++;
                }
            }
            else
                path.Insert(0, currentNode.GetData());

            path.Insert(0, Application.dataPath + "/");

            return path.ToString();
        }
        
        private string BytesToString(long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            
            if (byteCount == 0)
                return "0" + suf[0];
            
            long bytes = Math.Abs(byteCount);
            
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return Math.Sign(byteCount) * num + suf[place];
        }
    }
}