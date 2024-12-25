UPDATE [SchedulesForEvents] SET 
StartDate = DATEADD(MONTH, 1, StartDate),
EndDate = DATEADD(MONTH, 1, EndDate)
