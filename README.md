# csharp_sql

My attempt at a simple SQL implementation from scratch in C# to better understand compilers and database development


## Use Example
```
$ git clone https://github.com/andrewjhopkins/csharp_sql.git
$ cd csharp_sql
$ dotnet run --project csharp_sql
$ Welcome to C# sql
$ Press ctrl + c to quit
$ # CREATE TABLE users (id INT, name TEXT);
$ ok, table created
$ # INSERT INTO users (1, 'andrewjhopkins');
$ ok, new values inserted
$ # SELECT id, name FROM users;
$ | id | name |
  ====================
  | 1 |  andrewjhopkins |
  ok
```

#### TODO:
Short term
- [ ] Support asterisks in Select
- [ ] More informative response for create and insert
- [ ] On error. Return syntax error location
- [ ] General code cleanup

Long term
- [ ] WHERE filters
- [ ] Indexing
- [ ] UPDATE statements


#### resources
- [/r/databasedevelopment](https://www.reddit.com/r/databasedevelopment)
- [Phil Eaton's blog posts](https://notes.eatonphil.com/tags/databases.html)
