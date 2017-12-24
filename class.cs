using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
//need refrence to Microsoft.Build.Utilities.Core.dll and Microsoft.Build.

namespace GetAngular2File
{
    public class CreateIndexFile
    {
        private static string rootFolderPath = AppDomain.CurrentDomain.BaseDirectory;
        private static string webProjectName = rootFolderPath.Remove(rootFolderPath.Length - 1).Remove(0, rootFolderPath.Remove(rootFolderPath.Length - 1).LastIndexOf(@"\") + 1);
        private static string pathToAngularProject;
        private static string projectAngularName;
        private static string pathToIndexFile;

        private static FileSystemWatcher watcherProject = new FileSystemWatcher();
        private static FileSystemWatcher watcherDist = new FileSystemWatcher();

        public static void Play()
        {
            watcherProject.Path = rootFolderPath + "/angular/";
            watcherProject.NotifyFilter = NotifyFilters.LastWrite;
            watcherProject.Filter = "*";
            watcherProject.Changed += new FileSystemEventHandler(AngularChange);
            watcherProject.EnableRaisingEvents = true;

            watcherDist.NotifyFilter = NotifyFilters.LastWrite;
            watcherDist.Filter = "*.*";
            watcherDist.Changed += new FileSystemEventHandler(DistFolderChange);
            watcherDist.EnableRaisingEvents = false;

            if (Directory.GetDirectories(rootFolderPath + "/angular/").Length > 0)
                AngularExist();
        }

        private static void AngularChange(object source, FileSystemEventArgs e)
        {
            if (Directory.GetDirectories(rootFolderPath + "/angular/").Length > 0)
            {
                AngularExist();
            }
            else
            {
                watcherDist.EnableRaisingEvents = false;
            }
        }

        public static void AngularExist()
        {
            pathToAngularProject = Directory.GetDirectories(rootFolderPath + "/angular/")[0];
            projectAngularName = pathToAngularProject.Remove(0, pathToAngularProject.LastIndexOf('/') + 1);
            pathToIndexFile = pathToAngularProject + "/dist/index.html";

            string csproj = File.ReadAllText(rootFolderPath + "/" + webProjectName + ".csproj");
            if (!csproj.Contains(@"angular\" + webProjectName + @"\dist\*.*"))
            {
                var p = new Microsoft.Build.Evaluation.Project(rootFolderPath + "/" + webProjectName + ".csproj");
                p.AddItem("Content", @"angular\" + webProjectName + @"\dist\*.*");
                p.Save();
            }

            watcherDist.Path = pathToAngularProject;
            watcherDist.EnableRaisingEvents = true;

            if (Directory.Exists(pathToAngularProject + "/dist"))
            {
                if (File.Exists(pathToIndexFile))
                {
                    CopyIndex();
                }
            }
        }

        private static void DistFolderChange(object source, FileSystemEventArgs e)
        {
            watcherDist.EnableRaisingEvents = false;

            Thread t = new Thread(CopyIndex, 1000);
            t.Start();
        }

        private static void CopyIndex()
        {
            try
            {
                if (Directory.Exists(pathToAngularProject + "/dist"))
                {
                    if (File.Exists(pathToIndexFile))
                    {
                        string indexFile = File.ReadAllText(pathToIndexFile);

                        indexFile = indexFile.Remove(indexFile.IndexOf("<base"), indexFile.IndexOf("<meta name") - indexFile.IndexOf("<base"));

                        indexFile = indexFile.Insert(indexFile.IndexOf("<meta name"), "<base href='/angular/" + projectAngularName + "/dist/'>");

                        using (FileStream fs = new FileStream(rootFolderPath + "/index.html", FileMode.Create))
                        {
                            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                            {
                                w.WriteLine(indexFile);
                            }
                        }

                        using (FileStream fs = new FileStream(pathToIndexFile, FileMode.Create))
                        {
                            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                            {
                                w.WriteLine(indexFile);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            watcherDist.EnableRaisingEvents = true;
        }
    }
}
