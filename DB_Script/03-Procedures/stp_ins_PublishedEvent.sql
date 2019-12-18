CREATE PROCEDURE [dbo].[stp_ins_PublishedEvent]      
(      
 @EventId  int,      
 @RecordId  int,      
 @UserId  int,  
 @RecUserId Varchar(100),      
 @AdditionalData Text = NULL      
)      
AS      
SET NOCOUNT ON      
DECLARE @PublishedEventId int      
      
-- Insert into the PublishedEvents table      
INSERT INTO PublishedEvents (EventId, RecordId, UserId,RECUSERID, AdditionalData)      
VALUES (@EventId, @RecordId, @UserId,@RecUserId, @AdditionalData)      
SET @PublishedEventId = SCOPE_IDENTITY()      
      
-- Insert into the QueuedEvents table so the event will be processed      
-- the next time the event listener service runs      
INSERT INTO QueuedEvents (PublishedEventId, EventListenerId)      
 SELECT PublishedEvents.PublishedEventId, EventSubscriptions.EventListenerId      
 FROM Events INNER JOIN      
 PublishedEvents ON Events.EventId = PublishedEvents.EventId INNER JOIN      
 EventSubscriptions ON Events.EventId = EventSubscriptions.EventId      
 WHERE (PublishedEvents.PublishedEventId = @PublishedEventId) 