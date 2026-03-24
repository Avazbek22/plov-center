namespace PlovCenter.Application.Contract.Categories.Commands;

public sealed record ReorderCategoryItem(Guid CategoryId, int SortOrder);
