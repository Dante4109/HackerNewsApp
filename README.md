# Hacker News App

## Full Stack Code Challenge.

### :rocket: View the app on Azure here --> https://hackernews-frontend.azurewebsites.net/ 

## Prerequisites

Ensure your system has the following installed:

- [Node.js (LTS)](https://nodejs.org/) (Recommended: Latest LTS version)
- [Angular CLI](https://angular.io/cli) (Install via `npm install -g @angular/cli`)
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Git](https://git-scm.com/)

# 1. Clone the Repository

### Open a terminal and run:

```sh
git clone https://github.com/Dante4109/HackerNewsApp.git

cd HackerNewsApp
```

# 2. Backend Setup (.NET 9)

## Navigate to the **Backend** directory

```sh
cd Backend
```

### Restore Dependencies

```sh
dotnet restore
```

### Build the Project

```sh
dotnet build
```

### Navigate to the Api directory

```sh
cd HackerNews.Api
```

## Start the Backend Server

```sh
dotnet run
```

## 3. Frontend Setup (Angular 19)

### Open a separate terminal

### Navigate to the **Frontend** folder:

```sh
cd Frontend
```

### Install Dependencies

```sh
npm install
```

### Configure Environment Variables (Optional)

If needed, edit the `environment.ts` file inside `src/environments/:`
to match the localhost address from the backend api

```typescript
export const environment = {
  production: false,
  apiUrl: "http://localhost:5292/api/hackernews",
};
```

### Launch the Angualar SPA

```sh
ng serve
```

### Run Frontend Tests

### Open a separate terminal in the **Frontend** directory

```sh
ng test
```

# 4. Verify Everything Works

- Open http://localhost:4200/ in your browser.
- Ensure the frontend connects to the backend properly.
- Check the console/logs for any errors.

# 5. Run Tests

- ### Backend Tests:

Change directory to HackerNews.Api.Tests

```sh
cd ../Backend/HackerNews.Api.Tests
```

### Run Tests

```sh
dotnet test
```

- ### Fronted Tests:

Change directory to **Frontend**

```sh
cd ../Backend/HackerNews.Api.Tests
```

### Run Tests

```sh
ng test
```
