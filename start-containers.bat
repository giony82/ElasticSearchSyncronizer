docker network create school

docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Letmein123" -p 1433:1433 --name db --network school -d mcr.microsoft.com/mssql/server:2017-CU8-ubuntu

docker run --name elk --network school -p 5601:5601 -p 9200:9200 -p 5044:5044 -it -d sebp/elk:7.11.2

docker run --name redis -p 6379:6379 --network school -d redis

docker build -f "%cd%\StudentService\StudentService.REST\Dockerfile" --force-rm -t studentservice "%cd%\StudentService"

docker build -f "%cd%\ElasticSearchSyncService\ElasticSearchSync.REST\Dockerfile" --force-rm -t elasticsyncservice "%cd%\ElasticSearchSyncService"

docker build -f "%cd%\HangfireService\Hangfire.REST\Dockerfile" --force-rm -t hangfireservice "%cd%\HangfireService"

docker stop studentservice
docker rm studentservice
docker run --name studentservice --env-file StudentService/StudentService.REST/variables.env --network school --publish 9601:80 -d studentservice
 
docker stop elasticsyncservice
docker rm elasticsyncservice
docker run --name elasticsyncservice --env-file ElasticSearchSyncService/ElasticSearchSync.REST/variables.env --network school --publish 9602:80 -d elasticsyncservice

docker stop hangfireservice
docker rm hangfireservice
docker run --name hangfireservice --env-file HangfireService/Hangfire.REST/variables.env --network school  --publish 9603:80 -d hangfireservice