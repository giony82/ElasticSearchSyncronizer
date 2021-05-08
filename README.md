# Elastic Search Syncronizer

Syncronizes data in elastic search in batches.

## Arhitecture

TODO

## Technology stack

* MS ASP .NET Core & EF Core
* ELK
* Redis (no paid third party libraries needed)
* Hangfire
* Docker containers


## Solution 1 - via application layer

Each time an entity changes, the app layer triggers specific events which are translated in new entries in a Redis sorted set. From here, via cronjobs, a dedicated sync service is pulling data from Redis and then builds the necessary structure that must be pushed to Elastic Search. The replication is done in batches:

### Components [diagram](https://lucid.app/documents/view/6ca14e71-e916-4dda-ba4c-4ee699b25885):

![image](https://user-images.githubusercontent.com/16101625/117529567-e1289380-afe0-11eb-920d-34a63496da4a.png)

### Sequence diagram

![image](https://user-images.githubusercontent.com/16101625/117530148-3ca85080-afe4-11eb-8629-3bd7a3896493.png)

## Solution 2 - via SQL changed records notification mechanism

TODO

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

