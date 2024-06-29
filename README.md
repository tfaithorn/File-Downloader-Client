# File-Downloader-Application
Simple application to download files using a spreadsheet that contains: a URL, filename, relative folder path, countryId, post data.
The application is meant to be used in combination with the World Manager platform.
Check out `downloaderclient.cs` to see how it works.

##
- Invalid characters are stripped from the folder & file names.
- If the folders in the relative path does not exist the folder will be created.

##
There is code related to generating a screenshot with a webview, but this functionality was scapped.
It works but you'll need to intialize the webview and bind it to a button.
