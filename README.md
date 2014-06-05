Smarter Web Deploy
==================

A library to extend Microsoft's Web Deploy feature in Visual Studio to create reliable and fast deploys on production sites while online without adversely affecting users.

**Quick start**: Check out our [Getting Started Guide](https://github.com/mikeckennedy/smarter_web_deploy/blob/master/GettingStarted.md).

Rather than uploading directly into a live site (which can break during slow uploads or requests in the middle of a transfer), a second non public site is used to publish into. Then smart deploy compares the files on the server using hashs rather than time stamps. If the site has changes, it will be gracefully taken offline, files moved over, and then it will be brought back online.

Sample output
==================

    0.000 seconds: Smart deploy starting at 6/5/2014 9:30:29 AM
    0.000 seconds: Compare source and destination files (application is still online during this step) ...
    0.379 seconds: Setting application to offline state (http://localhost:26555/)
    0.381 seconds: Waiting 2 seconds for pending requests (if any) to complete ...
    2.363 seconds: Copying files to destination site ...
    2.363 seconds: Running 3 file operations...
    2.366 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SmarterWebDeploy.pdb
    2.368 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SourceWebsiteForTesting.dll
    2.371 seconds: File copied: D:\Smarter_web_DepLoy\DestSite\bin\SourceWebsiteForTesting.pdb
    2.381 seconds: Copied 3 files successfully.
    2.382 seconds: Waiting for final changes to be detected ...
    2.883 seconds: Removing application offline status ...
    2.985 seconds: Requesting page at http://localhost:26555/, starting site and verifying dpeloy ...
    7.224 seconds: Site started SUCCESSFULLY, 4,121 characters returned.
    7.224 seconds: Smart deploy successful. Visit this link to view the page: http://localhost:26555/
    Done.
    

If there are no real changes (file hashes are all identical) the site will never be taken offline.

Sample output
==================

    0.000 seconds: Smart deploy starting at 6/5/2014 9:35:16 AM
    0.000 seconds: Compare source and destination files (application is still online during this step) ...
    0.379 seconds: There are no file changes to deploy. Deploy cancelled and application is still online (unchanged).
    Done.
