smarter_web_deploy
==================

A library to extend Microsoft's Web Deploy feature in Visual Studio to create reliable and fast deploys on production sites while online without adversely affecting users.

Details soon.


Sample output
==================

    0.000 seconds: Smart deploy starting at 6/5/2014 9:30:29 AM
    0.000 seconds: Compare source and destination files (application is still online during this step) ...
    0.379 seconds: Setting application to offline state (http://localhost:26555/)
    0.381 seconds: Waiting 2 seconds for pending requests (if any) to complete ...
    2.363 seconds: Copying files to destination site ...
    2.363 seconds: Running 6 file operations...
    2.366 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SmarterWebDeploy.pdb
    2.368 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SourceWebsiteForTesting.dll
    2.371 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SourceWebsiteForTesting.pdb
    2.375 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\obj\Debug\1-SourceWebsiteForTesting.csprojResolveAssemblyReference.cache
    2.379 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\obj\Debug\SourceWebsiteForTesting.dll
    2.380 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\obj\Debug\SourceWebsiteForTesting.pdb
    2.381 seconds: Copied 6 files successfully.
    2.382 seconds: Waiting for final changes to be detected ...
    2.883 seconds: Removing application offline status ...
    2.985 seconds: Requesting page at http://localhost:26555/, starting site and verifying dpeloy ...
    7.224 seconds: Site started SUCCESSFULLY, 4,121 characters returned.
    7.224 seconds: Smart deploy successful. Visit this link to view the page: http://localhost:26555/
    Done.
    
