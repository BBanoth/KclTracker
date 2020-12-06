SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'InspectionType'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'dbo'))
  CREATE TABLE [dbo].[InspectionType]
    (
       [Id]        [INT] NOT NULL,
       [Name]      [NVARCHAR](450) NOT NULL,
       [IsActive]  BIT NOT NULL DEFAULT 1,
       [IsDeleted] BIT NOT NULL DEFAULT 0
       CONSTRAINT [PK_InspectionType] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'Shipment'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'dbo'))
  CREATE TABLE [dbo].[Shipment]
    (
       [Id]                 [INT] IDENTITY(1, 1) NOT NULL,
       [DeclarationNumber]  NVARCHAR(100) NOT NULL,
       [CdnNumber]          NVARCHAR(100) NOT NULL,
       [CompanyId]          INT NOT NULL,
       [PartName]           NVARCHAR(100) NOT NULL,
       [DateOfEntry]        DATETIME NOT NULL,
       [InspectionTypeId]   INT NOT NULL,
       [ContainerNumbers]   NVARCHAR(1000) NOT NULL,
       [NumberOfContainers] INT NOT NULL,
       [CreatedOn]          DATETIME NOT NULL,
       [CreatedBy]          NVARCHAR(450) NOT NULL,
       [ModifiedOn]         DATETIME NULL,
       [ModifiedBy]         NVARCHAR(450) NULL,
       [IsActive]           BIT NOT NULL DEFAULT 1,
       [IsDeleted]          BIT NOT NULL DEFAULT 0
       CONSTRAINT [PK_Shipment] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_Shipment_Company_CompanyId')
  ALTER TABLE [dbo].[Shipment]
    WITH CHECK ADD CONSTRAINT [FK_Shipment_Company_CompanyId] FOREIGN KEY([CompanyId]) REFERENCES [security].[Company] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_Shipment_InspectionType_InspectionTypeId')
  ALTER TABLE [dbo].[Shipment]
    WITH CHECK ADD CONSTRAINT FK_Shipment_InspectionType_InspectionTypeId FOREIGN KEY([InspectionTypeId]) REFERENCES [dbo].[InspectionType] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'ShipmentStatus'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'dbo'))
  CREATE TABLE [dbo].[ShipmentStatus]
    (
       [Id]        [INT] NOT NULL,
       [Name]      [NVARCHAR](450) NOT NULL,
       [OrderId]   [INT] NOT NULL,
       [IsActive]  BIT NOT NULL DEFAULT 1,
       [IsDeleted] BIT NOT NULL DEFAULT 0
       CONSTRAINT [PK_ShipmentStatus] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT *
              FROM   sys.tables
              WHERE  NAME = 'ShipmentHistory'
                     AND schema_id = (SELECT schema_id
                                      FROM   sys.schemas
                                      WHERE  NAME = 'dbo'))
  CREATE TABLE [dbo].[ShipmentHistory]
    (
       [Id]               [INT] IDENTITY(1, 1) NOT NULL,
       [ShipmentId]       [INT] NOT NULL,
       [ShipmentStatusId] [INT] NOT NULL,
       [UpdatedOn]        DATETIME NOT NULL,
       [IsActive]         BIT NOT NULL DEFAULT 1,
       [IsDeleted]        BIT NOT NULL DEFAULT 0
       CONSTRAINT [PK_ShipmentHistory] PRIMARY KEY CLUSTERED ( [Id] ASC )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    )
  ON [PRIMARY]

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_ShipmentHistory_Shipment_ShipmentId')
  ALTER TABLE [dbo].[ShipmentHistory]
    WITH CHECK ADD CONSTRAINT FK_ShipmentHistory_Shipment_ShipmentId FOREIGN KEY([ShipmentId]) REFERENCES [dbo].[Shipment] ([Id]) ON DELETE CASCADE

GO

IF NOT EXISTS(SELECT 1
              FROM   INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
              WHERE  CONSTRAINT_NAME = 'FK_ShipmentHistory_ShipmentStatus_ShipmentStatusId')
  ALTER TABLE [dbo].[ShipmentHistory]
    WITH CHECK ADD CONSTRAINT FK_ShipmentHistory_ShipmentStatus_ShipmentStatusId FOREIGN KEY([ShipmentStatusId]) REFERENCES [dbo].[ShipmentStatus] ([Id]) ON DELETE CASCADE

GO 
