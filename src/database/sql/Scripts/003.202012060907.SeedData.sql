SET ANSI_NULLS ON

GO

SET QUOTED_IDENTIFIER ON

GO

BEGIN
    IF NOT EXISTS (SELECT *
                   FROM   [dbo].[InspectionType])
      BEGIN
          INSERT INTO [dbo].[InspectionType]
                      (Id,
                       Name)
          VALUES      (1,
                       'QQQ')

          INSERT INTO [dbo].[InspectionType]
                      (Id,
                       Name)
          VALUES      (2,
                       'WWW')
      END
END

BEGIN
    IF NOT EXISTS (SELECT *
                   FROM   [dbo].[ShipmentStatus])
      BEGIN
          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (1,
                       'Shipment left the port',
                       1)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (2,
                       'KEPA approved shipment',
                       2)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (3,
                       'Shipment is cleared by KCL',
                       3)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (4,
                       'Samples collected by KCL',
                       4)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (5,
                       'KEPA approved Bayan',
                       5)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (6,
                       'Bayan sent to KEPA',
                       6)

          INSERT INTO [dbo].[ShipmentStatus]
                      (Id,
                       Name,
                       OrderId)
          VALUES      (7,
                       'Bayan has been shipped',
                       7)
      END
END 
