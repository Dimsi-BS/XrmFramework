using System;
using System.Collections.Generic;

namespace XrmFramework.XrmToolbox
{
    /// <summary>
    /// This class can help you to store settings for your plugin
    /// </summary>
    /// <remarks>
    /// This class must be XML serializable
    /// </remarks>
    public class Settings
    {
        
        public string LastUsedOrganizationWebappUrl { get; set; }

        public string CurrentOrganizationName { get; set; }

        public List<Project> RootFolders { get; set; } = new List<Project>();
    }
    [Serializable]
    public class Project
    {
        public Project()
        {

        }
        public Project(string orgName, string folderPath)
        {
            OrganizationName = orgName;
            FolderPath = folderPath;
        }
        public string OrganizationName { get; set; }
        public string FolderPath { get;set; }
    }
}