## [.Net Core微服务入门纪录](https://www.cnblogs.com/xhznl/p/13259036.html)

```bash
docker build -t orderapi -f ./src/DemoOrderAPI/Dockerfile .

docker run -d -p 5001:80 --name orderapi01 orderapi --ConsulSetting:ServicePort="5001"
docker run -d -p 5002:80 --name orderapi02 orderapi --ConsulSetting:ServicePort="5002"

docker build -t projectapi -f ./src/DemoProjectAPI/Dockerfile .

docker run -d -p 5011:80 --name projectapi01 projectapi --ConsulSetting:ServicePort="5011"
docker run -d -p 5012:80 --name projectapi02 projectapi --ConsulSetting:ServicePort="5012"


dotnet DemoAPIGateway.dll --urls="http://*:9070

docker-compose build
docker-compose up -d
docker-compose down

Add-Migration InitialCreate
Update-Database
```
## 踩坑经验
```bash
cap不要升级最新版本，和Pomelo.EntityFrameworkCore.Mysql现在不兼容，使用3.0.4版本即可

identityserver4鉴权服务，这个服务是容器内外部都需要访问的（容器内部ids4发现文档等接口的调用，外部浏览器访问）
host.docker.internal

网关swagger也需要http://host.docker.internal:9070/connect/token 从内部授权去查看
postman客户端授权 需要application/x-www-form-urlencodedh
参数是x-www-form-urlencoded格式的
```
