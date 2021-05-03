docker network create school

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Letmein123" -p 1433:1433 --name db --network school -d mcr.microsoft.com/mssql/server:2017-CU8-ubuntu

docker run --name elk --network school -p 5601:5601 -p 9200:9200 -p 5044:5044 -it -d sebp/elk

docker run --name redis -p 6379:6379 --network school -d redis

docker build -f "D:\Backup\Work\Didactic\StudentService\StudentService.REST\Dockerfile" --force-rm -t studentservice "D:\Backup\Work\Didactic\StudentService"

docker build -f "d:\Backup\Work\Didactic\ElasticSearchSyncService\ElasticSearchSync.REST\Dockerfile" --force-rm -t elasticsyncservice "D:\Backup\Work\Didactic\ElasticSearchSyncService"

docker build -f "d:\Backup\Work\Didactic\Hangfire\Hangfire.REST\Dockerfile" --force-rm -t hangfireservice "D:\Backup\Work\Didactic\Hangfire"

docker run --name studentservice --env-file StudentService/StudentService.REST/variables.env --network school --publish 9601:80 -d studentservice
 
docker run --name elasticsyncservice --env-file ElasticSearchSyncService/ElasticSearchSync.REST/variables.env --network school --publish 9602:80 -d elasticsyncservice

docker run --name hangfireservice --env-file Hangfire/Hangfire.REST/variables.env --network school  --publish 9603:80 -d hangfireservice