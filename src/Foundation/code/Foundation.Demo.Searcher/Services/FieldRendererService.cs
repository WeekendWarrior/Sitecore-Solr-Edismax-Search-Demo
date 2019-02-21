namespace Foundation.Demo.Search.Services
{
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Web;
    using Sitecore.Web.UI.WebControls;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public static class FieldRendererExtensions
    {
        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="string"/>.
        /// </summary>
        public static string RenderField(this Item item, string fieldName)
        {
            return FieldRenderer.Render(item, fieldName);
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="string"/>.
        /// </summary>
        public static string RenderField(this Item item, ID fieldId)
        {
            var field = item.Fields[fieldId];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            return FieldRenderer.Render(item, field.Name);
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="string"/>.
        /// </summary>
        public static string RenderField(this Item item, Guid fieldId)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            return FieldRenderer.Render(item, field.Name);
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="NameValueCollection"/>.
        /// </summary>
        public static NameValueCollection RenderNameValueField(this Item item, Guid fieldId)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string value = FieldRenderer.Render(item, field.Name);

            return WebUtil.ParseUrlParameters(value);
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="float"/>.
        /// </summary>
        public static float RenderFloatField(this Item item, Guid fieldId, float defaultValue = 0)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string value = FieldRenderer.Render(item, field.Name);

            float f;
            if (float.TryParse(value, out f))
            {
                return f;
            }

            return defaultValue;
        }

        /// <summary>
        /// Renders the sitecore field value and returns as an <see cref="int"/>.
        /// </summary>
        public static int RenderIntegerField(this Item item, Guid fieldId, int defaultValue = 0)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string value = FieldRenderer.Render(item, field.Name);

            int i;
            if (int.TryParse(value, out i))
            {
                return i;
            }

            return defaultValue;
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="Guid"/>.
        /// </summary>
        public static Guid RenderGuidField(this Item item, Guid fieldId)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string value = FieldRenderer.Render(item, field.Name);

            Guid g;
            if (Guid.TryParse(value, out g))
            {
                return g;
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="Guid"/>.
        /// </summary>
        public static Guid[] RenderGuidsField(this Item item, Guid fieldId)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string fieldValue = FieldRenderer.Render(item, field.Name);

            var results = new List<Guid>();

            // Value will be a pipe-delimited list of guids.
            foreach (string id in fieldValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Guid g;
                if (Guid.TryParse(id, out g))
                {
                    results.Add(g);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// Renders the sitecore field value and returns as a <see cref="bool"/>.
        /// </summary>
        public static bool RenderBoolField(this Item item, Guid fieldId)
        {
            var field = item.Fields[new ID(fieldId)];

            Assert.IsNotNull(field, "Field with id: " + fieldId + " is null on item " + item.Name);

            string value = FieldRenderer.Render(item, field.Name);

            return value != null && value.Equals("1", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}