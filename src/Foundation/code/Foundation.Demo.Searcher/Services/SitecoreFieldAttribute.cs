using System;

namespace Foundation.Demo.Search.Services
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SitecoreFieldAttribute : Attribute
    {
        public string FieldName;
        public Guid FieldId;

        public SitecoreFieldAttribute() { }

        public SitecoreFieldAttribute(string fieldName, string id)
        {
            FieldName = fieldName;
            FieldId = Guid.Parse(id);
        }
    }
}