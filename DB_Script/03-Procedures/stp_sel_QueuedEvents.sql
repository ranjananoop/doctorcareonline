
CREATE PROCEDURE stp_sel_QueuedEvents  
 @EventListenerClassId  INT,  
 @GUID      VARCHAR(50)  
AS  
  
SET NOCOUNT ON  
  
 DECLARE @DateLocked datetime  
 SELECT @DateLocked = getdate()  
  
 -- Update the rows that will be returned so no other process will get them.  
 UPDATE QueuedEvents  
 SET   
  QueuedEvents.GUID = @GUID,  
  QueuedEvents.DateLocked = @DateLocked  
 FROM QueuedEvents   
 INNER JOIN EventListeners ON QueuedEvents.EventListenerId = EventListeners.EventListenerId  
 WHERE   
   (EventListeners.EventListenerClassId = @EventListenerClassId)   
  AND (QueuedEvents.DateLocked IS NULL)   
  
 SELECT TOP 100 PERCENT QueuedEvents.QueuedEventId  
 FROM QueuedEvents INNER JOIN  
 EventListeners ON QueuedEvents.EventListenerId = EventListeners.EventListenerId INNER JOIN  
 PublishedEvents ON QueuedEvents.PublishedEventId = PublishedEvents.PublishedEventId  
 WHERE (EventListeners.EventListenerClassId = @EventListenerClassId)   
  AND (QueuedEvents.GUID = @GUID)   
  AND (QueuedEvents.DateLocked = @DateLocked)   
  AND (EventListeners.IsDeleted = 0)  
 ORDER BY PublishedEvents.EventDate, QueuedEvents.QueuedEventId  
  
 -- Get the event details  
 SELECT   
  QueuedEvents.QueuedEventId,   
  QueuedEvents.PublishedEventId,   
  QueuedEvents.EventListenerId,   
  PublishedEvents.EventDate,   
  PublishedEvents.RecordId,
  PublishedEvents.RecUserId,   
  PublishedEvents.AdditionalData,  
  PublishedEvents.EventID  
 FROM QueuedEvents   
 INNER JOIN PublishedEvents ON QueuedEvents.PublishedEventId = PublishedEvents.PublishedEventId   
 INNER JOIN EventListeners ON QueuedEvents.EventListenerId = EventListeners.EventListenerId  
 WHERE   
   (EventListeners.EventListenerClassId = @EventListenerClassId)  
  AND (QueuedEvents.GUID = @GUID)   
  AND (QueuedEvents.DateLocked = @DateLocked)  
  AND (EventListeners.IsDeleted = 0)  
   
 -- Get the event attributes  
 SELECT TOP 100 PERCENT   
   QueuedEvents.QueuedEventId,   
   EventListenerAttributes.AttributeName,   
   EventListenerAttributes.AttributeValue,  
   EventListenerAttributes.IsXML,  
   EventListenerAttributes.HtmlDecode  
 FROM EventListenerAttributes   
 INNER JOIN EventListeners ON EventListenerAttributes.EventListenerId = EventListeners.EventListenerId   
 INNER JOIN QueuedEvents ON EventListeners.EventListenerId = QueuedEvents.EventListenerId  
 WHERE   
   (EventListeners.EventListenerClassId = @EventListenerClassId)   
  AND (QueuedEvents.GUID = @GUID)   
  AND (QueuedEvents.DateLocked = @DateLocked)  
  AND (EventListeners.IsDeleted = 0)   
  AND (EventListenerAttributes.IsDeleted = 0)  


