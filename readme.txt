this architecture do you dont need add to www folder the files js, css and index.html all time build your project angular2, this do it automutclly, 
and include this file in project before publish.

inatall:

add to root folder new folder called "angular" and in this folder create your angular project.

add refrence to GetAngular2File.dll.

update global.asax file:
add: "using GetAngular2File;" ,
add to method Application_Start this line: "CreateIndexFile.Play();".
like this: 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using GetAngular2File;

namespace TryAngular
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            CreateIndexFile.Play();
        }
    }
}

that it! hope its help :)