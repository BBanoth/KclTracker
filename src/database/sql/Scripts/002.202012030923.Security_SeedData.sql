SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

BEGIN
    IF NOT EXISTS (SELECT *
                   FROM   [security].[AspNetUserTypes])
      BEGIN
          INSERT INTO [security].[AspNetUserTypes]
                      (Id,
                       Name)
          VALUES      (1,
                       'Admin')

          INSERT INTO [security].[AspNetUserTypes]
                      (Id,
                       Name)
          VALUES      (2,
                       'Standard')
      END
END 
