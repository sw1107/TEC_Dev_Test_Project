# TEC Dev Test Project

## Description

For this project I have created a console app which when launched will download CSVs from the internet, parse and validate the data contained in the CSVs and then insert it into a relational database.

The site where the data is found:
https://twtransfer.energytransfer.com/ipost/TW/capacity/operationally-available

The data taken is the 'Final' cycle, and data is taken from the last three days.

To run the application, run the following commands fom the command line:

```
dotnet build
dotnet run
```
