# SchnappsAndLiquor

Drinking game in the spirit (hehe) of Snakes and Ladders.

## Build

`git clone https://github.com/RAlbiez/SchnappsAndLiquor.git`

### Frontend

[Node.js](https://nodejs.org/en/) is a requirement for the frontend)

```
cd Frontend
npm install
npm run build
```

This creates a directory `Frontend/dist/saufen`, where the static files for the webserver are located.

### Backend

Best built by using [.NET SDK 5](https://dotnet.microsoft.com/download):

`dotnet publish SchnappsAndLiquor.sln --output ./build/ --runtime linux-x64`

You can then start the compiled application using the [.NET Runtime 5](https://dotnet.microsoft.com/download):

`dotnet ./build/SchnappsAndLiquor.dll`

The game can then be accessed at `http://localhost:8080`

### Develop

For development, you can run both run a Backend & Frontend "debug" process simultaneously.

```
cd Frontend
npm run start
```
and
```
cd SchnappsAndLiquor
dotnet run
```

The debug instance is then reachable at `http://localhost:4200`


## Deployment

For deployment, using Docker and a reverse Proxy makes things a little easier and more secure:

### Docker

You can use the Dockerfile to build your own image or use our docker-compose file using our official image from the Dockerhub.

Building your own Docker image:
```
git clone https://github.com/RAlbiez/SchnappsAndLiquor.git
cd SchnappsAndLiquor
docker build -t SchnappsAndLiquor .
docker run -dit --name SchnappsAndLiquor -p 8080:8080 SchnappsAndLiquor
- Visit http://<IP>:8080
```

Using docker-compose:
```
wget https://raw.githubusercontent.com/RAlbiez/SchnappsAndLiquor/master/docker-compose.yml
docker-compose up -d
- Visit http://<IP>:8080
```

### nginx

nginx can be used to securely proxy traffic to our application, if correctly setup with TLS certificates etc.
Use the following location block for your nginx config.

```
    location / {
        proxy_pass         http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection "upgrade";
    }
```

