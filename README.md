# PokemonDB 

![PokemonDB](/Images/Logo.PNG)

PokemonDB is a web app created entirely by myself. The main purpose of this project was to learn React frontend development, as well as to better learn microservices and how to run everything purely through Docker containers.

The frontend/webpages were created using React via the create-react-app template. The frontend makes all data requests to an ASP.NET Core REST web API, which serves all of the Pokemon stats and images back to the frontend via JSON responses. The API retrieves the Pokemon stats from an MsSQL Server database, and retrieves the Pokemon images data from a MongoDb database. All data retrieved from the databases is temporarily cached in a Redis cache for better performance. Each service runs independently in a Docker container (ie. there are 5 containers: *frontend, backend API, MsSQL Server, Mongo, Redis*). 

![Screen recording gif](/Images/screen-recording.gif)

### Project Overview

**HOME PAGE** - Search for Pokemon using the search bar, or click on one of the links in the Quick Access list on the right side of the page. Hyperlinks are included to redirect to the Swagger UI endpoint if you wish to use to manually make backend API requests. Hyperlinks are also included to redirect to the healthcheck and WatchDog logger endpoints.

![Home page](/Images/Home.PNG)

**PROFILES PAGE** - Page through to view all Pokemon. Click on a Pokemon image to view that Pokemon's stats. Click on the heart icon to add the Pokemon to your favourites list.

![Profiles page](/Images/Profiles.PNG)

**STATS PAGE** - Displays the Pokemon's stats. Click on the compare button and search for another Pokemon to have side-by-side comparisons of their stats.

![Stats page](/Images/Stat.PNG)

![Stats comparison](/Images/Stats.PNG)

**FAVOURITES PAGE** - Same as the Profiles page but it only shows Pokemon that you have favourited. Favourite Pokemon are stored in the browser's local storage so this list persists even after the browser is closed.

![Favourites page](/Images/Favourites.PNG)

**ADVANCED SEARCH PAGE** - Make sophisticated queries for Pokemon. Add or remove filter rows and click the submit button to determine the intersection of all Pokemon that satisfy all conditions of the applied filters. A maximum of 5 filter rows can be applied. Click on one of the returned Pokemon hyperlinks to navigate to that Pokemon's stats page.

![Advanced Search page](/Images/AdvancedSearch.PNG)

### How To Run This Project

1. The easiest way is to run the Docker compose file located at the root of this repo. Ensure you have Docker installed on your computer. Download the [compose.yml](compose.yml) file, and in whichever folder the file is located, open a terminal and execute command `docker compose up -d`. Navigate to `http://localhost:55559` and you should see the React frontend running. Note that it may take around a minute for all of the Docker containers to fully start and begin communicating with each other before it begins displaying any Pokemon.

2. You can also run each Docker container manually by executing the following commands in a terminal:

- `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=StrongPassword@1" -p 55555:1433 --name mssql-pokemon-db --net pokemondb-network --health-cmd="/opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P 'StrongPassword@1' -Q 'SELECT 1' || exit 1" --health-start-period=15s -d conkythegreat/mssql-pokemon-db:latest`
- `docker run -p 55556:27017 --net pokemondb-network --health-cmd="echo 'db.runCommand(\"ping\").ok' | mongosh localhost:27017/test --quiet" --health-start-period=15s --name mongo-pokemon-db -d conkythegreat/mongo-pokemon-db:latest`
- `docker run -p 55557:6379 --name redis-pokemon-db --net pokemondb-network --health-cmd="redis-cli ping || exit 1" --health-start-period=15s -d redis:latest`
- `docker run -p 55558:80 --net pokemondb-network --name backend-pokemon-db -d conkythegreat/backend-pokemon-db:latest`
- `docker run -p 55559:80 --name frontend-pokemon-db --net pokemondb-network -d conkythegreat/frontend-pokemon-db:latest`
- *Note that you must follow the `--name` convention specified for each container. You must also assign external port 55558 to the backend and external port 55559 to the frontend. The external port assignments for all of the other containers can be omitted or customized.*

### Key Features of React Frontend:

- Implementation of React hooks including useState, useEffect, useRef, useNavigate, useParams, useContext (global state management).
- Asynchronous GET requests of backend API using `fetch`.
- Browser local storage saving of favourited Pokemon allows persistance of favourites list even after webpage is closed.

### Key Features of .NET Backend API:

- Utilizes Dapper to retrieve data from SQL database using stored procedures.
- All database services implemented using dependency injection.
- Custom middleware to handle exceptions and serve non-descriptive error messages to client.
- Healthcheck endpoint available for performing health checks on the backend as a whole, as well as the individual MsSQL/Mongo/Redis services.
- Uses WatchDog nuget package to perform all logging.
- Unit/middleware/integration tests performed using xUnit, Moq, AutoFixture and FluentAssertion libraries.

### MsSQL Server Database Details:

- A pre-seeded database container is supplied in the docker compose file.
- The database name within the MsSQL Server container is *PokemonDB*.
- The table name within the database is *PokemonStats*.
- The table data was created using the [CreatePokemonDb.sql](/MsSQL%20-%20Pokemon%20stats/CreatePokemonDB/CreatePokemonDB.sql) file. This file was created using an online CSV to SQL converter.
- The stored procedures are already created and ready for use within the table. They can be viewed in the [CreatePokemonDb folder](/MsSQL%20-%20Pokemon%20stats/CreatePokemonDB).
- Further details on how the container was pre-seeded and where these files are located inside of the Docker container can be found in the [Docker Instructions file](Docker%20Instructions.txt).

### MongoDb Database Details:

- A pre-seeded database container is supplied in the docker compose file.
- The database name within the MongoDb container is *PokemonImagesDb*.
- The collection name within the database is *PokemonImages*.
- The table data was created using the [mongoJsonDB.json](/Mongo%20-%20Pokemon%20images/mongoJsonDB/mongoJsonDB.json) file. This file was created using a C# script to convert all of the original images into a base64 string.
- Further details on how the container was pre-seeded and where these files are located inside of the Docker container can be found in the [Docker Instructions file](Docker%20Instructions.txt).

### Credits

- The Pokemon stats data was obtained from this [Kaggle csv file](https://www.kaggle.com/datasets/rounakbanik/pokemon). Note that some of the data has been modified to ensure consistency and operability with the backend.
- The Pokemon images were obtained from this [Kaggle dataset](https://www.kaggle.com/datasets/kvpratama/pokemon-images-dataset). All image data in the backend of this project is represented as a base64 encoded string.
- The online CSV to SQL tool used for creating the Pokemon stats SQL database can be found [here](https://www.convertcsv.com/csv-to-sql.htm).

### Misc. Comments

- SSL/HTTPS protocol is not supported, only plain HTTP is used. If the containers are running but you are unable to connect to any of the frontend pages or backend Swagger UI, check to make sure the url is using HTTP.
- The WatchDog logging package consistently logs random exceptions related to read/write locks. This is something to do with the package's default usage of LiteDB and I could not figure out how to solve the issue. In the future I would instead try to sink all of the WatchDog logs to a custom database instance, or more likely just implement my own logging solution that is much more lightweight. 
- A major improvement for future projects would be to create an Nginx container running inside the Docker network that handles the HTTP communications between the frontend and backend container. Currently the front/backend containers are communicating via exposed external ports on the localhost, which requires CORS policies to be in place within the backend.
