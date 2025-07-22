# AI as User Interface

This project aims to showcase a new type of user interface for line of business applications.
It will allow users to query data in their application using natural language and lower the barrier of accessing and analyzing data.
This is similar to how most applications let users export data as excel files to do further analysis on their own.

## Demo
[https://aiui.azurewebsites.net](https://aiui.azurewebsites.net/)

[Demo video](https://github.com/ctyar/Aiui/assets/1432648/b97d3bd2-f5a4-4c08-a17c-80904411cb07)

### Privacy first
This project is designed to be a mediator between your system and AI. Which means that it does not share any user data with AI.
The only data that is sent to AI is the user query and the database schema.

### What is the database schema?
This sample is using The <a href="https://github.com/microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs">Northwind database</a>.
It contains sales data for a fictitious company called “Northwind Traders” which imports and exports specialty foods from around the world.
You can check the overview of the schema <a href="~/img/schema.png">here</a>.

### Has the AI been trained specifically on this database?
No, the AI will receive information on the database schema at runtime.

### Will the AI be able to access the data in the database?
No, The AI will generate SQL queries which the library will execute.

### What if the user tries to access or execute malicious queries?
You should use a read-only database user with minimum access to tables that the user needs.

### What if the user tries to get information about other users?
You should not share conversations between users.

## Build
[Install](https://get.dot.net) the [required](global.json) .NET SDK.

Run:
```
$ dotnet build
```
