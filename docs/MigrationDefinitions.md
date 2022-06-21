# Migrate definitions from the old definition system to the new table system

- Get the GenerateTableFiles executable
- launch the executable with the path to the dll for the core project of the project you want to migrate and the path to the folder were you want your tables to be saved (it should be named Definitions)
- You should now have one .table file in your destination folder for each definitions
- you can now delete the definition .cs files from your project (or save them out of your project)
- now you should update your project to the latest version of the framework
- you can now launch the project manager tool on XrmToolBox and select your project
- you should see the tables that were just generated in the UI
- for each of these tables, press the "Refresh attributes" button, the details should update and you should now see the full table with your selected attributes ticked on and your custom names.
- now if you rebuild your old project, you should have the exact same definition classes generated in your project

