Getting Started with Smarter Web Deploy
==================

Getting started is quick and easy provided you already have [Web Deploy](http://www.iis.net/downloads/microsoft/web-deploy) configured on your server (follow Microsoft's documentation to install it).

**Part 1 - Configuring your app**

There are a few steps required to configure your app. 

1. Add a reference to `SmarterWebDeploy.dll`.
1. Add the required fields to your web.config file (see below)

**web.config settings**

    <?xml version="1.0" encoding="utf-8"?>
        <configuration>
           <appSettings>
	          <!-- fromBaseFolder: Full path to temp deployment site. -->
              <add key="fromBaseFolder" value="D:\Programming\GitHub\smarter_web_deploy\src\SourceWebsiteForTesting\" />
              <!-- toBaseFolder: Full path to actual live site. -->
              <add key="toBaseFolder" value="D:\Programming\GitHub\smarter_web_deploy\src\DestinationWebsiteForTesting\" />
              <!-- appOfflineSourceFile: Relative path to offline html file in deployment site. -->
              <!-- This is the HTML that will be shown while your site is offline (rather than crashing during upgrade). -->
              <add key="appOfflineSourceFile" value="xappoffline.htm" />
              <!-- redirectUrl: Home page of live site. -->
              <add key="redirectUrl" value="http://localhost:26555/" />
              <!-- verificationKey: Secret key passed to start deployment via URL - keep private can be any URL friendly characters except #, /, :, etc. -->
              <!-- change this value, do not leave it the same as below -->
              <add key="verificationKey" value="547bbf02-7469-4606-8960-21e3e5af74bd" />
          </appSettings>
          <!-- ... -->
     </configuration>

**Part 2 - Configuring your server**

The server must be configured with web deploy (see above). Let's assume we are seting up for smarter web deploy on my blog: [http://blog.michaelckennedy.net](http://blog.michaelckennedy.net).

You will need to create two websites in IIS. 

* Your live site. E.g. http://blog.michaelckennedy.net
* A non-public deployment site. E.g. http://deployblog.michaelckennedy.net

The user account running your deployment site must have read / write permissions to the folders holding your live site. Put a sample ASP.NET website in each of these and make sure they load correctly in a browser. This is just basic hosting. Nothing about smarter web deploy is tested yet.

**Part 3 - Publishing to your live site**

Finally, configure Visual Studio to publish to **the deployment site** not the live site. The idea is to 

1. Publish your actual production code to the deployment site.
2. Launch a browser to a special URL on the deployment site.
3. This triggers smart deploy.
4. It will locally deploy to the live site and test the deployment.

Start by right-clicking your production web app in Visual Studio's solution explorer. Choose "Publish...".

Create a new custom profile (if you don't have one already).

In the connection settings, enter the following:

1. Server: Your server hosting both sites.
2. Site name: Your **deployment** site.
3. User / password: Your publish user credentials.
4. The URL to the action /deploy/complete/{securitykey} (e.g. `http://deployblog.michaelckennedy.net/deploy/complete/547bbf02-7469-4606-8960-21e3e5af74bd`)

Here is a screenshot of something along these lines:

![Screen shot](https://raw.githubusercontent.com/mikeckennedy/smarter_web_deploy/master/docs/screenshots/WebPublishConnection.png)

Check that everything is setup by validating the connection. 

Click 'Publish' to publish your site. It should load the deployment site which then locally runs the deploy to produciton on the server.

You should see output similar to this:

Sample output with changes

    0.000 seconds: Smart deploy starting at 6/5/2014 9:30:29 AM
    0.000 seconds: Compare source and destination files (application is still online during this step) ...
    0.379 seconds: Setting application to offline state (http://localhost:26555/)
    0.381 seconds: Waiting 2 seconds for pending requests (if any) to complete ...
    2.363 seconds: Copying files to destination site ...
    2.363 seconds: Running 3 file operations...
    2.366 seconds: File copied: D:\DestSite\bin\site.web.dll
    2.368 seconds: File copied: D:\DestSite\bin\site.data.dll
    2.371 seconds: File copied: D:\DestSite\views\home\index.cshtml
    2.381 seconds: Copied 3 files successfully.
    2.382 seconds: Waiting for final changes to be detected ...
    2.883 seconds: Removing application offline status ...
    2.985 seconds: Requesting page at http://localhost:26555/, starting site and verifying deploy ...
    7.224 seconds: Site started SUCCESSFULLY, 4,121 characters returned.
    7.224 seconds: Smart deploy successful. Visit this link to view the page: http://localhost:26555/
    Done.

Welcome to a better way to publish your website to IIS!

-------------------------------------
Guide written by [@mkennedy](https://twitter.com/mkennedy). Contributions to the project are welcome.





