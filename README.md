# Blackbaud-CRM-Fixer-Integration-Sample

While Blackbaud CRM does its absolute best to be your one-stop shop for all things related to accomplishing your mission, we realize that we simply cannot do everything. That is the reason why we made it easy to integrate third-party services with Infinity, enabling you to couple your CRM installation with other services. Here, we've provided a simple example of how an Infinity business process can call out to a web service to fetch and update currency exchange rates. In addition, we baked in some other concepts that we have been using to build our code, including:

* Usage of C# over VB
* Usage of .NET CLR over SQL-based data form specs
* Usage of different methods to interact with the CRM database
* Usage of a shared UIModel event handler for forms with identical or similar business logic

We've used these concepts in features and code that have shipped in various releases of CRM 4.0. This document will not only walk through the integration with Fixer, but will also discuss the advantages and disadvantages of these new approaches to Infinity development.

## Prerequisites

Before the features in this code sample can be seen and used in CRM, several basic prerequisites must be met. You will need:

* An installation of CRM
* Administrator-level access rights to said installation
* Access to said installation's database and virtual directory
* A specified and valid probe path in your installation's web config file
* An unlocked Multicurrency product module

To deploy the feature to your CRM instance, it is recommended that you take the following steps:

1. Use Visual Studio to compile both the Catalog and UIModel assemblies.  
Note that you may need to download and install the Newtonsoft Json.NET Nuget package for the Catalog assembly's compilation to succeed. This can be done easily and free of charge through Visual Studio.
2. Place these assemblies in the custom path of your installation's virtual directory.  
Note that this is the same place where you would deploy any custom CRM code.
3. Place the HTML files associated with the UIModel in your installation's browser/htmlforms folder, found in the virtual directory.  
Note that the HTML files must be in a folder named "currency," which itself resides in another folder named "custom," which is placed in the htmlforms folder. All the UIModel artifacts point to this location. If you prefer a different one, this can be changed in the source code. You will have to recompile and redeploy the package for the change to take effect.
4. Open CRM and access Administration > Application > Catalog browser.
5. Load the "Exchange Rate Refresh Process Package."

To use the functionality included in the package, navigate to Administration > Currency > Currency exchange rates, and then access the Exchange Rate Refresh Process tab.

## Fixer

For organizations that deal in multiple currencies, having up-to-date exchange rates is important. Currently Blackbaud CRM does not offer its own exchange rate service, but gives developers the ability to tie in to other products that offer it. One such service is [Fixer](https://fixer.io/). Fixer offers a variety of packages that contain endpoints allowing users to request historical and real-time currency exchange rates. This code sample was written using functionality that is available in the [Free](https://fixer.io/product) pricing tier.

In their [product documentation](https://fixer.io/documentation), Fixer outlines how developers interact with their service. It's simple! A standard HTTP web request is sent to the URL dedicated to serving the desired endpoint. The response is a single JSON object that can easily be parsed and manipulated by many programming languages, including C#. To do that we used Newtonsoft's free NuGet package, [Json.NET](https://www.newtonsoft.com/json). It can be installed directly using Visual Studio's NuGet package browser.

## Catalog assembly

There are a couple main points to highlight here. First, we will walk through the integration, as it is the centerpiece of this code sample. Second, we will look at the ideas behind choosing CLR data forms over SQL-based data forms and the inherent advantages and disadvantages.

### Integration

To solve the problem of refreshing stale currency exchange rates, we concluded that using a business process was the best approach. Business processes can be run in the background and are meant to deal with changing data in bulk. In the Catalog project you will notice the process, plus other supporting Infinity artifacts to enable users to add, edit, view, and run instances of the process.

You can find the code that allows CRM to interact with the service in the Fixer folder, specifically the `Service` class. As a brief note, the companion response objects were made to allow Json.NET to successfully parse the JSON objects returned by Fixer into .NET objects. Below is an example of the `Service` class calling an API endpoint, along with some referenced code presented for clarity.

``` csharp
public Response.SupportedSymbols GetSupportedCurrencies()
{
    // Get response from Fixer service
    string content = GetResponse(BASE_URL + "symbols?access_key=" + _accessKey);

    // Convert JSON to .NET object
    Response.SupportedSymbols symbols = JsonConvert.DeserializeObject<Response.SupportedSymbols>(content);

    // Return results
    if (symbols.Success)
    {
        return symbols;
    }
    else
    {
        throw new Exception(symbols.Error.Info);
    }
}

private static string GetResponse(string url)
{
    // Send request to Fixer service
    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

    // Read and parse response
    string content = string.Empty;
    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
    {
        content = sr.ReadToEnd();
    }

    return content;
}
```

Contacting the service is simple and clearly outlined in their documentation. The service listens for HTTP requests to be sent to dedicated URLs and always responds with a JSON object whether or not an error was encountered in processing the request. Using this class, we can then easily interact with the service at the web service layer, which deals primarily in .NET. The business process class, `ExchangeRateRefreshBusinessProcess`, through an instance of the `Service` class, can not only communicate with the service, but can loop through the returned exchange rates and insert them in to CRM.

### Choosing CLR

Almost immediately after opening the Catalog solution, it likely became evident that there are no SQL-based specs present, a fact revealed by the C\# companion class with nearly every spec. We implemented the features this way deliberately to provide an alternative way to code using the Infinity platform. There are distinct advantages to choosing CLR.

CLR reduces the need for database revisions. Unlike a SQL-based spec, CLR specs only hold the meta data that tells the Infinity platform about the fields on the form, how they should behave, and how they should be presented in CRM. Thus, for meta data changes, revisions are still necessary. However, should any aspect of the interaction with the database need to change, such as the SQL being used to perform it, that is housed entirely in .NET code. As such, changes to the SQL are deployed by pushing a new version of the DLL to your installation's virtual directory.

CLR allows for centralized and recyclable business logic. In addition to all database interaction being exclusively in .NET, business logic can be placed there as well. For example, you can now use .NET code to verify field values before adding them to the database rather than housing that in SQL. This allows the validation logic to be used by other forms or features that need to run the same checks. Should a change need to be made to this validation, it can be made in one place instead of in repeated code scattered among multiple SQL procedures.

CLR makes writing unit tests easier. Since all form logic is coded in .NET, writing unit tests against it becomes easy. The test code can call the production code directly, sending in fake data, removing the need for the automation to communicate with the database. This allows for entire test suites that run in milliseconds. By combining that with Visual Studio's code coverage analysis, these tests can give confidence that your code is working exactly as intended.

However, SQL-based specs also have some distinct advantages. For simpler interactions with the database, such as a simple query without any business logic, SQL-based specs are the more efficient choice. They carry less "overhead" than CLR specs with respect to the number of files associated with a single spec. Everything, including exception handling, can be defined within the meta data of the spec.

### Interacting with the CRM database

Depending on what our code is trying to do, we may have to interact with the database at different points in the workflow. We may need to pull relevant data for processing, validate configuration options, or fetch additional information requested by the user. At the web service layer, there are two primary ways to do this, each with their own set of advantages.

#### Using a SQL connection

The most straightforward way to accomplish this is to run a query directly on the CRM database. This is best used when the query is relatively simple and you know exactly which tables contain the information you need. In the below example, we access the table for the Exchange Rate Refresh business process to get information about a specific instance of the process.

``` csharp
private void LoadProperties()
{
    StringBuilder sql = new StringBuilder();
    sql.AppendLine("select");
    sql.AppendLine("  FIXERAPIACCESSKEY,");
    sql.AppendLine("  DATECODE,");
    sql.AppendLine("  HISTORICALDATE");
    sql.AppendLine("from dbo.USR_EXCHANGERATEREFRESHPROCESS");
    sql.AppendLine("where ID = @ID;");

    using (SqlConnection conn = this.RequestContext.OpenAppDBConnection(RequestContext.ConnectionContext.BusinessProcess))
    {
        using (SqlCommand cmd = conn.CreateCommand())
        {
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = this.ProcessCommandTimeout;
            cmd.CommandText = sql.ToString();

            cmd.Parameters.AddWithValue("@ID", this.ProcessContext.ParameterSetID);

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    _accessKey = reader.GetString(0);
                    _dateCode = (Common.DateCode)reader.GetByte(1);

                    if (_dateCode == Common.DateCode.Historical)
                    {
                        _date = reader.GetDateTime(2);
                    }
                }
                else
                {
                    throw new ServiceException("Could not retrieve process properties.");
                }
            }
        }
    }
}
```

One important point of emphasis should be made on the following line:

``` csharp
cmd.Parameters.AddWithValue("@ID", this.ProcessContext.ParameterSetID);
```

This line takes the value given to us by the Infinity platform that represents the business process' parameter set ID and inserts it in to the query in place of the `@ID` parameter. This is called "parameterizing" a SQL query. It is recommended best practice for protecting queries from [SQL injection](https://www.owasp.org/index.php/SQL_Injection), especially when the parameters come from user input. Always parameterize queries when using this method to interact with the CRM database.

#### Using the Web API

Another way to interact with the database is to go through the CRM Web API. This is best when dealing with more complex SQL or you would like to leverage the security features built in to CRM. In the above example, we did not need to take site security or constituent security into consideration. All security is enforced at the web service layer, so going directly to the database would bypass that. By using the Web API, you would get all this functionality for free! In the below example, we use the Currency Exchange Rate Add form to add a new rate.

``` csharp
// Create request
DataFormSaveRequest request = new DataFormSaveRequest()
{
    FormID = new Guid("9b16843e-c20a-4ddf-b31b-1d179380476e"), // CurrencyExchangeRate.Add.xml
    SecurityContext = new RequestSecurityContext()
    {
        SecurityFeatureID = new Guid("37e7492f-9a86-4029-b125-d133f330bf90"), // ExchangeRateRefresh.BusinessProcess.xml
        SecurityFeatureType = SecurityFeatureType.BusinessProcess
    },
    DataFormItem = new AppFx.XmlTypes.DataForms.DataFormItem()
};

// Fill out form
request.DataFormItem.SetValue("FROMCURRENCYID", baseCurrencyId);
request.DataFormItem.SetValue("TOCURRENCYID", currencies[r.Key].Id);
request.DataFormItem.SetValue("TYPECODE", 1); // Daily
request.DataFormItem.SetValue("RATE", r.Value);
request.DataFormItem.SetValue("ASOFDATETIME", date);
request.DataFormItem.SetValue("TIMEZONEENTRYID", timeZoneId);

// Save the form
ServiceMethods.DataFormSave(request, this.RequestContext);
```

Most web service API endpoints operate in three phases:

1. Request  
The caller must fill out a request object, detailing the operation you want the web server to perform. In this case, we used a `DataFormSaveRequest` to tell the web server we wanted to call the Save portion of a data form feature.
2. Process  
Calling the `ServiceMethod` corresponding to the requested operation invokes the request processor on the web server. This is where the request is carried out, along with the enforcement of all security and business logic rules.
3. Reply  
As the request is being performed, a reply object is built. This reply is then returned to the caller so they can check the status of the request and/or extract information generated by the request for further processing. In this case, we did not capture the `DataFormSaveReply` because saving the form is the last step in our workflow.

This method can be used to invoke features that Blackbaud has written and shipped with CRM as well as features that you have built and loaded in to your installation!

## UIModel assembly

No catalog would be complete without an accompanying UIModel. In this code sample, we've included some ideas that we have been using to enhance code reusability and make it easier to write effective automation against it. We achieve that using the centralized event handler class `ExchangeRateRefreshProcessAddEditHandler`.

Upon inspection of the UIModel web server event handler classes for the Exchange Rate Refresh Process add and edit forms, it can be seen that the logic to handle server-side event processing has been shipped out to a separate class. In fact, the classes for the individual forms do not perform any event handling. We were able to do this because the forms are so similar that they can both be serviced by the same logic. Doing this provides the following extra benefits:

* Ability to reuse code  
Moving the code to support these forms into one place makes it faster and easier to modify their logic. Whether it's a bug fix or a feature update, both forms instantly benefit from a single code change.
* Ability to automate  
Since the logic is in a separate class, it's possible to test the event handling on the forms without needing an instance of those forms on a CRM web server. Additionally, any associated prerequisite data in the database is unnecessary. In a test, an instance of the handler class can be made using generic `Blackbaud.AppFx.UIModeling.Core.UIField`s. In this case, a `ValueListField` and a `DateField` can be used. The test can then make a change to one of those fields to trigger the appropriate event handler and then validate the results by looking at the field properties.

We encourage you to get the code, compile it, and load it in to your test environment to see it in action!