# covid-dataset

This is a test repository that stores a random Covid dataset.

The data cames from https://github.com/deepset-ai/COVID-QA and is subject to the following licence https://github.com/deepset-ai/COVID-QA/blob/master/LICENSE.

# data
The data contained in the csv files are separated by the **pipe** (**|**) character in the following format: `id|category|question|answer|datetime`.

The `id` is the unique identifier of the question.
The `category` is the group that identify the question. 
The `question` contains the text of the question itself.
The `answer` contains the answer to that question.
The `datetime` contains the timestamp of the question.

# what the candidate should do
The candidate should use the csv files to **store** them in a repository of choice. Then this data should be **exposed** via a Rest Api endpoints, so it will be available via the following GET requests:

 1. The top 10 most asked questions
 2. The top 10 most used category

# where to store the code
The code should be stored in this repository. The following projects should be added to it:
  
 1. A project to load the csv files and store them in a repository
 2. A project to load the repository and expose the previously mentioned GET endpoints via Rest API

# constraints
There's *no restriction* on the use of a language for importing the files to the repository, the candidate can pick up one he/she likes but the preference is C# or Python.
There's *no restriction* on the use of a repository, the preference is for SqlServer/PostgreSql/Memory.
There's a **restriction** in exposing the data, it must be a ASP .NET Core Web Application project 


