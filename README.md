# Elastic Search Syncronizer

Syncronizes data in elastic search in batches. Suitable for applications where data changes quite often and search queries should respond fast even for large amount of data.

## Arhitecture

The replication strategy is to update entities in ES in batches and not "stress" ES each time something changes in the app. Redis is used as the central point of syncronization so that the services can be scaled. This is accomplies with sorted sets, without the need of paid Redis third-party libraries.

Two solutions are considered:
 - The app layer is aware of the need to syncronize data in another system. This suits best new projects where ES is considered from the beginning. 
 - The app layer is not aware of the need to syncronize data in another system. This might be a good solution for old projects that are hard and risky to change, but ES is needed for fast search queries. 

## Technology stack

* MS ASP .NET Core & EF Core
* ELK
* Redis (no paid third party libraries needed)
* Hangfire
* Docker containers

## Solution 1 - via application layer

Each time an entity changes, the app layer triggers specific events which are translated in new entries in a Redis sorted set. From here, via cronjobs, a dedicated sync service is pulling data from Redis and then builds the necessary structure that must be pushed to Elastic Search. The replication is done in batches:

### Components [diagram](https://lucid.app/documents/view/6ca14e71-e916-4dda-ba4c-4ee699b25885):

![image](https://user-images.githubusercontent.com/16101625/117539369-fdddbf00-b012-11eb-96ae-d961a1871bb6.png)

### Sequence [diagram](https://lucid.app/documents/view/fa2f3abb-12f8-4053-b9d8-441ea85e93fc)

![image](https://user-images.githubusercontent.com/16101625/117530148-3ca85080-afe4-11eb-8629-3bd7a3896493.png)

## Solution 2 - via SQL change record notification mechanism (built in MS SQL)

Instead modifying the application layer (StudentService in this case), another service could run and listen for record change notifications (SqlChangeTrackerService)

In order to test this solution, edit the [environments.env](https://github.com/giony82/ElasticSearchSyncronizer/blob/master/StudentService/StudentService.REST/variables.env) file from the StudentService and disable the Elastic Search synchronization within this service.

Components [diagram](https://lucid.app/documents/view/769a2017-12b8-4a01-8363-9ef6f532c9ac)

![image](https://user-images.githubusercontent.com/16101625/117539242-798b3c00-b012-11eb-9806-9e9686932ffe.png)

## Redis sorted sets and scalability

Both solutions are using the producer consumer pattern. This is accomplished using a sorted Redis set where IDs are acumulating via the StudenService or SqlChangeTrackerService. **Note**:_If an item is already added to a sorted set, then it's not added twice_.

Tipically, a sorted set contains pairs of {value, score}. The score will represent in our case the number of retries. If an item fails to be processed, then it's pushed back to Redis but with the score incremented (number of retries). This allows us to process an item for a limited/configured number of times. 

The consumer (ElasticSearchSyncService) is pulling values(IDs) from Redis in a transactional way and in small batches (eg 10 and can be configured). 

The solution can be scaled for both producer and consumer, without the risk to process an item twice, or miss to process. 

## Run containers - no replicas

Run start-containers.bat

Wait for the containers to build and start

Import the Kibana dashboard  in [http://localhost:5601/app/management/kibana/objects](http://localhost:5601/app/management/kibana/objects) by clicking import and selecting the **kibana-dashboards.ndjson** from this branch.

Browse the StudentService's API [here](http://localhost:9601/swagger/index.html) and POST students.

The data and/or the logs can be visualized in the Discover section of Kibana [http://localhost:5601/app/discover](http://localhost:5601/app/discover)

The dashboard can be seen here: [http://localhost:5601/app/dashboards#/list](http://localhost:5601/app/dashboards#/list)

## Run in swarm - with replicas

First, make sure to delete any container created by the above chapter.

Run start-swarm.bat from this branch.

Start some tests with Locust (see the section bellow) and verify the performance dashboard in Kibana. 

## Locust 

Locust it's a convenient way to simulate multiple users/clients accesing your API, with some nice charts and statistics being generated at the end. 

Steps to use locust:

- Install locust by following the steps described here: [https://docs.locust.io/en/stable/installation.html](https://docs.locust.io/en/stable/installation.html)
- Run startlocust.bat from this branch
- Go to [http://localhost:8089/](http://localhost:8089/) 
  - Enter the number of users to simulate 
  - Click start

