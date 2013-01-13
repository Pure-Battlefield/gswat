#The Pure Game Server Web Administration Tool
***
The intent of this tool is to make administering servers a breeze without ever leaving the comfort of your favorite browser, no matter what kind of device you're using. Enjoy!

## Setup

* Install Visual Studio - You can grab the free [VS Web Express version here](http://www.microsoft.com/visualstudio/eng/downloads#d-express-web)
* Fork our repo at [GSWAT](https://github.com/Pure-Battlefield/gswat)
* Open the project by clicking the `gswat.sln` file in the repo
* In the `Solution Explorer`, find the `WebFrontend.Azure` item, right click it, and select `Set as StartUp Project`

## Run Locally

* Make sure that the project is set to `Debug` mode, you can find this at the top of the toolbar
* Click `Debug` -> `Start Debugging`, hit `F5`, or click the green arrow icon in the top toolbar

## Run on Windows Azure

If you have a [Windows Azure](windowsazure.com) account, you can use it to put a stage version of GSWAT to test.

###Steps
####Publish to Azure
* Open the VS Project
* Click on `Build` -> `Publish to Windows Azure`
* Sign in to your Azure Account by following the prompts
* Choose a `Cloud Service`, `Environment` (choosing Production is easier)
* Set the Build Configuration to `Release` if you don't want extra diagnostic logging
* Select the `Cloud` profile
* Hit `Next` and then `Publish`

Once the process completes, you can check your [Windows Azure Dashboard](https://manage.windowsazure.com/#Workspace/All/dashboard) for the view link, you'll need to configure the `Storage` first though

####Configure Sotrage
Once you've got it published, go to your [Windows Azure Storage](https://manage.windowsazure.com/#Workspace/StorageExtension/storage), and follow these steps:
* Select the storage instance and click on `Manage Keys` at the bottom of the window
* Copy the first two fields into the following string: DefaultEndpointsProtocol=https;AccountName=`STORAGE ACCOUNT NAME`;AccountKey=`PRIMARY ACCESS KEY`
* Under `WebFrontend.Azure`, open the `Roles` folder, and open the `WebFrontend` file
* Click `Settings`
* Select the `Cloud` configuration option
* Paste the string from earlier into the `Value` column of the `StorageConnectionString` and `Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString` and then re-publish

You can also update these settings directly in your `Windows Azure Cloud Instance` by selecting the instance, going to `Configure`, and pasting the string into the `webfrontend` `settings` field and hitting `Save`

## Technologies

#####Server Platform
* Windows Azure
 
#####Backend Code
* C# .NET4.5 MVC3 on the backend

#####Frontend Code
* Backbone
* Underscore
* jQuery
* LESS
* ICanHaz (modified)
* Mustache
* Metro UI CSS
* YepNope
