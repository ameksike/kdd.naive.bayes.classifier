# Priberam Challenge
The task is to write an HTTP REST web service which classifies news documents. At any given point in time, the service can receive: 
  (1) a training document already classified with a topic; 
  (2) a test document, to which it has to return a prediction for the topic of that document. 
  
The web service should run "forever" and be ready to receive either request (1) or (2) at any given time. The topic to be predicted is 1 out of 5: "business", "entertainment", "politics", "sports" or "tech".

## Task Description
For the topic preditiction part, a classifier algorithms should be implemented. We suggest a Naive Bayes Classifier, which is detailed further in this document.

## Install steps
- dotnet build
- dotnet ef database update --project Classifier
- dotnet run --project Classifier
- http://localhost:8080

## Develop steps
- dotnet --version
- dotnet new sln -o priberam
- cd ./priberam
- dotnet new tool-manifest

### Develop .Net Core Command
- dotnet new --list
- dotnet new webapi --name Classifier 
- dotnet sln add ./Classifier/Classifier.csproj
- cd ./Classifier
- dotnet add package Microsoft.EntityFrameworkCore.Tools
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer
- dotnet add package Microsoft.EntityFrameworkCore.Sqlite
- dotnet add package Microsoft.EntityFrameworkCore.Design
- dotnet add package Swashbuckle.AspNetCore

### Entity Framework Core Command
- dotnet tool install --global dotnet-ef
- dotnet ef migrations add CreateDatabase --project Classifier    
- dotnet ef database update --project Classifier  

## Skeleton Project
Main directories and files that make up the project: 
```
- Classifier 
|  - bin
|  - src
|  |  + Controllers
|  |  - Models
|  |  |  + DAO: Data Access Objects
|  |  |  + DTO: Data Transfer Objects
|  |  |  + ORM: Object Relational Mapping
|  |  - Services
|  |  |  + Algorithm 
|  |  |  + Startup
|  + Migrations
|  - db
|  |  |  + Dataset
|  |  |  |  - train.json
|  |  |  - mldb1.db
|  - appsettings.json
|  - Program.cs
- priberam.sln
- README.md
```
## Data model
```
--------------------------------
|           TopicWord          |
--------------------------------
| topic    | word    | count   |
................................
| string   | string  | integer |
--------------------------------

----------------------
|      TopicDoc      |
----------------------
| topic    | docs    |
......................
| string   | integer |
----------------------
```

### HTTP API definition:

1. _Receiving training documents_

   `POST /api/training/document`

   ```json
   {
     "text": "",
     "topic": ""
   }
   ```

   The response of this API call should be just HTTP code 200 on success or an error code otherwise.

2. _Receiving test documents_

   `POST /api/test/document`

   ```json
   {
     "text": ""
   }
   ```

   The response of this API call should be a JSON with the topic classification prediction on success, or an error code otherwise. An example response follows:

   ```json
   {
     "topic": "politics"
   }
   ```

A dataset is provided in file `train.json` with training documents already classified with topics (Source: It's a modified version of the data in <http://mlg.ucd.ie/datasets/bbc.html>).

### Naive Bayes Classifier

You can read the wikipedia page for context (<https://en.wikipedia.org/wiki/Naive_Bayes_classifier>). A simple way to implement the classifier is as follows.

1. For each received *training document* split it into words (a very simple approach is splitting by `' ',',','.',';','!','?'`). Each training document also comes with an associated topic.
2. You need to keep and update the count statistics for each (word, topic) pair. For example if you've seen so far 10 "business" documents and 5 "entertainment" documents with the word "car" on them your system needs to know that `"car": {"business": 10, "entertainment": 5}`. Your system should know this for all words. your system should also keep the global topic counts, e.g., if it has seen 235 entertainment documents `{"entertainment": 235}`.
3. When the program receives a *test document*, the goal is to predict a topic for this document. For this you can run the Naive Bayes inference. For this, you can compute the probability of the document being of any of the topics and then select the topic with higher probability. You can compute the score of each topic `t` given document `d` as follows:

```
score(t, d) = log(p(t)) + sum(log(p(w_i|t)))
```

which is the probability of the topic times the sum of the probabilities of each word `w_i` given the topic. We're summing logs instead of multipling probabilities to avoid numerical problems. More details:

```
p(t) = "Number of documents seen with topic t" / "Total number of documents seen"
p(w_i|t) = "Number of documents of topic t seen with word w_i" / "Number of documents seen with topic t"
```

With this information the system should be able to compute score(t, d) for all possible t topics, choose the one with the highest probability and answer that.

### Database

The system should store all incoming training data in a database. The database schema should be made intelligently, such that training and testing is efficient. This means that for either the training or testing calls, the number of read and write operations should be as small as possible. For example, just storing the list of documents one per row in memory or in a table will not work since then for every testing call the system would need to consult *all* training documents read so far to produce a classification - *this is not acceptable*, as the complexity would grow linearly with the number of ingested documents.

A viable option here is to use the "Entity Framework Core" C# library with an SQLite database.

## Deliverable

You need to deliver a standalone software project, written in C# ".NET 5". The program can use existing library/frameworks, provided those are freely available and that we can install and run the program on our side (Windows/Linux).

_Important_: The classifier algorithm part must be implemented from scratch (not by importing an existing library).

When the program starts, it should launch the web service on `http://localhost:8080` and start listening for API requests.

We'll evaluate your project submission according to:

1. Correct implementation of the web service and classifier.
2. Code quality, readability, organization and comments.
3. Scalability of the proposed approach.
4. If the server is terminated for any reason and then restarted, it should maintain the state (what it has learned from the training data seen so far) in the database.


