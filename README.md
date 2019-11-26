# Getting Started

## Environments

### Token settings
* **"AppSettings:APISettings:PublicKey"**
* **"AppSettings:APISettings:ISSUER"**
* **"AppSettings:APISettings:AUDIENCE"**
* **"AppSettings:APISettings:RefreshTokenExpiresInMinutes"**
* **"AppSettings:APISettings:AccessTokenExpiresInMinutes"**

### Database connection
* **"DB_HOST"** - address with database
* **"DB_PORT"** - port for access to database
* **"DB_NAME"** - name of database
* **"DB_USER"** - name of database user
* **"DB_PASSWORD"** - password of database user

### Google authenticate
* **"GoogleOAuth:ClientId"** - Client id of google project
* **"GoogleOAuth:ClientSecret"** - Client secret of google project

### Front-end urls
* **"StaticUrls:SetPasswordUrl"** - Url to page for set password (for registration)
* **"StaticUrls:ResetPasswordUrl"** - Url to page for set new password
* **"StaticUrls:InviteToCompanyPageUrl"** - Url for invite user to company
* **"StaticUrls:InviteToTeamPageUrl"** - Url for invite user to team
* **"StaticUrls:InviteToProjectPageUrl"** - Url for invite user to project

### Email (smtp)
* **"EmailSettings:SmtpServer"**
* **"EmailSettings:SmtpPort"**
* **"EmailSettings:Name"**
* **"EmailSettings:Email"**
* **"EmailSettings:Password"**

### Rabbit (for publish and subscribe)
* **"RabbitConnection:Host"**
* **"RabbitConnection:Port"**
* **"RabbitConnection:Username"**
* **"RabbitConnection:Password"**

### Rabbit (for RabbitMQ himself)
* **"RABBITMQ_ERLANG_COOKIE"**
* **"RABBITMQ_DEFAULT_USER"**
* **"RABBITMQ_DEFAULT_PASS"**
* **"RABBITMQ_DEFAULT_VHOST"**