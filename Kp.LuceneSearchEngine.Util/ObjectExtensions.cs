using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kp.LuceneSearchEngine.Util
{
    public class ObjectExtensions
    {
        public static void CopyProperties(object source, object destination)
        {
            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            var sourceProperties = sourceType.GetProperties();
            foreach (var property in sourceProperties)
            {
                if (destinationType.GetProperty(property.Name) != null)
                {
                    property.SetValue(destination, property.GetValue(source));
                }
            }
        }
    }
}
