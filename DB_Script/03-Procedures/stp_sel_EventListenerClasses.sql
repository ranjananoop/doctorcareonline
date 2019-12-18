
CREATE PROCEDURE dbo.stp_sel_EventListenerClasses  
AS  
  
SET NOCOUNT ON  
  
SELECT ClassId,  
ClassName,  
AssemblyName,  
AssemblyPath,  
IntervalHours,  
IntervalMinutes,  
IntervalSeconds,  
RunOnStart,  
StartTime  
FROM EventListenerClasses  
WHERE (IsDeleted = 0)


