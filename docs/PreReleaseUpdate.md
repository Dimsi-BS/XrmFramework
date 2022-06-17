# How to update a regular project to the prerelease version.

- Delete the DefinitionManager
- ### For Deploy; Deploy.Webresources :
   - go in the Nugget package manager, tick PreRelease on, update the Nugget packages to the latest recent preRelease version.

- ### Core project :
	- delete the XrmFramework.Coreproject package
	- update the XrmFramework package to the latest preRelease version
	- add the latest preRelease version of the XrmFramework.Analyzers package 
	- copy the text 1) in the csproj
- ### Plugins project :
	- update the XrmFramework package to the latest preRelease version.
	- copy the text 1) in the csproj
	- copy the text 2) in the csproj, replace the "PROJECTCORE" part by the name of your Core project, ex : TestProjet.Core 

- ### Remote debugger : (to be completed)
	- unload the project
	**OR**
	- keep the current version of the remote debugger package
	


















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

