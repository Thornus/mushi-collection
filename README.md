# Mushi Collection
## Description
Mushi Collection is a demo game, meaning it has been made for demonstrational purposes.

Mushi Collection is made with [Unity](https://en.wikipedia.org/wiki/Unity_(game_engine)) and combines C# and PHP to interact with a remote database and perform actions such as login, registration and items' data retrieval.

## Why use PHP to interact with the database?
The answer is simple: C# files are local and if they contained code that accesses the database directly, players would have the possibility of cheating by modifying the files.

Instead, C# files send an HTTP request to the URL of the specific PHP file, so that it can handle the connection to the database and access data opportunely.

## Where can I find the code?
The C# files can be found in [mushi-collection/Assets/Scripts](https://github.com/Thornus/mushi-collection/tree/master/Assets/Scripts).

The PHP files can be found in [mushi-collection/PHP](https://github.com/Thornus/mushi-collection/tree/master/PHP).

## Instructions to build and play the game
1. Upload the PHP files to a server (or use localhost) and modify the URLs accordingly in the C# files that make the HTTP requests.
2. Create a database accordingly with the MySQL queries that are in the PHP files (or modify the queries to suit you best).
3. Import the project in Unity and build. The game has controls set for computers only, so it's best to choose PC or Mac as platform.

An already built version of the game is available zipped in [mushi-collection/Build](https://github.com/Thornus/mushi-collection/tree/master/Build).
