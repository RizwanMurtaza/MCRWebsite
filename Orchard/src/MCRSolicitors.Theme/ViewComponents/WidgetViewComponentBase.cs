using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace MCRSolicitors.Theme.ViewComponents;

/// <summary>
/// Base class for widget ViewComponents with common helper methods
/// Works with Orchard Core's System.Text.Json.Dynamic.JsonDynamicObject
/// </summary>
public abstract class WidgetViewComponentBase : ViewComponent
{
    /// <summary>
    /// Unwrap the widget data from the anonymous type wrapper passed by Component.InvokeAsync
    /// When using: new { widgetData = widget }, the Invoke method receives { widgetData: {...} }
    /// This method extracts the actual widget data from inside the wrapper.
    /// </summary>
    protected static dynamic? UnwrapWidgetData(dynamic? input)
    {
        if (input == null) return null;

        // Check if this is an anonymous type with a 'widgetData' property
        var type = input.GetType();
        if (type.Name.StartsWith("<>f__AnonymousType"))
        {
            var prop = type.GetProperty("widgetData", BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(input);
            }
        }

        // If not wrapped, return as-is
        return input;
    }

    /// <summary>
    /// Safely get a dynamic value using indexer or property access
    /// </summary>
    protected static dynamic? GetDynamicValue(dynamic? obj, string name)
    {
        if (obj == null) return null;

        try
        {
            // First try indexer access (for JsonDynamicObject)
            return obj[name];
        }
        catch
        {
            try
            {
                // Fallback to property access via reflection
                var type = obj.GetType();
                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop != null)
                {
                    return prop.GetValue(obj);
                }
            }
            catch { }
            return null;
        }
    }

    protected static string? GetTextField(dynamic? widgetData, string partName, string fieldName)
    {
        if (widgetData == null) return null;
        try
        {
            // Access using dynamic member access - JsonDynamicObject supports this
            dynamic? part = GetDynamicValue(widgetData, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;
            dynamic? text = GetDynamicValue(field, "Text");
            return text?.ToString();
        }
        catch
        {
            return null;
        }
    }

    protected static string? GetHtmlField(dynamic? widgetData, string partName, string fieldName)
    {
        if (widgetData == null) return null;
        try
        {
            dynamic? part = GetDynamicValue(widgetData, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;

            // Try Html first, then Text
            dynamic? html = GetDynamicValue(field, "Html");
            if (html != null) return html.ToString();

            dynamic? text = GetDynamicValue(field, "Text");
            return text?.ToString();
        }
        catch
        {
            return null;
        }
    }

    protected static string? GetMediaField(dynamic? widgetData, string partName, string fieldName)
    {
        if (widgetData == null) return null;
        try
        {
            dynamic? part = GetDynamicValue(widgetData, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;

            dynamic? paths = GetDynamicValue(field, "Paths");
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        return "/media/" + path.ToString();
                    }
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    protected static bool GetBoolField(dynamic? widgetData, string partName, string fieldName, bool defaultValue = false)
    {
        if (widgetData == null) return defaultValue;
        try
        {
            dynamic? part = GetDynamicValue(widgetData, partName);
            if (part == null) return defaultValue;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return defaultValue;
            dynamic? value = GetDynamicValue(field, "Value");
            if (value == null) return defaultValue;

            // Handle JsonElement bool
            if (value is bool boolVal)
            {
                return boolVal;
            }

            if (bool.TryParse(value.ToString(), out bool result))
            {
                return result;
            }
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    protected static decimal GetNumericField(dynamic? widgetData, string partName, string fieldName, decimal defaultValue = 0)
    {
        if (widgetData == null) return defaultValue;
        try
        {
            dynamic? part = GetDynamicValue(widgetData, partName);
            if (part == null) return defaultValue;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return defaultValue;
            dynamic? value = GetDynamicValue(field, "Value");
            if (value == null) return defaultValue;

            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }
            return defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    protected static List<dynamic> GetBagPartItems(dynamic? widgetData, string bagPartName)
    {
        var items = new List<dynamic>();
        if (widgetData == null) return items;

        try
        {
            dynamic? bagPart = GetDynamicValue(widgetData, bagPartName);
            if (bagPart == null) return items;

            dynamic? contentItems = GetDynamicValue(bagPart, "ContentItems");
            if (contentItems == null) return items;

            foreach (var item in contentItems)
            {
                if (item != null)
                {
                    items.Add(item);
                }
            }
        }
        catch
        {
            // Return empty list on error
        }

        return items;
    }

    /// <summary>
    /// Get a text field from an item's part
    /// </summary>
    protected static string? GetItemTextField(dynamic? item, string partName, string fieldName)
    {
        if (item == null) return null;
        try
        {
            dynamic? part = GetDynamicValue(item, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;
            dynamic? text = GetDynamicValue(field, "Text");
            return text?.ToString();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get an HTML field from an item's part
    /// </summary>
    protected static string? GetItemHtmlField(dynamic? item, string partName, string fieldName)
    {
        if (item == null) return null;
        try
        {
            dynamic? part = GetDynamicValue(item, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;

            // Try Html first, then Text
            dynamic? html = GetDynamicValue(field, "Html");
            if (html != null) return html.ToString();

            dynamic? text = GetDynamicValue(field, "Text");
            return text?.ToString();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get a media field from an item's part
    /// </summary>
    protected static string? GetItemMediaField(dynamic? item, string partName, string fieldName)
    {
        if (item == null) return null;
        try
        {
            dynamic? part = GetDynamicValue(item, partName);
            if (part == null) return null;
            dynamic? field = GetDynamicValue(part, fieldName);
            if (field == null) return null;

            dynamic? paths = GetDynamicValue(field, "Paths");
            if (paths != null)
            {
                foreach (var path in paths)
                {
                    if (path != null)
                    {
                        return "/media/" + path.ToString();
                    }
                }
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get the ContentItemId from an item
    /// </summary>
    protected static string? GetItemId(dynamic? item)
    {
        if (item == null) return null;
        try
        {
            dynamic? id = GetDynamicValue(item, "ContentItemId");
            return id?.ToString();
        }
        catch
        {
            return null;
        }
    }
}
