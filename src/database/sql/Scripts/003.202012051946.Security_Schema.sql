SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

--IdentityServer Tables
------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT schema_name
               FROM   information_schema.schemata
               WHERE  schema_name = 'idp')
  EXEC Sp_executesql
    N'CREATE SCHEMA idp;'

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'PersistedGrants'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'idp'))
  CREATE TABLE [idp].[PersistedGrants]
    (
       [Key]          NVARCHAR(200) NOT NULL,
       [Type]         NVARCHAR(50) NOT NULL,
       [SubjectId]    NVARCHAR(200) NULL,
       [SessionId]    NVARCHAR(100) NULL,
       [ClientId]     NVARCHAR(200) NOT NULL,
       [Description]  NVARCHAR(200) NULL,
       [CreationTime] DATETIME2 NOT NULL,
       [Expiration]   DATETIME2 NULL,
       [ConsumedTime] DATETIME2 NULL,
       [Data]         NVARCHAR(max) NOT NULL,
       CONSTRAINT [PK_PersistedGrants] PRIMARY KEY CLUSTERED ( [Key] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'DeviceCodes'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'idp'))
  CREATE TABLE [idp].[DeviceCodes]
    (
       [UserCode]     NVARCHAR(200) NOT NULL,
       [DeviceCode]   NVARCHAR(200) NOT NULL,
       [SubjectId]    NVARCHAR(200) NULL,
       [SessionId]    NVARCHAR(100) NULL,
       [ClientId]     NVARCHAR(200) NOT NULL,
       [Description]  NVARCHAR(200) NULL,
       [CreationTime] DATETIME2 NOT NULL,
       [Expiration]   DATETIME2 NOT NULL,
       [Data]         NVARCHAR(max) NOT NULL,
       CONSTRAINT [PK_DeviceCodes] PRIMARY KEY CLUSTERED ( [UserCode] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT 1
              FROM   sys.indexes
              WHERE  object_id = Object_id('[idp].[DeviceCodes]')
                     AND name = 'IX_DeviceCodes_DeviceCode')
  CREATE UNIQUE INDEX [IX_DeviceCodes_DeviceCode]
    ON [idp].[DeviceCodes] ([DeviceCode]);

GO

IF NOT EXISTS(SELECT 1
              FROM   sys.indexes
              WHERE  object_id = Object_id('[idp].[DeviceCodes]')
                     AND name = 'IX_DeviceCodes_Expiration')
  CREATE INDEX [IX_DeviceCodes_Expiration]
    ON [idp].[DeviceCodes] ([Expiration]);

GO

IF NOT EXISTS(SELECT 1
              FROM   sys.indexes
              WHERE  object_id = Object_id('[idp].[PersistedGrants]')
                     AND name = 'IX_PersistedGrants_Expiration')
  CREATE INDEX [IX_PersistedGrants_Expiration]
    ON [idp].[PersistedGrants] ([Expiration]);

GO

IF NOT EXISTS(SELECT 1
              FROM   sys.indexes
              WHERE  object_id = Object_id('[idp].[PersistedGrants]')
                     AND name = 'IX_PersistedGrants_SubjectId_ClientId_Type')
  CREATE INDEX [IX_PersistedGrants_SubjectId_ClientId_Type]
    ON [idp].[PersistedGrants] ([SubjectId], [ClientId], [Type]);

GO

IF NOT EXISTS(SELECT 1
              FROM   sys.indexes
              WHERE  object_id = Object_id('[idp].[PersistedGrants]')
                     AND name = 'IX_PersistedGrants_SubjectId_SessionId_Type')
  CREATE INDEX [IX_PersistedGrants_SubjectId_SessionId_Type]
    ON [idp].[PersistedGrants] ([SubjectId], [SessionId], [Type]);

GO

--Security Tables
------------------------------------------------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT schema_name
               FROM   information_schema.schemata
               WHERE  schema_name = 'security')
  EXEC Sp_executesql
    N'CREATE SCHEMA security;'

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetRoles'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetRoles]
        (
           [Id]               [NVARCHAR](450) NOT NULL,
           [Name]             [NVARCHAR](256) NULL,
           [NormalizedName]   [NVARCHAR](256) NULL,
           [ConcurrencyStamp] [NVARCHAR](max) NULL,
           CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetRoleClaims'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetRoleClaims]
        (
           [Id]         [INT] IDENTITY NOT NULL,
           [RoleId]     [NVARCHAR](450) NOT NULL,
           [ClaimType]  [NVARCHAR](max) NULL,
           [ClaimValue] [NVARCHAR](max) NULL,
           CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetRoleClaims_AspNetRoles_RoleId')
  ALTER TABLE [security].[AspNetRoleClaims]
    WITH CHECK ADD CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [security].[AspNetRoles] ([Id]) ON DELETE CASCADE

GO

ALTER TABLE [security].[AspNetRoleClaims]
  CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUserTypes'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  CREATE TABLE [security].[AspNetUserTypes]
    (
       Id        INT NOT NULL,
       Name      NVARCHAR(100) NOT NULL,
       IsActive  BIT NOT NULL DEFAULT 1,
       IsDeleted BIT NOT NULL DEFAULT 0
       CONSTRAINT [PK_AspNetUserTypes] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUsers'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetUsers]
        (
           [Id]                   [NVARCHAR](450) NOT NULL,
           [UserName]             [NVARCHAR](256) NULL,
           [NormalizedUserName]   [NVARCHAR](256) NULL,
           [Email]                [NVARCHAR](256) NULL,
           [NormalizedEmail]      [NVARCHAR](256) NULL,
           [EmailConfirmed]       [BIT] NOT NULL,
           [PasswordHash]         [NVARCHAR](max) NULL,
           [SecurityStamp]        [NVARCHAR](max) NULL,
           [ConcurrencyStamp]     [NVARCHAR](max) NULL,
           [PhoneNumber]          [NVARCHAR](max) NULL,
           [PhoneNumberConfirmed] [BIT] NOT NULL,
           [TwoFactorEnabled]     [BIT] NOT NULL,
           [LockoutEnd]           [DATETIMEOFFSET](7) NULL,
           [LockoutEnabled]       [BIT] NOT NULL,
           [AccessFailedCount]    [INT] NOT NULL,
           [UserTypeId]           [INT] NOT NULL,
           [IsActive]             [BIT] NOT NULL
           CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO


IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUsers_AspNetUserTypes_UserTypeId')
  ALTER TABLE [security].[AspNetUsers]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUsers_AspNetUserTypes_UserTypeId] FOREIGN KEY([UserTypeId]) REFERENCES [security].[AspNetUserTypes] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'Company'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  CREATE TABLE [security].[Company]
    (
       [Id]        [INT] NOT NULL,
       [Name]       [NVARCHAR](450) NOT NULL
       CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO


IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'UserCompany'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[UserCompany]
        (
           [Id]        [INT] IDENTITY(1, 1) NOT NULL,
           [UserId]    [NVARCHAR](450) NOT NULL,
           [CompanyId]              INT NOT NULL,
       [IsActive]  BIT NOT NULL DEFAULT 1,
       [IsDeleted] BIT NOT NULL DEFAULT 0
           CONSTRAINT [PK_UserCompany] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO


IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_UserCompany_AspNetUsers_UserId')
  ALTER TABLE [security].[UserCompany]
    WITH CHECK ADD CONSTRAINT [FK_UserCompany_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_UserCompany_Company_CompanyId')
  ALTER TABLE [security].[UserCompany]
    WITH CHECK ADD CONSTRAINT [FK_UserCompany_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [security].[Company] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUserClaims'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetUserClaims]
        (
           [Id]         [INT] NOT NULL,
           [UserId]     [NVARCHAR](450) NOT NULL,
           [ClaimType]  [NVARCHAR](max) NULL,
           [ClaimValue] [NVARCHAR](max) NULL,
           CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUserClaims_AspNetUsers_UserId')
  ALTER TABLE [security].[AspNetUserClaims]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

ALTER TABLE [security].[AspNetUserClaims]
  CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUserLogins'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetUserLogins]
        (
           [LoginProvider]       [NVARCHAR](128) NOT NULL,
           [ProviderKey]         [NVARCHAR](128) NOT NULL,
           [ProviderDisplayName] [NVARCHAR](max) NULL,
           [UserId]              [NVARCHAR](450) NOT NULL,
           CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ( [LoginProvider] ASC, [ProviderKey] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUserLogins_AspNetUsers_UserId')
  ALTER TABLE [security].[AspNetUserLogins]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

ALTER TABLE [security].[AspNetUserLogins]
  CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUserRoles'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetUserRoles]
        (
           [UserId] [NVARCHAR](450) NOT NULL,
           [RoleId] [NVARCHAR](450) NOT NULL,
           CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ( [UserId] ASC, [RoleId] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetRoles_RoleId')
  ALTER TABLE [security].[AspNetUserRoles]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId]) REFERENCES [security].[AspNetRoles] ([Id]) ON DELETE CASCADE

GO

ALTER TABLE [security].[AspNetUserRoles]
  CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUserRoles_AspNetUsers_UserId')
  ALTER TABLE [security].[AspNetUserRoles]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

ALTER TABLE [security].[AspNetUserRoles]
  CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'AspNetUserTokens'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  BEGIN
      CREATE TABLE [security].[AspNetUserTokens]
        (
           [UserId]        [NVARCHAR](450) NOT NULL,
           [LoginProvider] [NVARCHAR](128) NOT NULL,
           [Name]          [NVARCHAR](128) NOT NULL,
           [Value]         [NVARCHAR](max) NULL,
           CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ( [UserId] ASC, [LoginProvider] ASC, [Name] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        )
      ON [PRIMARY]
  END

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_AspNetUserTokens_AspNetUsers_UserId')
  ALTER TABLE [security].[AspNetUserTokens]
    WITH CHECK ADD CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'UserProfile'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'security'))
  CREATE TABLE [security].[UserProfile]
    (
       [Id]        [INT] IDENTITY(1, 1) NOT NULL,
       [UserId]    [NVARCHAR](450) NOT NULL,
       [FirstName] [NVARCHAR](256) NULL,
       [LastName]  [NVARCHAR](256) NULL
       CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ( [UserId] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_UserProfile_AspNetUsers_UserId')
  ALTER TABLE [security].[UserProfile]
    WITH CHECK ADD CONSTRAINT [FK_UserProfile_AspNetUsers_UserId] FOREIGN KEY([UserId]) REFERENCES [security].[AspNetUsers] ([Id]) ON DELETE CASCADE

GO

