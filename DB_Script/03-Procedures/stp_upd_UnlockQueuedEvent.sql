
CREATE PROCEDURE [dbo].[stp_upd_UnlockQueuedEvent]  
@QueuedEventId int  
AS  
  
SET NOCOUNT ON  
UPDATE QueuedEvents  
SET GUID = null,  
DateLocked = null  
WHERE QueuedEventId = @QueuedEventId