# Migrate definitions from the old definition system to the new table system

This document explains how to setup a project that uses the old Definition system of the framework (using DefinitionManager inside of a VS instance), so that it can use the new latest version of the framework. 



- Clone the XrmFramework project in the folder of your choice.
- Open a powershell window and set the working directory to the one where the executable is located
- launch the executable with the following arguments : path to the dll for the core project of the project you want to migrate and the path to the folder were you want your tables to be saved.

[CoreProject](images/MigrationCoreProject.PNG)


image de la commande 


- You should now have one .table file in your destination folder for each definitions
- you can now delete the definition .cs files from your project (or save them out of your project)

- Now you should update your project to the latest version of the framework (you can do so by following [this tutorial](PreReleaseUpdate.md))
- 
- You can now launch the project manager tool on XrmToolBox and select your project
- You should see the tables that were just generated in the UI, just like a regular project.
- For each of these tables, press the "Refresh attributes" button, the details should update and you should now see the full table with your selected attributes ticked on and your custom names.
- now if you rebuild your old project, you should have the exact same definition classes generated in your project.

