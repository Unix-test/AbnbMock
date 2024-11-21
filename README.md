
# ABNB_MOCK

Demo request of Reservation System


## Installation

Docker Postgres DB

```bash
  docker pull postgres
  docker run --name reservoirDB -e POSTGRES_DB=reservoir -e POSTGRES_USER=admin  -e POSTGRES_PASSWORD=admin -p 5432:5432 -d postgres
```
    
## Usage/Examples

1/ Run dotnet ef migrations add --project Core/Core.csproj --startup-project AbnbMock.csproj --context Core.Helpers.ReservoirDbContext --configuration Debug --verbose Initial --output-dir Migrations

2/ Run api: "/authenticate" => username: admin, password: admin

3/ Run api "/room/add" to seeding sample data to db (data file is "room.data.json")


## Authors

- [@louisAndersen](https://github.com/Unix-test/AbnbMock)

