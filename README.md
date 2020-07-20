# HackerNewsClientAPI

API can be called by: https://localhost:5001/api/hackernews

Two values are required on appsettings.json:

      "BaseUrl": "https://hacker-news.firebaseio.com/v0/",
      "RequestCountLimit": "3"
      
Those include the HackerNews base url and also the limit for concurrent requests.
That will limit the amount of requests made at same time to the client API.
I decided not to include a Task.Wait in seconds for quick testing, but that could be applied for reducing even more the risk of overloading.
