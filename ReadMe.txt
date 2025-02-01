GameService.sql 
Находтся в папке проекта GameService

ConnectionString
Находится в самом классе AppDbContext (GameService/Models/Db/AppDbContext.cs)

WebApi
Пример запроса транзакции
POST
https://localhost:8080/api/v1/transactions?bet=5000&payeeid=1&senderid=3

Пример создания матча со ставкой
POST
https://localhost:8080/api/v1/games?bet=440 