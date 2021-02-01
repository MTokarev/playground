1. [About](#About)

   1.1 [Demo](#Small-Demo)

2. [What is inside](#What-is-inside)

   2.1 [Where is service configuration](#Where-is-service-configuration)

   2.2 [Auth](#Auth)

   2.3 [How token is set](#How-token-is-set)

   2.4 [How passwords are handled](#How-passwords-are-handled)

   2.5 [Roles](#Roles)

   2.6 [Handling unauthorized access](#Handling-unauthorized-access)

   2.7 [Communication with end user](#Communication-with-end-user)

3. [Logging](#Logging)

4. [Bottom line](#Bottom-line)

### About

This is an app that has some futures built-in and ready to go
for web projects that require _Authentication_ and _Authorization_ enabled.
That includes registering\store users in database.

#### Small Demo:

![Demo](readmeContent/playgroundDemo.gif)

### Why that

You can benefit from this project by using it as a base and extend
with your own functionality.

### What is inside

##### Where is service configuration

To keep `Startup.cs` file clean all configuration was moved `=>` `Extension`
folder. Helper classes are handling adding and configuring services.

##### Auth

JWT tokens are used to provide authorization. There are several claims that assigned to the user out of box:

```
- Email address in _NameId_ field
- Valid audience (can be configured inside appsettings.json)
- Role list (to handle auth based on roles)
```

##### How token is set

`Login` method produces **Cookies** that have `X-Access-Token` and `X-Username` set.
By default token is not assigned to the requests and wouldn't work if you won't
add Bearer token by default.
That is why JWT has an event `OnMessageReceived` configured (in `CustomServices.cs`) that
reads Cookies and sets http context to send Bearer token inside the request.
Each token is valid for 3 days (you can configure this setting in `appsettings.json`)

##### How passwords are handled

To complicate rainbow table usage against passwords additional layer on top of hashing was used.
By default each user has an unique SALT key (SHA512) generated and stored in Users table `passwordSalt` column. This salt is used to compute hash for password.

##### Roles

For sake of showing how the roles work few entries were created:

- Administrator
- User

###### Role assignments

Each user could have one or more than one role. Role assignments stored in `Roles` table. Relationship between user and roles configured in one-to-many manner and cascade deletion enabled. That means no matter how many assignments user has all will be removed when user deleted.

User with email **admin@localhost** gets **Administrator** role assigned by default.
Other users get **User** role assigned.

**_How to know role works_**:

`User` role has an access to get all users from DB, open page '/user/GetAllUsers' to retrieve a list. To check `Administrator` role you can try to Remove user from '/user/GetAllUsers'.
Other way of checking is to access role assignment page
'/user/GetAllRoleAssignments'.

##### Handling unauthorized access

All response with 401 and 403 status redirect to "/error/showError?errorCode={responseCode}".
This behavior configured in `CustomConfiguration.cs` using `UseStatusCodePages` middleware.

##### Communication with end user

Shared \_Layout.cshtml has a modal window that hidden. If `ViewData["modalMessage"] ` is not empty then user will get a popup window with that message:

![ModalMessage](readmeContent/modalWindow1.jpg?raw=true)

### Logging

`Serilog` package is used to log events bot to the console and file.
Configuration is in `Program.cs`, log will be created daily in `Log` folder with prefix `log{Data}.txt`. By default 31 files will be retained.
Configuration could be found in `appsettings.json`

### Bottom line

I purposely removed bootstrap and JQuery from project to try JS\HTML\CSS.
This is my first project that includes FrontEnd created from scratch.
I am pretty sure that CSS\HTML parts could be optimized.
