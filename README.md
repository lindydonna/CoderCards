# CoderCards

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Flindydonna%2Fcodercards%2Fmaster%2Fazuredeploy.json" target="_blank">![Deploy to Azure](http://azuredeploy.net/deploybutton.png)</a>

## About the demo

* The function is triggered when a new .jpg file appears in the container `card-input`.

* Based on an input image, one of 4 card templates is chosen based on emotion

* The **filename** of the input image is used to draw the name and title on the card. 

* **Filename must be in the form `Name of person-Title of person.jpg`**

* A score based on the predominant emotion (e.g., anger, happiness) is drawn on the top

* The card is written to the output blob container `card-output`

## Running the demo

### Demo Setup

1. Fork the repo into your own GitHub

2. Ensure that you've authorized at least one Azure Web App on your subscription to connect to your GitHub account. To learn more, see [Continuous Deployment to Azure App Service](https://azure.microsoft.com/en-us/documentation/articles/app-service-continuous-deployment/).

3. Click the Deploy to Azure button above. 
  
  * Enter a sitename **with no dashes**. 
  
  * A storage account will be created with the same site name (and Storage does not allow dashes in account names).
  
  * Enter the API key from the Cognitive Services Emotion API (https://www.microsoft.com/cognitive-services/en-us/emotion-api)

4. Open the Function App you just deployed. Go to Function App settings -> Configure Continuous Integration. In the command bar, select **Disconnect**.

5. Close and reopen the Function App. Verify that you can edit code in CardGenerator -> Develop.

6. In [Azure Storage Explorer](http://storageexplorer.com/), navigate to the storage account with the same name as your Function App.
   
   * Create the blob container `card-input`
   
   * Output from the function will be in `card-output`, but you don't need to create this container explicitly. 

### Running the demo

1. Choose images that are **square** with a filename in the form `Name of person-Title of person.jpg`. The filename is parsed to produce text on the card.

2. Drop images into the `card-input` container. Once the function runs, you'll see generated cards in `card-output`.

### Notes

* The demo uses System.Drawing, which is NOT recommended for production apps. To learn more, see [Why you should not use System\.Drawing from ASP\.NET applications](http://www.asprangers.com/post/2012/03/23/Why-you-should-not-use-SystemDrawing-from-ASPNET-applications.aspx).

* Happy faces get a multiplier of 4, angry gets a multiplier of 2. I encourage you to tweak for maximum comedic effect!

### Talking points about Azure Functions

* The code is triggered off a new blob in a container. We automatically get a binding for both the byte array and the blob name

* The blob name is used to generate the text on the image

* The input binding is just a byte array, which makes it easy to manipulate with memory streams (no need to create new ones)

* Other binding types for C# are Stream, CloudBlockBlob, etc, which is very flexible.

* The output binding is just a stream that you just write to

* This is a very declarative model, details of binding are in a separate json file that can be edited manually or through the Integrate UX

* In general, functions will scale based on the amount of events in input. A real system would probably use something like Azure Queue or Service Bus triggers, in order to track messages more closely.