﻿using System;
using System.Collections.Generic;
using System.Text;
using IO = System.IO;

namespace FileManageAndBackupBot
{
    public class FileTree
    {
        public class DirectoryNode
        {
            private LocalDirectory directory;
            private List<File> childFiles;
            private List<LocalDirectory> childDirs;

            public LocalDirectory Directory
            {
                get => directory;
            }

            public File[] ChildFiles
            {
                get => childFiles.ToArray();
            }

            public DirectoryNode(LocalDirectory directory)
            {
                this.directory = directory;
                string[] children = directory.GetChildrenNames();

                foreach(string child in children)
                {
                    Uri uri = new Uri(directory.FullName + child);
                    if (uri.IsFile) childFiles.Add(new File(uri));
                    else childDirs.Add((LocalDirectory)Activator.CreateInstance(typeof(LocalDirectory), uri));
                }
            }
        }

        //DirectoryNode startNode;
        LocalDirectory startLocalDirectory;
        int layer;

        /// <summary>
        /// Creates a new file tree object using custom root driectory object.
        /// </summary>
        /// <param name="directory">The managing directory object of root directory.</param>
        public FileTree(LocalDirectory directory)
        {
            startLocalDirectory = directory;
            layer = 0;
        }

        /// <summary>
        /// Creates a new file tree object using custom root directory path string.
        /// </summary>
        /// <param name="path">The path of custom root directory in file system.</param>
        public FileTree(string path)
        {
            startLocalDirectory = (LocalDirectory)Activator.CreateInstance(typeof(LocalDirectory), path);
            layer = 0;
        }

        /// <summary>
        /// Creats a new file tree object with given path string and layer since custom root directory.
        /// </summary>
        /// <param name="path">The path of directory in file system.</param>
        /// <param name="layer">The layer since custom root directory.</param>
        private FileTree(string path, int layer)
        {
            startLocalDirectory = (LocalDirectory)Activator.CreateInstance(typeof(LocalDirectory), path);
            this.layer = layer;
        }

        /// <summary>
        /// Prints all subfiles and subdirectories of current one hierarchically in output console.
        /// </summary>
        public void PrintFileTree()
        {
            IO.FileSystemInfo[] children = startLocalDirectory.GetChildren();
            foreach (var child in children)
            {
                PrintLine(layer, child.Name);
                if (child.Extension == String.Empty)
                {
                    new FileTree(child.FullName, layer + 1).PrintFileTree();
                }
            }
        }

        /// <summary>
        /// Prints new line in output console with prewritten tabs.
        /// </summary>
        /// <param name="preTabs">Number of prewritten tabs.</param>
        /// <param name="text">Text to write in the output console.</param>
        private void PrintLine(int preTabs, string text)
        {
            string tabs = String.Concat(System.Linq.Enumerable.Repeat("\t", preTabs));
            Console.WriteLine(tabs + text);
        }
    }
}