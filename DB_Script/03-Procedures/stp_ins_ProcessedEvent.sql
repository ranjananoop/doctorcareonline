CREATE PROCEDURE [dbo].[stp_ins_ProcessedEvent]  
(  
 @QueuedEventId  INT,  
 @Request    Text = NULL,  
 @Response    Text = NULL  
)  
AS  
SET NOCOUNT ON  
  
 INSERT INTO ProcessedEvents   
     (PublishedEventId,   
   EventListenerId,   
   DateProcessed,   
   Request,   
   Response)  
 SELECT   
  PublishedEventId,   
  EventListenerId,   
  getdate(),   
  @Request,   
  @Response  
 FROM QueuedEvents  
 WHERE   
  QueuedEventId = @QueuedEventId  
  
 DELETE FROM QueuedEvents  
 WHERE   
  QueuedEventId = @QueuedEventId  