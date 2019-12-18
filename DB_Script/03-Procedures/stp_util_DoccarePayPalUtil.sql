CREATE PROCEDURE [dbo].[stp_util_DoccarePayPalUtil]
AS

SET NOCOUNT ON

IF OBJECT_ID('tempdb..#SubscriptionTable') IS NOT NULL 
 BEGIN
  DROP TABLE #SubscriptionTable
 END

BEGIN TRY
 
 BEGIN TRANSACTION
 
 SELECT u.UserName 
 INTO #SubscriptionTable
 FROM dbo.USERS u (nolock)
 INNER JOIN DOCSUBSCRIPTIONS ds ON u.UserName=ds.UserName
 WHERE DATEDIFF(M,ds.StartingDate,GETDATE())=1
 ORDER BY ds.StartingDate

 
 DECLARE @UserName VARCHAR(100)
 
 DECLARE SubscriptionCursor CURSOR FOR 
 SELECT UserName FROM #SubscriptionTable

 OPEN SubscriptionCursor

 FETCH NEXT FROM SubscriptionCursor 
 INTO @UserName

 WHILE @@FETCH_STATUS = 0
 BEGIN  
	EXEC stp_ins_PublishedEvent 17,0,0,@UserName,'<Data><Note>Monthly Subscription</Note></Data>'
  FETCH NEXT FROM SubscriptionCursor 
  INTO @UserName
 END 
 CLOSE SubscriptionCursor
 DEALLOCATE SubscriptionCursor 
 DROP TABLE #SubscriptionTable 
 COMMIT TRANSACTION
END TRY
BEGIN CATCH
 IF @@TRANCOUNT > 0
     ROLLBACK TRANSACTION
END CATCH