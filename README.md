# File-Downloader-Application
Simple application to download files using a spreadsheet that contains: a URL, filename, relative folder path, countryId, post data.
The application is meant to be used in combination with the World Manager platform.

##
Notes:
- Invalid characters are stripped from the file & folder names.
- If the folders in the relative path do not exist they will be created automatically.

##
There is code related to generating a screenshot with a webview, but this functionality was scapped.
It works but you'll need to intialize the webview and bind it to a button.
