## How to Run
1. Clone this repository `git clone git@github.com:tabakerov/testtask.git`
2. Run `docker-compose up --build`:
   - Go-lang category service: [http://localhost:3000/swagger/index.html](http://localhost:3000/swagger/index.html)
   - .NET product service: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

## About Data Consistency
While the product service verifies the existence of a given category during product creation and update, this setup does not guarantee strong data consistency. A category might be deleted immediately after the product service checks it. This is a known limitation and should be addressed in a production environment.

To mitigate communication issues between the product and category services, a standard Resilience Handler has been added to the HttpClient in the product service.

## Future Improvements
- Authorization/Authentication
- Improved data consistency - Consider using a queue mechanism or a distributed transaction manager to ensure data consistency between services.
- Request tracing - Implement distributed tracing to monitor and track requests across the microservices for better observability and debugging.
