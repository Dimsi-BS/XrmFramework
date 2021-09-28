using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using XrmFramework.Utils;

namespace XrmFramework.MSBuild.Generation
{
    public class TableFileCodeBehindGenerator : ITableFileCodeBehindGenerator
    {
        private readonly FilePathGenerator _filePathGenerator;
        private readonly TableCodeBehindGenerator _tableCodeBehindGenerator;

        public TableFileCodeBehindGenerator(TaskLoggingHelper log, TableCodeBehindGenerator tableCodeBehindGenerator)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            _tableCodeBehindGenerator = tableCodeBehindGenerator;
            _filePathGenerator = new FilePathGenerator();
        }

        public TaskLoggingHelper Log { get; }

        public IEnumerable<string> GenerateFilesForProject(
            IReadOnlyCollection<string> tableFiles,
            string projectFolder,
            string outputPath)
        {
            var codeBehindWriter = new CodeBehindWriter(null);

            if (tableFiles == null)
            {
                yield break;
            }

            foreach (var tableFile in tableFiles)
            {
                string tableFileItemSpec = tableFile;
                var generatorResult = _tableCodeBehindGenerator.GenerateCodeBehindFile(tableFileItemSpec);

                if (!generatorResult.Success)
                {
                    foreach (var error in generatorResult.Errors)
                    {
                        Log.LogError(
                            null,
                            null,
                            null,
                            tableFile,
                            error.Line,
                            error.LinePosition,
                            0,
                            0,
                            error.Message);
                    }

                    continue;
                }

                string targetFilePath = _filePathGenerator.GenerateFilePath(
                    projectFolder,
                    outputPath,
                    tableFile,
                    generatorResult.Filename);

                string resultedFile = codeBehindWriter.WriteCodeBehindFile(targetFilePath, tableFile, generatorResult);

                yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
            }
        }
    }
}
