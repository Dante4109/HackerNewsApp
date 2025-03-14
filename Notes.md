Get top news stories here: https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty

Example story: https://hacker-news.firebaseio.com/v0/item/43357856.json?print=pretty

Steps:

.NET backend

- Create WebAPI project with dotnet cli
  - used `$ dotnet new webapi -o -n HackerNews.API --user-controller`
- Create HackerNewsService
- Add Services to program.cs
- Create HackerNewsController
- Test the controller
- Implement the caching mechanism
- Implement HackerNewsService Test
- Implement HackerNewsController Test

Angular Frontend

- Create frontend with ng cli
- Create Hacker News Service as observable for fetching data from backend api
- Create component for displaying stories
- Create basic UI to display stories
- Test to confirm data is display correctly
- Implement client side pagination
- Add buttons to switch between pages
- Test to confirm pages change each time.
- Add search function using the forms module.
