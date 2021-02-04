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

   2.8 [Email configuration](#Email)

   2.9 [Forgot password](#Forgot-password)

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
with your awesome functionality.

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

`Login` method produces **Cookies** that have `X-Access-Token` and `X-Username`.
By default token is not assigned to the further requests and pages that require authorization won't work if you wouldn't include `Bearer` token.
That is why JWT has an event `OnMessageReceived` configured (in `CustomServices.cs`) that reads Cookies and sets http context to send Bearer token inside the request.
Each token is valid for 3 days (configurable in `appsettings.json`)

##### How passwords are handled

To complicate rainbow table usage against passwords additional layer on top of hashing was used.
By default each user has an unique SALT key (SHA512) generated and stored in `Users` table `passwordSalt` column. This salt is used to compute hash for password.

##### Roles

For sake of showing how the roles work - several were created:

- Administrator
- User

###### Role assignments

Each user could have one or more than one role. Role assignments stored in `Roles` table. Relationship between user and roles configured in one-to-many manner and cascade deletion enabled. That means no matter how many assignments user have all will be removed when user deleted.

User with email **admin@localhost** gets **Administrator** role assigned by default.
Other users get **User** role.

**_How to know role works_**:

`User` role has an access to read all users from DB. You can do that by opening page `/user/GetAllUsers`. To check `Administrator` role you can try to Remove user from same list or access role assignment page `/user/GetAllRoleAssignments`.

##### Handling unauthorized access

All response with **401** and **403** status will be redirected to `/error/showError?errorCode={responseCode}` page and user will be notified with error message.
This behavior configured in `CustomConfiguration.cs` using `UseStatusCodePages` middleware.

##### Communication with end user

Shared \_Layout.cshtml has a modal window that hidden. If `ViewData["modalMessage"] ` is not empty then user will get a popup window with that message:

![ModalMessage](readmeContent/modalWindow1.jpg?raw=true)

##### Email

This project use SendGrid as email provider. If you want to use same implementation you need to register account _(you can create for free)_. In your SendGrid dashboard you need to generate an API key and then update `appsettings.json` with following directive:

```
  "SendGrid": {
    "ApiKey": "YOUR_API_KEY",
    "SenderEmail": "sendFrom@email.address",
    "SenderName":  "Sender Name"
  }
```

How to create a SendGrid API key: https://sendgrid.com/docs/ui/account-and-settings/api-keys/#creating-an-api-key

##### Forgot Password

User might reset password in case it was forgotten. From login form user has to click on "Forgot password" link and enter email.
What happens when user submit form:

- Service `ActionKeyService.cs` will generate a key (if user exist in database) and store it in the `UserActionKeys` table
- Service `SendGridService.cs` will send a mail to the user
- User open address from e-mail that contain key
- App will check if key exist and check associated user
- User enters a new password and submit forms
- Form contain new password, _email and key (hidden html elements)_ goes to the appropriate controller action
- Controller verifies request and change password

### Logging

`Serilog` package is used to log events both to the console and file. Log file will be created daily in `./Log` folder with prefix `log{Date}.txt`. By default 31 files will be retained.
Configuration could be found in `appsettings.json`.

Default configuration:

```
"Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "restrictedToMinimumLevel": "Information"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "./logs/log.txt",
          "restrictedToMinimumLevel": "Information",
          "RollingInterval": "Day"
        }
      }
    ]
  }
```

### Bottom line

I purposely removed bootstrap and JQuery from project to try JS\HTML\CSS. This is my first project that includes FrontEnd created from scratch. I am pretty sure that CSS\HTML parts could be optimized (well, to be honest c# part also could be optimized😎).
