call stop-swarm-services.bat

docker swarm leave

docker swarm init

docker network create -d overlay nginx-net

docker build -f "%cd%\StudentService\StudentService.REST\Dockerfile" --force-rm -t studentservice "%cd%\StudentService"

docker build -f "%cd%\ElasticSearchSyncService\ElasticSearchSync.REST\Dockerfile" --force-rm -t elasticsyncservice "%cd%\ElasticSearchSyncService"

docker build -f "%cd%\HangfireService\Hangfire.REST\Dockerfile" --force-rm -t hangfireservice "%cd%\HangfireService"

docker service create -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Letmein123" -p 1433:1433 --name db --network nginx-net -d mcr.microsoft.com/mssql/server:2017-CU8-ubuntu

docker service create --name elk --network nginx-net -p 5601:5601 -p 9200:9200 -p 5044:5044 -d sebp/elk

docker service create --name redis -p 6379:6379 --network nginx-net -d redis

docker service create --name studentservice --env-file StudentService/StudentService.REST/variables.env --replicas=2 --network nginx-net --publish 9601:80 -d studentservice

docker service create --name elasticsyncservice --env-file ElasticSearchSyncService/ElasticSearchSync.REST/variables.env --replicas=2 --network nginx-net --publish 9602:80 -d elasticsyncservice

docker service create --name hangfireservice --env-file HangfireService/Hangfire.REST/variables.env --replicas=1 --network nginx-net --publish 9603:80 -d hangfireservice