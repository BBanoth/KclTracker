param (
	[Parameter(Mandatory=$False)]
	[string]
	$Args
)
dotnet KclTracker.Database.Sql.dll $Args