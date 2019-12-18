INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Doctor SignUp'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Doctor SignUp Admin'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Patient SignUp'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Cancel Appointment'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO


INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Patient Appointment Confirm'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Doctor Appointment Confirm'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

--BookYourAppointmentClinic

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Patient BookYour Appointment Clinic'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO

INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Doctor BookYour Appointment Clinic'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO
--ForgotPassword
INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Forgot Password'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO
--PasswordReset
INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Password Reset'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO
--GetDoctorsUpdate
INSERT INTO [DOCCAREONLINE].[dbo].[EVENTLISTENERS]
           ([EVENTLISTENERDESC]
           ,[EVENTLISTENERCLASSID]
           ,[DATEADDED]
           ,[ISDELETED]
           ,[DATEDELETED])
     VALUES
           ('Get Doctors Update'
           ,1
           ,GETDATE()
           ,0
           ,NULL)
GO