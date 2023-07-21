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
$ table: users created
$ # INSERT INTO users VALUES (1, 'andrewjhopkins');
$ Values (1, andrewjhopkins) inserted into table: users
$ # SELECT id, name FROM users;
$ | id | name |
  ====================
  | 1 |  andrewjhopkins |
  ok
```

#### TODO:
Short term
- [ ] Support asterisks in Select
- [ ] Memory background unit tests
- [ ] Full scenario tests
- [x] More informative response for create and insert
- [x] On error. Return syntax error location
- [ ] General code cleanup

Long term
- [ ] WHERE filters
- [ ] Indexing
- [ ] UPDATE statements


#### resources
- [/r/databasedevelopment](https://www.reddit.com/r/databasedevelopment)
- [Phil Eaton's blog posts](https://notes.eatonphil.com/tags/databases.html)
