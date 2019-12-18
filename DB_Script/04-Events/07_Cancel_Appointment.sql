USE [DOCCAREONLINE]
GO
--Doctor SignUp
DECLARE @ObjectTypeId INT
DECLARE @EventDescription VARCHAR(50)
DECLARE @DisplayDescription VARCHAR(250)
DECLARE @EventId INT
DECLARE @EventListenerId INT
--SET Values
SET @ObjectTypeId=3
SET @EventDescription='Cancel Appointment'
SET @DisplayDescription='Cancel Appointment'
SET @EventListenerId =1
IF NOT EXISTS(SELECT 1 FROM [DOCCAREONLINE].[dbo].[Events] WHERE EventDescription =@EventDescription )
BEGIN
	INSERT INTO [DOCCAREONLINE].[dbo].[Events]
			   ([ObjectTypeId]
			   ,[DateAdded]
			   ,[EventDescription]
			   ,[DisplayDescription])
		 VALUES
				(@ObjectTypeId
				,GETDATE()
				,@EventDescription
				,@DisplayDescription)
				
				SELECT @EventId = SCOPE_IDENTITY()
END
--Adding newly created ID to EventSubscriptions Table
IF (@EventId IS NOT NULL)
	BEGIN
	--print @EventId
		IF NOT EXISTS(SELECT 1 FROM EventSubscriptions WHERE EventId=@EventId)
		BEGIN
			INSERT INTO EventSubscriptions
			([EventId]
			,[EventListenerId]
			,[DateAdded]) 
			VALUES 
			(@EventId
			,@EventListenerId
			,GETDATE())
		END
   END
