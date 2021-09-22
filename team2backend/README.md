## How to run in Docker from the commandline

Build in container
```
docker build -t aspnet_sandbox .
```

to run

```
docker run -d -p 8081:80 --name aspnet_sandbox_container aspnet_sandbox
```

to stop container
```
docker stop aspnet_sandbox_container
```

to remove container
```
docker rm aspnet_sandbox_container
```

## Deploy to heroku

1. Create heroku account
2. Create application
3. Make sure application works locally in Docker


Login to heroku
```
heroku login
heroku container:login
```

Push container
```
heroku container:push -a aspnet-sandbox web
```

Release the container
```
heroku container:release -a aspnet-sandbox web
```

##Apply Database migrations

Add migration
```
dotnet ef migrations add MigrationName
```

Update database
```
dotnet ef database update
```

Check migrations list
```
dotnet ef migrations list
```