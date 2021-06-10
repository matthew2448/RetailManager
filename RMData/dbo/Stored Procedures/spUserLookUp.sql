CREATE PROCEDURE [dbo].[spUserLookUp]
	@Id nvarchar(128)
AS
begin
	set nocount on;

	SELECT FirstName, LastName, EmailAddress, CreatedDate 
	From [dbo].[User]
	Where Id = @Id;

end