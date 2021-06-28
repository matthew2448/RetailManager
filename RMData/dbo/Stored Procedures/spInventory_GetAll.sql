CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
begin
	set nocount on;

	select [Id], [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	From dbo.Inventory;
end
