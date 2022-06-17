# How to update a regular project to the prerelease version.

- Delete the DefinitionManager
- ### For Deploy; Deploy.Webresources :
   - go in the Nugget package manager, tick PreRelease on, update the Nugget packages to the most recent preRelease version.

- ### Pour le projet Core :
	- supprimer le package XrmFramework.Coreproject
	- update le package XrmFramework à la dernière version preRelease
	- ajouter le package XrmFramework.Analyzers en version preRelease
	- copier le texte 1) dans le fichier csproj
- ### Pour le projet Plugin :
	- update les package XrmFramework à la dernière version preRelease 
	- copier le texte 1) dans le fichier csproj
	- copier le texte 2) dans le fichier csproj en remplacant PROJETCORE par le nom du projet Core ex : TestProjet.Core

- ### Remote debugger : (to be completed)
unload the project
	


















Textes à copier :
1)
```XML
	<PropertyGroup>
		<EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
		<CompilerGeneratedFilesOutputPath>Generated</CompilerGeneratedFilesOutputPath>
	</PropertyGroup>
	<ItemGroup>
		<None Remove="..\**\*.table" />
		<AdditionalFiles Include="..\**/*.table" />
		<Compile Remove="Generated\**" />
		<None Include="Generated\**" />
	</ItemGroup>
	<ItemGroup>
		<None Remove="..\**\*.model" />
		<AdditionalFiles Include="..\**/*.model" />
		<Compile Remove="Generated\**" />
		<None Include="Generated\**" />
	</ItemGroup>
	<ItemGroup>
		<Folder Include="Definitions\" />
		<Folder Include="Generated\" />
		<Folder Include="Generated\XrmFramework.Analyzers\XrmFramework.Analyzers.Generators.ModelSourceFileGenerator\" />
		<Folder Include="Generated\XrmFramework.Analyzers\XrmFramework.Analyzers.Generators.TableSourceFileGenerator\" />
	</ItemGroup>
```

2)
```XML
	<ItemGroup>
		<Compile Remove ="..\PROJETCORE\Generated\**\*.cs"/>
	</ItemGroup>
```

3)
```XML
	<ItemGroup>
   <ProjectReference Update="@(ProjectReference)" Exclude="">
      <Aliases>$([System.String]::Copy("%(FileName)").Replace(".",""))</Aliases>
   </ProjectReference>
   <ProjectReference Update="../../Framework/XrmFramework.RemoteDebugger.Client\XrmFramework.RemoteDebugger.Client.csproj">
      <Aliases></Aliases>
   </ProjectReference>
	</ItemGroup>
```

