use SwingHouse;

DELETE FROM Messages WHERE Id > 1176
DELETE FROM RelationsForAccounts
UPDATE Messages SET IsDeleted = 0

SELECT * FROM [Messages] ORDER BY Id DESC; SELECT * FROM [RelationsForAccounts];
