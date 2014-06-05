Getting Started with Smarter Web Deploy
==================

Getting started is quick and easy provided you already have [Web Deploy](http://www.iis.net/downloads/microsoft/web-deploy) configured on your server (follow Microsoft's documentation to install it).

You may want to watch the quick intro video to see the moving parts before you try it yourself.

    [link to video]

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

The user account running your deployment site must have read / write permissions to the folders holding your live site. Put a sample ASP.NET website in each of these and make sure they load correctly in a browser. This is just basic hosting, nothing about smarter web deploy is tested yet.

**Part 3 - Publishing to your live site**

Finally, configure Visual Studio to publish to **the deployment site** not the live site. The idea is to 

1. Publish your actual production code to the deployment site.
2. Launch a browser to a special URL on the deployment site.
3. This triggers smart deploy.
4. It will locally deploy to the live site and test the deployment.


