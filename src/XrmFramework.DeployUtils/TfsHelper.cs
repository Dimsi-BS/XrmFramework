﻿// Copyright (c) Christophe Gondouin (CGO Conseils). All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace XrmFramework.DeployUtils
{
    public static class TfsHelper
    {
        public class FileInfo
        {
            public FileInfo(string newName)
            {
                NewName = newName;
            }
            public FileInfo(string newName, string oldName)
                : this(newName)
            {
                OldName = oldName;
            }
            public string NewName { get; private set; }
            public string OldName { get; private set; }
        }

    }
}
