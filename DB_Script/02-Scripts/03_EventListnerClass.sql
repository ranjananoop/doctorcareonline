INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERCLASSES]
           ([CLASSNAME]
           ,[ASSEMBLYNAME]
           ,[ASSEMBLYPATH]
           ,[INTERVALHOURS]
           ,[INTERVALMINUTES]
           ,[INTERVALSECONDS]
           ,[RUNONSTART]
           ,[STARTTIME]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('DOCVIDEO.EventListener.OrderListener'
           ,'DOCVIDEO.EventListener.dll'
           ,'E:\DOCCARE-ENH\ENH\DOCVIDEO.EventListener\bin\Debug\'
           ,0
           ,1
           ,0
           ,0
           ,null
           ,GETDATE()
           ,0
           ,null)
GO


