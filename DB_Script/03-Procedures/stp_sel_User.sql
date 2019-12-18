USE [DOCCAREONLINE]
GO
/****** Object:  StoredProcedure [dbo].[stp_sel_QueuedEvents]    Script Date: 10/06/2013 17:25:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[stp_sel_User]  
 @UserID VARCHAR(100)  
AS  
  
SET NOCOUNT ON  
  
 SELECT FIRSTNAME,LASTNAME,VERIFICATIONCODE FROM USERS where UserName=@UserID
  
 GO