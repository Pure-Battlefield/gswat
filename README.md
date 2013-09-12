#The Pure Game Server Web Administration Tool
***
The intent of this tool is to make administering servers a breeze without ever leaving the comfort of your favorite browser, no matter what kind of device you're using. Enjoy!

## Setup

If you do NOT have any version of Visual Studio 2012 currently installed on your computer,  you can grab the free [VS Web Express version here](http://www.microsoft.com/visualstudio/eng/downloads#d-express-web), which is prepackaged with all the required Windows Azure components.

* Fork our repo at [GSWAT](https://github.com/Pure-Battlefield/gswat). Choose the `develop` branch if you plan on modifying the project.
* Open the project in VS by clicking the `gswat.sln` file in the repo.
* If prompted to, convert the project to target Windows Azure Tools 2.1.
* In the `Solution Explorer`, find the `WebFrontend.Azure` item, right click it, and select `Set as StartUp Project`

If your version of Visual Studio 2012 is having trouble opening the Windows Azure files in the project, you will need to install some additional components.

* Download and launch the [Microsoft Web Platform Installer](http://www.microsoft.com/web/downloads/platform.aspx "MS Web Platform Installer").
* In the search box, type `azure`. Find and add the following:
	* Windows Azure Libraries for .NET (VS 2012) - 2.1
	* Windows Azure Libraries for .NET - 2.1
	* Windows Azure SDK - 2.1
* Install the components and try opening the project again.

## Run Locally

* Visual Studio should be running with administrative privileges, or the Windows Azure emulator(s) may have trouble launching. 
* Make sure that the project is set to `Debug` mode, you can find this at the top of the toolbar.
* Click `Debug` -> `Start Debugging`, hit `F5`, or click the green arrow icon in the top toolbar. GSWAT will open automatically (as a new tab) in the selected browser.

You may need to clear your browser's cache before you can see changes.

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

####Configure Storage
Once you've got it published, go to your [Windows Azure Storage](https://manage.windowsazure.com/#Workspace/StorageExtension/storage), and follow these steps:
* Select the storage instance and click on `Manage Keys` at the bottom of the window
* Copy the first two fields into the following string: DefaultEndpointsProtocol=https;AccountName=`STORAGE ACCOUNT NAME`;AccountKey=`PRIMARY ACCESS KEY`
* Go to `Windows Azure Cloud` and select your app instance
* Go to the `Configure` tab
* Paste the string into the `webfrontend` `settings` field and hit `Save`

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
* Moment JS
* Twitter Bootstrap
* YepNope
