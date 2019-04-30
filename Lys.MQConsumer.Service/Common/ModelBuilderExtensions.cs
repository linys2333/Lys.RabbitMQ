using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Text;

namespace Lys.MQConsumer.Service.Common
{
    public static class ModelBuilderExtensions
    {
        public static void UseMySqlNamingStyle(this ModelBuilder modelBuilder, string prefix = null)
        {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.Name, b =>
                {
                    var tableName = ConvertUndercoresStyle(entity.ClrType.Name);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        tableName = $"{prefix.ToLower()}_{tableName}";
                    }
                    b.ToTable(tableName);

                    foreach (var property in entity.GetProperties())
                    {
                        var columnName = ConvertUndercoresStyle(property.Relational().ColumnName);
                        b.Property(property.Name).HasColumnName(columnName);
                    }
                });
            }
        }

        private static string ConvertUndercoresStyle(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;
            var newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]))
                {
                    if ((text[i - 1] != '_' && !char.IsUpper(text[i - 1])) ||
                        (char.IsUpper(text[i - 1]) && i < text.Length - 1 && !char.IsUpper(text[i + 1])))
                    {
                        newText.Append('_');
                    }
                }
                newText.Append(text[i]);
            }
            return newText.ToString().ToLower();
        }
    }
}